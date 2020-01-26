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
        [SerializeField]
        private PlayField[] playFields;

        [SerializeField]
        private NumberDisplay[] attackCount;

        // Start is called before the first frame update

        StateMachine<BattleState> stateMachine;

        [SerializeField]
        private BattleState currentState;

        private ComPlayer comPlayer;

        [SerializeField]
        private bool useComPlayer;
        /// <summary>
        /// COM プレイヤー
        /// </summary>
        public bool UseComPlayer
        {
            get { return useComPlayer;}
            set
            {
                useComPlayer = value;
            }
        }

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

            comPlayer = new DefaultComPlayer()
            {
                Self = playFields[0],
                Opponent = playFields[1],
            };
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

            playFields[1].ComPlayer = UseComPlayer ? comPlayer : null;
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
            // こうげきだま更新
            UpdateAttackCount();
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
        private void UpdateAttackCount()
        {
            UpdateAttackCount(playFields[0], playFields[1], attackCount[0]);            
            UpdateAttackCount(playFields[1], playFields[0], attackCount[1]);            
        }
        /// <summary>
        /// こうげきだま数更新
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
