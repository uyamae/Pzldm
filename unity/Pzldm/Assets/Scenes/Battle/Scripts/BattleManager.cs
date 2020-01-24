using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public enum BattleState
    {
        Init,
        Ready,
        Playing,
        GameOver,
    }
    /// <summary>
    /// 対戦を管理するクラス
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        public PlayField[] playFields;

        // Start is called before the first frame update

        StateMachine<BattleState> stateMachine;

        public BattleState currentState;

        void Start()
        {
            var states = new StateMachine<BattleState>.State[] {
                new StateMachine<BattleState>.State() {
                    Update = UpdateInit,
                },
                new StateMachine<BattleState>.State() {
                    Update = UpdateReady,
                },
                new StateMachine<BattleState>.State() {
                    Update = UpdatePlaying,
                },
                new StateMachine<BattleState>.State() {
                    Update = UpdateGameOver,
                },
            };
            stateMachine = new StateMachine<BattleState>(states);
            stateMachine.StartState(BattleState.Init);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            stateMachine.UpdateState();
            currentState = stateMachine.CurrentState;
        }
        private void UpdateInit()
        {
            foreach (var p in playFields)
            {
                if (p == null) continue;
                p.IsStateManaged = true;
            }
            stateMachine.ChangeState(BattleState.Ready);
        }
        private void UpdateReady()
        {
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
        private void UpdatePlaying()
        {
            // ゲームオーバー同期
            for (int i = 0; i < playFields.Length; ++i)
            {
                if (!playFields[i].gameObject.activeInHierarchy) continue;
                if (playFields[i].CurrentState == PlayingState.GameOver)
                {
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
        private void UpdateGameOver()
        {
            foreach (var p in playFields)
            {
                if (p == null) return;
                if (!p.gameObject.activeInHierarchy) return;
                if (p.CurrentState != PlayingState.AskContinue) return;
            }
            // ここにたどり着いたら全部がAskContinue
            foreach (var p in playFields)
            {
                if (p != null && p.gameObject.activeInHierarchy)
                {
                    p.ChangeState(PlayingState.Ready);
                }
            }
            stateMachine.ChangeState(BattleState.Ready);
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
    }
}
