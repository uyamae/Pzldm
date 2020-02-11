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
            /// <summary>
            /// 次の状態を生成する
            /// </summary>
            public System.Func<T> NextState { get; set; }
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
        // 次の状態に遷移
        public void ChangeToNextState()
        {
            int index = System.Convert.ToInt32(CurrentState);
            if (states[index]?.NextState != null)
            {
                ChangeState(states[index].NextState());
            }
            else 
            {
                ++index;
                if (!System.Enum.IsDefined(typeof(T), index + 1))
                {
                    index = 0;
                }
                ChangeState((T)System.Enum.ToObject(typeof(T), index));
            }
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
