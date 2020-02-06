using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public partial class PlayField
    {
        /// <summary>
        /// 連鎖数表示
        /// </summary>
        [SerializeField]
        private ChainCount chainCounter;
        /// <summary>
        /// 連鎖数
        /// </summary>
        private int chainCount;

        /// <summary>
        /// 設置されたたまの連結確認ワーク領域
        /// </summary>
        private int[,] tamaFieldCheck;
        /// <summary>
        /// 設置されたたまグループの連結数
        /// </summary>
        private int[] tamaGroupLinkCount;
        /// <summary>
        /// たまが消える接続数
        /// </summary>
        private int TamaRemoveLinkCount { get { return 3; } }
        /// <summary>
        /// たまを設置した後の待ち時間
        /// </summary>
        private int tamaPutWait;
        /// <summary>
        /// 設置ステートの開始処理
        /// </summary>
        private void StatePuttingTamaEnter()
        {
            // 設置ウェイト
            tamaPutWait = setting.tamaPutWait;
        }
        /// <summary>
        /// 設置ステートの更新処理
        /// </summary>
        private void StatePuttingTamaUpdate()
        {
            // 設置ウェイト
            --tamaPutWait;
            if (tamaPutWait <= 0)
            {
                //PutCurrentTamaToField();
                //ChangeState(PlayingState.DroppingTama);
                ChangeStateAfterPutting();
            }
        }
        /// <summary>
        /// 設置ステートの終了処理
        /// </summary>
        private void StatePuttingTamaLeave()
        {

        }
        /// <summary>
        /// こうげきだま同期待ち更新
        /// </summary>
        private void StateSyncAttackUpdate()
        {
            // 同一フレームに連鎖が終了したとき
            // 2P 側連鎖終了処理時には1P はつぎのたま操作に
            // 遷移してしまうので１フレーム待つステート

            // あいてのこうげきだまがあるならそちら
            if (RecievedAttackCount > 0)
            {
                ChangeStateToAttackTama();
            }
            // 何もなければつぎのたま操作
            else
            {
                ChangeStateAfterDropping();
            }
        }
        /// <summary>
        /// たま落下中の開始処理
        /// </summary>
        private void StateDroppingTamaEnter()
        {
            // 操作中のたまがあればフィールドに移す
            PutCurrentTamaToField();
            // 設置ウェイト
            //tamaPutWait = setting.tamaPutWait;
            tamaPutWait = 0;
        }
        /// <summary>
        /// たま落下中の更新処理
        /// </summary>
        private void StateDroppingTamaUpdate()
        {
            if (tamaPutWait > 0)
            {
                --tamaPutWait;
                return;
            }
            // フィールドのたま落下処理
            if (DropTamaInFIeld())
            {
                return;
            }
            // 設置処理
            ChangeState(PlayingState.PuttingTama);
        }
        private void ChangeStateAfterPutting()
        {
            // たま消去チェック
            if (CheckTamaRemoving())
            {
                // 連鎖数カウントアップ
                ++chainCount;
                chainCounter?.StartDisplay(1, chainCount);

                ChangeState(PlayingState.RemovingTama);
            }
            else
            {
                // こうげきだまが発生していたら送る
                if (SendAttackCount > 0)
                {
                    BattleManager?.SendAttackTama(this);
                }
                // 同一フレームの相手のこうげきだま発生を待つ
                ChangeState(PlayingState.SyncAttackTama);
            }
        }
        /// <summary>
        /// たま自由落下後の遷移
        /// </summary>
        private void ChangeStateAfterDropping()
        {
            CheckGameOver();
            if (IsGameOver)
            {
                ChangeState(PlayingState.GameOver);
            }
            else
            {
                ChangeState(PlayingState.ControlTama);
            }
        }
        /// <summary>
        /// たま落下中の終了処理
        /// </summary>
        private void StateDroppingTamaLeave()
        {

        }
        /// <summary>
        /// 設置チェック
        /// </summary>
        private void PutCurrentTamaToField()
        {
            for (int n = 0; n < currentTamaPair.Length; ++n)
            {
                if (currentTamaPair[n] != null)
                {
                    PutTamaToField(currentTamaPair[n]);
                    currentTamaPair[n] = null;
                }
            }
        }
        /// <summary>
        /// フィールドのたまを落下させる
        /// </summary>
        private bool DropTamaInFIeld()
        {
            bool falling = false;
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    //TamaData data = tamaField[y, x];
                    TamaData data = GetTamaFromField(x, y);
                    if ((data != null) && (CheckMovingVertical(data, -1)))
                    {
                        RemoveTamaFromField(x, y);
                        MoveTamaDown(data);
                        PutTamaToField(data);
                        falling = true;
                    }
                }
            }
            return falling;
        }
        /// <summary>
        /// 設置されたたまが消えるかどうかのチェック
        /// </summary>
        /// <returns></returns>
        private bool CheckTamaRemoving()
        {
            ClearTamaFieldCheck();
            int groups = CheckTamaFieldLink();
            return CountTamaFieldLink(groups);
        }
        /// <summary>
        /// ワーク領域をクリア
        /// </summary>
        private void ClearTamaFieldCheck()
        {
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    tamaFieldCheck[y, x] = -1;
                }
            }
        }
        /// <summary>
        /// 連続している個所を探す
        /// </summary>
        /// <returns>連結グループ番号最大値</returns>
        private int CheckTamaFieldLink()
        {
            int group = 1;
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    // フィールドにたまがなければ終了
                    var data = GetTamaFromField(x, y);
                    if (data == null)
                    {
                        tamaFieldCheck[y, x] = 0;
                        break;
                    }
                    // チェック済みはスキップ
                    if (tamaFieldCheck[y, x] >= 0) continue;
                    // こだまの場合はリンクされない
                    if (data.State == TamaStateType.Small)
                    {
                        // リンク無しにチェックして次へ
                        tamaFieldCheck[y, x] = 0;
                        continue;
                    }
                    // チェックする
                    if (CheckTamaFieldLink(data, group))
                    {
                        ++group;
                    }
                }
            }
            return group - 1;
        }
        /// <summary>
        /// 接続をチェック
        /// </summary>
        /// <param name="data"></param>
        /// <param name="group"></param>
        /// <param name="found"></param>
        /// <returns>連結されたものある場合true</returns>
        private bool CheckTamaFieldLink(TamaData data, int group, bool found = false)
        {
            // 隣
            (int x, int y, bool b)[] nexts = {
                ( data.X - 1, data.Y, data.X > 0 ),
                ( data.X + 1, data.Y, data.X < setting.columnsCount - 1 ),
                ( data.X, data.Y - 1, data.Y > 0 ),
                ( data.X, data.Y + 1, data.Y < setting.rowsCount - 1 ),
            };
            // 隣を順に調べる
            foreach (var n in nexts)
            {
                // 隣がないかチェック済みならスキップ
                if (!n.b || (tamaFieldCheck[n.y, n.x] >= 0)) continue;
                // 隣取得
                var next = GetTamaFromField(n.x, n.y);
                if ((next?.State == TamaStateType.Large) && (next?.Color == data.Color))
                {
                    // 同じ色のおおだまの場合はリンクする
                    if (!found)
                    {
                        found = true;
                        tamaFieldCheck[data.Y, data.X] = group;
                    }
                    tamaFieldCheck[n.y, n.x] = group;
                    // 再帰的に調べる
                    CheckTamaFieldLink(next, group, true);
                }
            }
            // リンク見つからず
            if (!found)
            {
                tamaFieldCheck[data.Y, data.X] = 0;
            }
            return found;
        }
        /// <summary>
        /// 連続個所の個数を数える
        /// </summary>
        /// <returns></returns>
        private bool CountTamaFieldLink(int groupCount)
        {
            bool remove = false;
            // カウンタクリア
            for (int i = 0; i <= groupCount; ++i)
            {
                tamaGroupLinkCount[i] = 0;
            }
            // カウントする
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    int group = tamaFieldCheck[y, x];
                    if (group < 0) break;
                    else if (group == 0) continue;

                    ++tamaGroupLinkCount[group];
                    if (!remove && tamaGroupLinkCount[group] >= TamaRemoveLinkCount)
                    {
                        remove = true;
                    }
                }
            }
            // 消す準備
            int removedCount = 0;
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    int group = tamaFieldCheck[y, x];
                    if (group < 0) break;
                    if (tamaGroupLinkCount[group] >= TamaRemoveLinkCount)
                    {
                        // 消える奴は消去エフェクトを表示しておく
                        var data = GetTamaFromField(x, y);
                        data.Spark.enabled = true;
                        data.Spark.sprite = tamaGenerator.GetTamaSparkSprite(0);
                        ++removedCount;
                    }
                }
            }
            // 消すなら
            if (removedCount > 0)
            {
                // 連鎖数を増やす
                ++ChainCount;
                // こうげきだまをカウントする
                if (ChainCount > 1)
                {
                    SendAttackCount += 6;
                }
                // 同時消しなら+3
                if (removedCount >= 6)
                {
                    SendAttackCount += 3;
                    // さらに３つごとに+1
                    removedCount -= 6;
                    SendAttackCount += (removedCount / 3);
                }
            }


            SetTamaFieldCheckDebugText();
            // 消えるかどうか
            return remove;
        }

        private void SetTamaFieldCheckDebugText()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine($"tamaFieldCheck[{setting.rowsCount},{setting.columnsCount}]");
            for (int i = setting.rowsCount - 1; i >= 0; --i)
            {
                for (int j = 0; j < setting.columnsCount; ++j)
                {
                    if (tamaFieldCheck[i, j] < 0)
                    {
                        builder.Append("  ");
                    }
                    else
                    {
                        builder.Append($"{tamaFieldCheck[i, j]} ");
                    }
                }
                builder.AppendLine();
            }
            builder.AppendLine("group counts");
            for (int i = 1; i < tamaGroupLinkCount.Length; ++i)
            {
                if (tamaGroupLinkCount[i] > 0)
                {
                    builder.AppendLine($"{i}:{tamaGroupLinkCount[i]}");
                }
            }

            SetDebugText(builder.ToString());
        }
    }
}
