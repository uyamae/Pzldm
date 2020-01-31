using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public enum PlayingState
    {
        /// <summary>
        /// 初期化
        /// </summary>
        Initialize,
        /// <summary>
        /// 試合開始
        /// </summary>
        Ready,
        /// <summary>
        /// たま操作中
        /// </summary>
        ControlTama,
        /// <summary>
        /// たま設置中
        /// </summary>
        PuttingTama,
        /// <summary>
        /// たま落下中
        /// </summary>
        DroppingTama,
        /// <summary>
        /// たま消し中
        /// </summary>
        RemovingTama,
        /// <summary>
        /// こうげきだませり上げ
        /// </summary>
        LiftUpAttackTama,
        /// <summary>
        /// こうげきだま落下
        /// </summary>
        DropAttackTama,
        /// <summary>
        /// 勝敗チェック
        /// </summary>
        CheckGameOver,
        /// <summary>
        /// 勝敗演出
        /// </summary>
        GameOver,
        /// <summary>
        /// 続行確認
        /// </summary>
        AskContinue,
    }
    public partial class PlayField
    {
        /// <summary>
        /// 状態遷移をBattleManager が管理するかどうか
        /// </summary>
        public bool IsStateManaged { get; set; }
        //class PlayState
        //{
        //    public System.Action Enter { get; set; }
        //    public System.Action Update { get; set; }
        //    public System.Action Leave { get; set; }
        //}
        //private PlayState[] states;
        //private PlayingState currentState;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public PlayingState CurrentState { get { return stateMachine.CurrentState; } }
        /// <summary>
        /// ステートマシン
        /// </summary>
        private StateMachine<PlayingState> stateMachine;
        /// <summary>
        /// ステート処理初期化
        /// </summary>
        private void InitPlayingState()
        {
            var states = new StateMachine<PlayingState>.State[]
            {
                /// 初期化
                new StateMachine<PlayingState>.State() { Update = StateInitUpdate },
                // 試合開始
                new StateMachine<PlayingState>.State() {
                    Enter = StateReadyEnter,
                    Update = StateReadyUpdate,
                    Leave = StateReadyLeave,
                },
                // たま操作中
                new StateMachine<PlayingState>.State() {
                    Enter = StateControlTamaEnter,
                    Update = StateControlTamaUpdate,
                    Leave = StateControlTamaLeave,
                },
                // たま設置中
                new StateMachine<PlayingState>.State() {
                    Enter = StatePuttingTamaEnter,
                    Update = StatePuttingTamaUpdate,
                    Leave = StatePuttingTamaLeave,
                },
                // たま落下中
                new StateMachine<PlayingState>.State() {
                    Enter = StateDroppingTamaEnter,
                    Update = StateDroppingTamaUpdate,
                    Leave = StateDroppingTamaLeave,
                },
                // たま消し中
                new StateMachine<PlayingState>.State() {
                    Enter = StateRemovingTamaEnter,
                    Update = StateRemovingTamaUpdate,
                    Leave = StateRemovingTamaLeave,
                },
                // こうげきだませり上げ
                new StateMachine<PlayingState>.State() {
                    Enter = StateLiftUpAttackTamaEnter,
                    Update = StateLiftUpAttackTamaUpdate,
                    Leave = StateLiftUpAttackTamaLeave,
                },
                // こうげきだま落下
                new StateMachine<PlayingState>.State() {
                    Enter = StateDropAttackTamaEnter,
                    Update = StateDropAttackTamaUpdate,
                    Leave = StateDropAttackTamaLeave,
                },
                // 勝敗チェック
                new StateMachine<PlayingState>.State() { },
                // 勝敗演出
                new StateMachine<PlayingState>.State() {
                    Enter = StateGameOverEnter,
                    Update = StateGameOverUpdate,
                    Leave = StateGameOverLeave,
                },
                // 続行確認
                new StateMachine<PlayingState>.State() {
                    //Enter = StateAskContinueEnter,
                    //Update = StateAskContinueUpdate,
                    //Leave = StateAskContinueLeave,
                },
            };
            stateMachine = new StateMachine<PlayingState>(states);

            stateMachine.StartState(PlayingState.Initialize);
        }
        /// <summary>
        /// ステート処理
        /// </summary>
        private void ProcessState()
        {
            stateMachine.UpdateState();
        }
        /// <summary>
        /// ステート開始、初回だけ
        /// </summary>
        /// <param name="state"></param>
        private void StartState(PlayingState state)
        {
            stateMachine.StartState(state);
            StateName = state.ToString();
        }
        /// <summary>
        /// ステート変更
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(PlayingState state)
        {
            stateMachine.ChangeState(state);
            StateName = state.ToString();
        }
        /// <summary>
        /// 初期化処理更新
        /// </summary>
        private void StateInitUpdate()
        {
            ChangeState(PlayingState.Ready);
        }
    }
}
