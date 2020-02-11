using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    /// <summary>
    /// 進化したCOM
    /// </summary>
    public class AdvancedComPlayer : ComPlayer
    {
        /// <summary>
        /// 思考状態
        /// </summary>
        enum StateType
        {
            WaitControl,
            DecideColumn,
            ControlTama,
            DropTama,
        }
        private StateMachine<StateType> stateMachine;
        uint bits;
        /// <summary>
        /// デバッグ情報
        /// </summary>
        public ComInfoBehaviour ComInfo { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvancedComPlayer()
        {
            StateMachine<StateType>.State[] states = new StateMachine<StateType>.State[]
            {
                new StateMachine<StateType>.State()
                {
                    Update = UpdateWaitControl,
                },
                new StateMachine<StateType>.State()
                {
                    Enter = EnterDecideColumn,
                    Update = UpdateDecideColumn,
                },
                new StateMachine<StateType>.State()
                {
                    Update = UpdateControlTama,
                },
                new StateMachine<StateType>.State()
                {
                    Update = UpdateDropTama,
                },
            };
            stateMachine = new StateMachine<StateType>(states);
            stateMachine.StartState(StateType.WaitControl);
            destColumns = new ColumnState[2];
        }
        /// <summary>
        /// 操作開始時
        /// </summary>
        public override void Start()
        {
            stateMachine.StartState(StateType.WaitControl);
        }
        /// <summary>
        /// 操作の入力ビット生成
        /// </summary>
        /// <returns></returns>
        public override uint GenerateBits()
        {
            bits = 0;
            // 思考処理
            stateMachine.UpdateState();
            // デバッグ情報反映
            if (ComInfo != null)
            {
                ComInfo.State = stateMachine.CurrentState.ToString();
                ComInfo.Main = $"Main:{(destColumns == null ? 0 : destColumns[0].Column)}";
                ComInfo.Sub = $"Sub:{(destColumns == null ? 0 : destColumns[1].Column)}";
            }

            return bits;
        }
        /// <summary>
        /// 操作状態になるまで待つ
        /// </summary>
        private void UpdateWaitControl()
        {
            if (Self?.CurrentState == PlayingState.ControlTama)
            {
                stateMachine.ChangeState(StateType.DecideColumn);
            }
        }
        /// <summary>
        /// 列の状態
        /// </summary>
        struct ColumnState
        {
            public ColorType Color { get; set; }
            public int Top { get; set; }
            public int Column { get; set; }
        }
        private ColumnState[] destColumns;
        /// <summary>
        /// 各列の情報
        /// </summary>
        private ColumnState[] columnStates;

        private void EnterDecideColumn()
        {
            if (columnStates == null)
            {
                columnStates = new ColumnState[Self.ColumnsCount];
            }
            for (int col = 0; col < Self.TamaField.GetLength(1); ++col)
            {
                ColorType color = ColorType.Invalid;
                columnStates[col].Top = 0;
                for (int row = 0; row < Self.TamaField.GetLength(0); ++row)
                {
                    var tama = Self.GetTamaFromField(col, row);
                    if (tama == null)
                    {
                        break;
                    }
                    else
                    {
                        columnStates[col].Top++;
                        color = tama.Color;
                    }
                }
                columnStates[col].Color = color;
            }
        }
        /// <summary>
        /// 列を決める
        /// </summary>
        private void UpdateDecideColumn()
        {
            // それぞれを置く候補
            for (int i = 0; i < destColumns.Length; ++i)
            {
                destColumns[i].Color = ColorType.Invalid;
                destColumns[i].Top = Self.TamaField.GetLength(0);
                destColumns[i].Column = -1;
            }
            for (int i = 0; i < columnStates.Length; ++i)
            {
                for (int j = 0; j < destColumns.Length; ++j)
                {
                    if (columnStates[i].Color == Self.CurrentTamaPair[j].Color)
                    {
                        if ((columnStates[i].Top < destColumns[j].Top) || (destColumns[j].Color == ColorType.Invalid))
                        {
                            destColumns[j].Top = columnStates[i].Top;
                            destColumns[j].Color = Self.CurrentTamaPair[j].Color;
                            destColumns[j].Column = i;
                        }
                    }
                    else if ((destColumns[j].Color == ColorType.Invalid) && (columnStates[i].Top < destColumns[j].Top))
                    {
                        destColumns[j].Top = columnStates[i].Top;
                        destColumns[j].Column = i;
                    }
                }
            }
            stateMachine.ChangeState(StateType.ControlTama);
        }
        private uint BitOfInput(KeyIndex key)
        {
            return 1u << (int)key;
        }
        /// <summary>
        /// たま操作
        /// </summary>
        private void UpdateControlTama()
        {
            if (destColumns[0].Column >= 0)
            {
                if (Self.CurrentTamaPair == null || Self.CurrentTamaPair[0] == null)
                {

                }
                else if (destColumns[0].Column < Self.CurrentTamaPair[0].X)
                {
                    // 左へ
                    bits |= BitOfInput(KeyIndex.Left);
                }
                else if (destColumns[0].Column > Self.CurrentTamaPair[0].X)
                {
                    // 右へ
                    bits |= BitOfInput(KeyIndex.Right);
                }
                else
                {
                    stateMachine.ChangeState(StateType.DropTama);
                }
            }
            else if (destColumns[1].Column >= 0)
            {
                if (Self.CurrentTamaPair == null || Self.CurrentTamaPair[1] == null)
                {

                }
                else if (destColumns[1].Column < Self.CurrentTamaPair[1].X)
                {
                    // 左へ
                    bits |= BitOfInput(KeyIndex.Left);
                }
                else if (destColumns[1].Column > Self.CurrentTamaPair[1].X)
                {
                    // 右へ
                    bits |= BitOfInput(KeyIndex.Right);
                }
                else
                {
                    stateMachine.ChangeState(StateType.DropTama);
                }
            }

            if (destColumns[0].Column < destColumns[1].Column)
            {

            }
            else if (destColumns[0].Column > destColumns[1].Column)
            {

            }
            // 操作中でなければ操作待ちに遷移
            if (Self.CurrentState != PlayingState.ControlTama)
            {
                stateMachine.ChangeState(StateType.WaitControl);
            }
        }
        private void UpdateDropTama()
        {
            bits |= BitOfInput(KeyIndex.Down);
            // 操作中でなければ操作待ちに遷移
            if (Self.CurrentState != PlayingState.ControlTama)
            {
                stateMachine.ChangeState(StateType.WaitControl);
            }
        }
    }
}
