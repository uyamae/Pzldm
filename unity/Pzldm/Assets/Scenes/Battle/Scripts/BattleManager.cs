﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Pzldm
{
    /// <summary>
    /// 試合の状態
    /// </summary>
    public enum BattleState
    {
        /// <summary>
        /// 初期化
        /// </summary>
        Init,
        /// <summary>
        /// 試合開始
        /// </summary>
        Ready,
        /// <summary>
        /// 試合中
        /// </summary>
        Playing,
        /// <summary>
        /// 試合終了
        /// </summary>
        GameOver,
        /// <summary>
        /// 続行確認
        /// </summary>
        AskContinue,
    }
    /// <summary>
    /// 対戦を管理するクラス
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// 各プレイヤーのフィールド
        /// </summary>
        [SerializeField]
        private PlayField[] playFields;

        /// <summary>
        /// こうげきだま表示
        /// </summary>
        [SerializeField]
        private NumberDisplay[] attackCounts;

        /// <summary>
        /// 状態遷移管理
        /// </summary>
        StateMachine<BattleState> stateMachine;

        /// <summary>
        /// COM
        /// </summary>
        private ComPlayer comPlayer;

        /// <summary>
        /// 2P 側にCOM を使用するかどうか
        /// </summary>
        [SerializeField]
        private bool useComPlayer;

        /// <summary>
        /// ゲームオーバー時の表示（CONTINUE/EXIT）
        /// </summary>
        [SerializeField]
        private GameOverScreen gameOverScreen;

        /// <summary>
        /// デバッグ表示グループ
        /// </summary>
        [SerializeField]
        private GameObject debugGroup;

        /// <summary>
        /// COM プレイヤーを使用するかどうか
        /// </summary>
        public bool UseComPlayer
        {
            get { return useComPlayer;}
            set
            {
                useComPlayer = value;
            }
        }
        /// <summary>
        /// 更新開始時
        /// </summary>
        void Start()
        {
            /// 状態遷移初期化
            var states = new StateMachine<BattleState>.State[] {
                new StateMachine<BattleState>.State() {
                    Update = UpdateInit,
                },
                new StateMachine<BattleState>.State() {
                    Enter = EnterReady,
                    Update = UpdateReady,
                },
                new StateMachine<BattleState>.State() {
                    Update = UpdatePlaying,
                },
                new StateMachine<BattleState>.State() {
                    Update = UpdateGameOver,
                },
                new StateMachine<BattleState>.State()
                {
                    Enter = EnterAskContinue,
                    Update = UpdateAskContinue,
                    Leave = LeaveAskContinue,
                }
            };
            stateMachine = new StateMachine<BattleState>(states);
            stateMachine.StartState(BattleState.Init);
            // COM 作成
            comPlayer = new DefaultComPlayer()
            {
                Self = playFields[0],
                Opponent = playFields[1],
            };
        }
        /// <summary>
        /// 毎フレームの更新
        /// </summary>
        // Update is called once per frame
        void FixedUpdate()
        {
            stateMachine.UpdateState();
        }
        /// <summary>
        /// 初期化状態更新
        /// </summary>
        private void UpdateInit()
        {
            // 設定反映
            var mgr = GameObject.Find("PzldmManager")?.GetComponent<PzldmManager>();
            for (int i = 0; i < playFields.Length; ++i)
            {
                var p = playFields[i];
                if (p == null) continue;
                p.IsStateManaged = true;
                int oppo = (i + 1) % playFields.Length;
                var a = mgr?.GetPlayerAttackPattern(oppo);
                if (a != null)
                {
                    playFields[i].OpponentAttackPattern = a;
                }
            }
            if (mgr?.PlayingMode == PlayingModeType.SinglePlay)
            {
                playFields[1].ComPlayer = comPlayer;
            }
            // 強制的にCOM 使用
            if (UseComPlayer)
            {
                playFields[1].ComPlayer = comPlayer;
            }

            stateMachine.ChangeState(BattleState.Ready);
        }
        /// <summary>
        /// 準備状態開始
        /// </summary>
        private void EnterReady()
        {
            // こうげきだま表示をリセット
            foreach (var n in attackCounts)
            {
                n.Number = 0;
            }
        }
        /// <summary>
        /// 試合開始同期
        /// </summary>
        private void UpdateReady()
        {
            // 準備完了待ち
            foreach (var p in playFields)
            {
                if (p == null) return;
                if (!p.gameObject.activeInHierarchy) return;
                if (p.CurrentState != PlayingState.Ready) return;
            }
            // ここにたどり着いたら全部がReady
            foreach (var p in playFields)
            {
                if (p != null && p.gameObject.activeInHierarchy)
                {
                    p.BattleManager = this;
                    p.IsReadyToStart = true;
                }
            }
            stateMachine.ChangeState(BattleState.Playing);
        }
        /// <summary>
        /// 試合中更新
        /// </summary>
        private void UpdatePlaying()
        {
            // こうげきだま更新
            UpdateAttackCount();
            // ゲームオーバー同期
            for (int i = 0; i < playFields.Length; ++i)
            {
                if (!playFields[i].gameObject.activeInHierarchy) continue;
                if (playFields[i].CurrentState == PlayingState.GameOver)
                {
                    // 全員ゲームオーバーにして遷移
                    for (int j = 0; j < playFields.Length; ++j)
                    {
                        if (i == j) continue;
                        if (!playFields[j].gameObject.activeInHierarchy) continue;
                        playFields[j].ChangeState(PlayingState.GameOver);
                    }
                    stateMachine.ChangeState(BattleState.GameOver);
                    break;
                }
            }
        }
        /// <summary>
        /// ゲームオーバー演出同期
        /// </summary>
        private void UpdateGameOver()
        {
            // ゲームオーバー演出待ち
            foreach (var p in playFields)
            {
                if (p == null) return;
                if (!p.gameObject.activeInHierarchy) return;
                if (p.CurrentState != PlayingState.AskContinue) return;
            }
            // ここにたどり着いたら全部がAskContinue
            stateMachine.ChangeState(BattleState.AskContinue);
        }
        /// <summary>
        /// コンティニュー確認開始
        /// </summary>
        private void EnterAskContinue()
        {
            gameOverScreen.StartAskContinue();
        }
        /// <summary>
        /// コンティニュー確認更新
        /// </summary>
        private void UpdateAskContinue()
        {
            if (gameOverScreen.Result == GameOverScreen.ResultType.Continue)
            {
                // 続行、全体をReady に
                foreach (var p in playFields)
                {
                    if (p != null && p.gameObject.activeInHierarchy)
                    {
                        p.ChangeState(PlayingState.Ready);
                    }
                }
                stateMachine.ChangeState(BattleState.Ready);
            }
            else if (gameOverScreen.Result == GameOverScreen.ResultType.Exit)
            {
                // 終了、シーン遷移
                SceneManager.LoadScene("select");
            }
        }
        /// <summary>
        /// コンティニュー確認終了
        /// </summary>
        private void LeaveAskContinue()
        {
            gameOverScreen.IsActive = false;
        }
        /// <summary>
        /// 確定したこうげきだまを送る
        /// </summary>
        /// <param name="field"></param>
        public void SendAttackTama(PlayField field)
        {
            PlayField opponent = null;
            if (field == playFields[0])
            {
                opponent = playFields[1];
            }
            else
            {
                opponent = playFields[0];
            }
            opponent.RecievedAttackCount += field.SendAttackCount;
            field.SendAttackCount = 0;
        }
        /// <summary>
        /// こうげきだま数更新
        /// </summary>
        private void UpdateAttackCount()
        {
            UpdateAttackCount(playFields[0], playFields[1], attackCounts[0]);            
            UpdateAttackCount(playFields[1], playFields[0], attackCounts[1]);            
        }
        /// <summary>
        /// 各プレイヤーのこうげきだま数更新
        /// </summary>
        /// <param name="p0">自分</param>
        /// <param name="p1">相手</param>
        /// <param name="n">自分の表示</param>
        private void UpdateAttackCount(PlayField p0, PlayField p1, NumberDisplay n)
        {
            if (n == null || p0 == null) return;
            int count = p0.SendAttackCount;
            if (p1 != null)
            {
                count += p1.RecievedAttackCount;
            }
            if (n.Number < count)
            {
                ++n.Number;
            }
            else if (n.Number > count)
            {
                --n.Number;
            }
        }
    }
}
