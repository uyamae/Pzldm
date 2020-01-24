using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public class StateMachine<T> where T : System.Enum
    {
        /// <summary>
        /// 状態
        /// </summary>
        public class State
        {
            /// <summary>
            /// 状態開始時に呼び出される処理
            /// </summary>
            public System.Action Enter { get; set; }
            /// <summary>
            /// 状態を更新する処理
            /// </summary>
            public System.Action Update { get; set; }
            /// <summary>
            /// 状態終了時に呼び出される処理
            /// </summary>
            public System.Action Leave { get; set; }
        }
        private State[] states;
        /// <summary>
        /// 現在の状態
        /// </summary>
        public T CurrentState { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="states"></param>
        public StateMachine(State[] states)
        {
            this.states = states;
        }
        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="state"></param>
        public void StartState(T state)
        {
            CurrentState = state;
            states[System.Convert.ToInt32(state)].Enter?.Invoke();
        }
        /// <summary>
        /// 状態変更
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(T state)
        {
            states[System.Convert.ToInt32(CurrentState)].Leave?.Invoke();
            CurrentState = state;
            states[System.Convert.ToInt32(state)].Enter?.Invoke();
        }
        /// <summary>
        /// 状態更新
        /// </summary>
        public void UpdateState()
        {
            states[System.Convert.ToInt32(CurrentState)].Update?.Invoke();
        }
    }
}
