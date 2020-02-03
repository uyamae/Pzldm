using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pzldm
{
    public partial class PlayField
    {
        /// <summary>
        /// バトルマネージャー
        /// </summary>
        public BattleManager BattleManager { get; set; }

        /// <summary>
        /// 対戦相手のこうげきだまパターンデータ
        /// </summary>
        [SerializeField]
        private AttackPatternData opponentAttackPattern;
        /// <summary>
        /// 対戦相手のこうげきだまパターンデータ
        /// </summary>
        public AttackPatternData OpponentAttackPattern
        {
            get { return opponentAttackPattern; }
            set { opponentAttackPattern = value; }
        }

        /// <summary>
        /// 今回適用するせり上げるこうげきだまの数
        /// </summary>
        private int currentLiftAttackCount;
        /// <summary>
        /// せり上げパターンのインデックス
        /// </summary>
        private int liftPatternIndex;

        /// <summary>
        /// 落下するこうげきだまのバッファ用データ
        /// </summary>
        private struct DropAttackBuffer
        {
            /// <summary>
            /// リンク用配列番号
            /// </summary>
            public int next;
            /// <summary>
            /// たまの色
            /// </summary>
            public ColorType color;
        }
        /// <summary>
        /// 落下するこうげきだまの列ごとのリンクリストの先頭
        /// </summary>
        private int[] dropAttackBufferIndices;
        /// <summary>
        /// 落下するこうげきだまのバッファ
        /// </summary>
        private DropAttackBuffer[] dropAttackBuffer;
        /// <summary>
        /// 一度に適用するこうげきだま最大数
        /// </summary>
        private int maxApplyAttackCount;

        /// <summary>
        /// 試合開始時にこうげきだまに関するパラメータを初期化
        /// </summary>
        private void InitAttackTamaParameters()
        {
            liftPatternIndex = 0;
            if (setting == null) return; // これはいかん
            if (dropAttackBufferIndices?.Length != setting.columnsCount)
            {
                dropAttackBufferIndices = null;
            }
            if (dropAttackBufferIndices == null)
            {
                dropAttackBufferIndices = new int[setting.columnsCount];
                for (int i = 0; i < dropAttackBufferIndices.Length; ++i)
                {
                    dropAttackBufferIndices[i] = -1;
                }
                dropAttackBuffer = new DropAttackBuffer[setting.columnsCount * OpponentAttackPattern.MaxDropLines];
            }
            maxApplyAttackCount = setting.columnsCount * OpponentAttackPattern.AttackDirectionPattern.Length;
        }
        /// <summary>
        /// 一回分のこうげきだま設定
        /// </summary>
        private void SetupCurrentAttackTama()
        {
            // 今回使う分
            int count = System.Math.Min(maxApplyAttackCount, RecievedAttackCount);
            RecievedAttackCount -= count;
            // 段数換算
            int lines = (count + setting.columnsCount - 1) / setting.columnsCount;
            // せり上げ段数
            int liftLines = 0;
            for (int i = 0; i < lines; ++i)
            {
                if (OpponentAttackPattern.AttackDirectionPattern[i] == AttackPatternData.AttackDirectionType.Bottom)
                {
                    ++liftLines;
                }
            }
            // せり上げ個数(列数の倍数にする)
            currentLiftAttackCount = 0;
            while ((liftLines > 0) && (count >= setting.columnsCount))
            {
                currentLiftAttackCount += setting.columnsCount;
                count -= setting.columnsCount;
            }
            // 残りを落下個数として落下こうげきだまバッファの設定
            SetupCurrentDropAttackTama(count);
        }
        /// <summary>
        /// 落下こうげきだまの設定
        /// </summary>
        private void SetupCurrentDropAttackTama(int count)
        {
            // リンクリストの先頭リセット
            for (int i = 0; i < dropAttackBufferIndices.Length; ++i)
            {
                dropAttackBufferIndices[i] = -1;
            }
            int index = 0;
            int attack = 0;
            while ((count > 0) && (attack < OpponentAttackPattern.DropAttackTamaPattern.Length))
            {
                // 先頭から順に調べる
                var data = OpponentAttackPattern.DropAttackTamaPattern[attack++];
                // フィールドが空いていたら
                if (tamaField[data.Position.y, data.Position.x] == null)
                {
                    // バッファの空き部分に登録
                    dropAttackBuffer[index].color = data.Color;
                    dropAttackBuffer[index].next = -1;
                    // リンクリストに接続
                    if (dropAttackBufferIndices[data.Position.x] < 0)
                    {
                        dropAttackBufferIndices[data.Position.x] = index;
                    }
                    else
                    {
                        int next = dropAttackBufferIndices[data.Position.x];
                        while (dropAttackBuffer[next].next >= 0)
                        {
                            next = dropAttackBuffer[next].next;
                        }
                        dropAttackBuffer[next].next = index;
                    }
                    ++index;
                    --count;
                }
            }
            // 未使用箇所の設定
            while (index < dropAttackBuffer.Length)
            {
                dropAttackBuffer[index].next = -1;
                ++index;
            }
        }
        /// <summary>
        /// こうげきだま適用ステートへの遷移
        /// </summary>
        private void ChangeStateToAttackTama()
        {
            SetupCurrentAttackTama();
            if (currentLiftAttackCount > 0)
            {
                ChangeState(PlayingState.LiftUpAttackTama);
            }
            else
            {
                ChangeState(PlayingState.DropAttackTama);
            }
        }
        /// <summary>
        /// こうげきだま１段分を適用するウェイト
        /// </summary>
        private int attackLineWaitFrame;
        #region せりあげ処理
        /// <summary>
        /// せり上げ開始
        /// </summary>
        private void StateLiftUpAttackTamaEnter()
        {
            attackLineWaitFrame = setting.attackLiftUpWaitFrame;
        }
        /// <summary>
        /// せり上げ処理
        /// </summary>
        private void StateLiftUpAttackTamaUpdate()
        {
            if (attackLineWaitFrame > 0)
            {
                --attackLineWaitFrame;
            }
            else if (currentLiftAttackCount <= 0)
            {
                ChangeState(PlayingState.DropAttackTama);
            }
            else
            {
                ApplyLiftUpAttackLine();
                attackLineWaitFrame = setting.attackLiftUpWaitFrame;
            }
        }
        /// <summary>
        /// せり上げ終了
        /// </summary>
        private void StateLiftUpAttackTamaLeave()
        {
        }
        /// <summary>
        /// せり上げこうげきだまを一段適用する
        /// </summary>
        private void ApplyLiftUpAttackLine()
        {
            if (currentLiftAttackCount <= 0) return;
            currentLiftAttackCount -= setting.columnsCount;

            var pattern = OpponentAttackPattern.LiftAttackTamaPattern;
            int offset = setting.columnsCount * liftPatternIndex;
            // せり上げパターンのループ
            ++liftPatternIndex;
            if (liftPatternIndex == pattern.Length / setting.columnsCount)
            {
                liftPatternIndex = 0;
            }
            // せり上げ処理
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = setting.rowsCount - 1; y > 0; --y)
                {
                    //var data = tamaField[y - 1, x];
                    var data = GetTamaFromField(x, y - 1);
                    if (data != null)
                    {
                        RemoveTamaFromField(x, y - 1); 
                        MoveTamaUp(data, false);
                        PutTamaToField(data);
                    }
                }
                // 一番下の段に新たなこだまを設定
                var tama = tamaGenerator.GenerateAttackTama(pattern[offset + x].Color);
                SetTamaPosition(tama, x, 0);
                PutTamaToField(tama);
            }
        }
        #endregion

        #region 落下処理
        private void StateDropAttackTamaEnter()
        {
            attackLineWaitFrame = setting.attackDropWaitFrame;
        }
        private void StateDropAttackTamaUpdate()
        {
            // こうげきだま適用
            if (attackLineWaitFrame > 0)
            {
                --attackLineWaitFrame;
            }
            else if (dropAttackBufferIndices.Count(x => x >= 0) > 0)
            {
                // まだこうげきだまがあれば適用
                ApplyDropAttackLine();
                attackLineWaitFrame = setting.attackDropWaitFrame;
            }
            // こうげきだま落下
            var drop = DropTamaInFIeld();
            // 落下するものがなくなれば次へ
            if (!drop && (dropAttackBufferIndices.Count(x => x >= 0) <= 0))
            {
                ChangeStateAfterDropping();
            }
        }
        private void StateDropAttackTamaLeave()
        {

        }
        /// <summary>
        /// 落下するこうげきだまを１段分適用する
        /// </summary>
        private void ApplyDropAttackLine()
        {
            for (int i = 0; i < setting.columnsCount; ++i)
            {
                int next = dropAttackBufferIndices[i];
                if (next < 0) continue;
                var color = dropAttackBuffer[next].color;
                dropAttackBufferIndices[i] = dropAttackBuffer[next].next;
                var tama = tamaGenerator.GenerateAttackTama(color);
                SetTamaPosition(tama, i, setting.rowsCount - 1);
                PutTamaToField(tama);
            }
        }
        #endregion
    }
}
