using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public partial class PlayField
    {
        private int readyWaitFrames;
        /// <summary>
        /// 開始できるかどうかフラグ(IsStateManaged 時)
        /// </summary>
        public bool IsReadyToStart { get; set; }
        /// <summary>
        /// Ready 開始
        /// </summary>
        private void StateReadyEnter()
        {
            // たま生成乱数リセット
            tamaGenerator.ResetTamaRandom();
            // つぎのたま生成
            GenerateNextTamaPair();
            // たま自由落下フレームをリセット
            TamaFallFrame = setting.tamaFallFrame;
            // 待ち時間
            readyWaitFrames = (int)(setting.readyWaitSeconds * 50);
            // こうげきだまの数リセット
            SendAttackCount = 0;
            RecievedAttackCount = 0;
            InitAttackTamaParameters();
            // 連鎖数リセット
            ChainCount = 0;
            // ゲームオーバーフラグリセット
            IsGameOver = false;
            // 開始できるかどうか
            IsReadyToStart = false;
            // プレイのフレームカウントをリセット
            PlayFrameCount = 0;
        }
        /// <summary>
        /// Ready 更新
        /// </summary>
        private void StateReadyUpdate()
        {
            // 準備同期待ち
            if (IsStateManaged && !IsReadyToStart)
            {
                return;
            }
            //// 演出待ち
            //if (readyWaitFrames > 0)
            //{
            //    --readyWaitFrames;
            //    return;
            //}
            // 操作開始
            ChangeState(PlayingState.ControlTama);
        }
        /// <summary>
        /// Ready 終了
        /// </summary>
        private void StateReadyLeave()
        {
        }
    }
}
