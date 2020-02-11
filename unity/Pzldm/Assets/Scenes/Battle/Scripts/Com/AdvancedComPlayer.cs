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
            /// <summary>
            /// 操作状態になるのを待つ
            /// </summary>
            WaitControl,
            /// <summary>
            /// 列を決定する
            /// </summary>
            DecideColumn,
            /// <summary>
            /// メインのたまを基準に横移動
            /// </summary>
            ControlTamaForMain,
            /// <summary>
            /// サブのたまを基準に横移動
            /// </summary>
            ControlTamaForSub,
            /// <summary>
            /// たまを回転
            /// </summary>
            RotateTama,
            /// <summary>
            /// たまを落下させる
            /// </summary>
            DropTama,
        }
        private StateMachine<StateType> stateMachine;
        uint bits;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvancedComPlayer()
        {
            StateMachine<StateType>.State[] states = new StateMachine<StateType>.State[]
            {
                // 操作待ち
                new StateMachine<StateType>.State()
                {
                    Update = UpdateWaitControl,
                },
                // 決定
                new StateMachine<StateType>.State()
                {
                    Enter = EnterDecideColumn,
                    Update = UpdateDecideColumn,
                },
                // 操作
                new StateMachine<StateType>.State()
                {
                    Enter = EnterControlTama,
                    Update = UpdateControlTamaForMain,
                    NextState = () => StateType.DropTama,
                },
                // 操作
                new StateMachine<StateType>.State()
                {
                    Enter = EnterControlTama,
                    Update = UpdateControlTamaForSub,
                    NextState = () => StateType.DropTama,
                },
                // 回転
                new StateMachine<StateType>.State()
                {
                    Enter = EnterRotateTama,
                    Update = UpdateRotateTama,
                },
                // 落下
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
                ComInfo.SubPos = subTamaPos.ToString();
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
        /// <summary>
        /// 各たまをどの列に配置したいかの情報
        /// </summary>
        private ColumnState[] destColumns;
        /// <summary>
        /// 各列の情報
        /// </summary>
        private ColumnState[] columnStates;
        /// <summary>
        /// サブのたまの位置
        /// </summary>
        enum SubTamaPosType
        {
            North, East, South, West,
        }
        private SubTamaPosType subTamaPos;

        private void EnterDecideColumn()
        {
            UpdateColumnStates();
            ResetDestColumns();
        }
        /// <summary>
        /// 各列の情報を更新
        /// </summary>
        private void UpdateColumnStates()
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
            DecideDestColumnsByColor();
            // サブのたまの位置を決定
            DecideSubTamaPos();
            // 次の操作に遷移
            stateMachine.ChangeState(StateType.RotateTama);
        }
        /// <summary>
        /// 状況に応じたたま操作に遷移
        /// </summary>
        private void ChangeStateToControlTama()
        {
            // メインのたま行き先が色なし & サブはあり
            if ((destColumns[0].Color == ColorType.Invalid) &&
                (destColumns[1].Color != ColorType.Invalid))
            {
                // サブ基準での操作に遷移する
                stateMachine.ChangeState(StateType.ControlTamaForSub);
            }
            else
            {
                // メイン基準での操作に遷移する
                stateMachine.ChangeState(StateType.ControlTamaForMain);
            }
        }
        /// <summary>
        /// たまを置く候補列をリセット
        /// </summary>
        private void ResetDestColumns()
        {
            subTamaPos = SubTamaPosType.North;
            for (int i = 0; i < destColumns.Length; ++i)
            {
                destColumns[i].Color = ColorType.Invalid;
                destColumns[i].Top = Self.TamaField.GetLength(0);
                destColumns[i].Column = -1;
            }
        }
        /// <summary>
        /// 色を基準に置く列を決める
        /// </summary>
        private void DecideDestColumnsByColor()
        {
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
        }
        /// <summary>
        /// たまの回転を決める
        /// </summary>
        private void DecideSubTamaPos()
        {
            // どちらも行き先がなくて色が違う
            if ((destColumns[0].Color == ColorType.Invalid) &&
                (destColumns[1].Color == ColorType.Invalid) &&
                (Self.CurrentTamaPair[0].Color  != Self.CurrentTamaPair[1].Color))
            {
                // 
                subTamaPos = SubTamaPosType.East;
            }
            else if (destColumns[0].Column < destColumns[1].Column)
            {
                subTamaPos = SubTamaPosType.East;
            }
            else if (destColumns[0].Column > destColumns[1].Column)
            {
                subTamaPos = SubTamaPosType.West;
            }
            else if (destColumns[0].Color == ColorType.Invalid)
            {
                subTamaPos = SubTamaPosType.South;
            }
        }
        /// <summary>
        /// 入力に対応するビットを生成
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private uint BitOfInput(KeyIndex key)
        {
            return 1u << (int)key;
        }
        private int waitControl;
        private const int waitControlFrame = 60;
        private void EnterControlTama()
        {
            waitControl = waitControlFrame;
        }
        /// <summary>
        /// 操作終了かどうかのカウントを更新
        /// </summary>
        /// <returns>操作終了ならtrue</returns>
        private bool UpdateWaitToFinishControl()
        {
            if (waitControl > 0)
            {
                --waitControl;
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// メイン基準でのたま操作
        /// </summary>
        private void UpdateControlTamaForMain()
        {
            // 操作終了なら次へ
            if (UpdateWaitToFinishControl())
            {
                stateMachine.ChangeToNextState();
            }
            // 操作中でなければ操作待ちに遷移
            else if (Self.CurrentState != PlayingState.ControlTama)
            {
                stateMachine.ChangeState(StateType.WaitControl);
            }
            else if (Self.CurrentTamaPair == null || Self.CurrentTamaPair[0] == null)
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
                stateMachine.ChangeToNextState();
            }
        }
        /// <summary>
        /// サブのたまを基準に横移動
        /// </summary>
        private void UpdateControlTamaForSub()
        {
            // 操作終了なら次へ
            if (UpdateWaitToFinishControl())
            {
                stateMachine.ChangeToNextState();
            }
            else if (Self.CurrentState != PlayingState.ControlTama)
            {
                // 操作中でなければ操作待ちに遷移
                stateMachine.ChangeState(StateType.WaitControl);
            }
            else if (Self.CurrentTamaPair == null || Self.CurrentTamaPair[1] == null)
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
                stateMachine.ChangeToNextState();
            }
        }
        private int rotateWait;
        private const int rotateCount = 1;
        /// <summary>
        /// たまの回転初期化
        /// </summary>
        private void EnterRotateTama()
        {
            rotateWait = 0;
        }
        /// <summary>
        /// たまを回転させる
        /// </summary>
        private void UpdateRotateTama()
        {
            if (RotateTama())
            {
                ChangeStateToControlTama();
                //stateMachine.ChangeState(StateType.DropTama);
                // 操作中でなければ操作待ちに遷移
                if (Self.CurrentState != PlayingState.ControlTama)
                {
                    stateMachine.ChangeState(StateType.WaitControl);
                }
            }
        }
        /// <summary>
        /// たまの回転操作
        /// </summary>
        /// <returns></returns>
        private bool RotateTama()
        {
            switch (subTamaPos)
            {
                case SubTamaPosType.North:
                    if (Self.CurrentTamaPair[0].Y < Self.CurrentTamaPair[1].Y)
                    {
                        return true;
                    }
                    break;
                case SubTamaPosType.East:
                    if (Self.CurrentTamaPair[0].X < Self.CurrentTamaPair[1].X)
                    {
                        return true;
                    }
                    break;
                case SubTamaPosType.South:
                    if (Self.CurrentTamaPair[0].Y > Self.CurrentTamaPair[1].Y)
                    {
                        return true;
                    }
                    break;
                case SubTamaPosType.West:
                    if (Self.CurrentTamaPair[0].X > Self.CurrentTamaPair[1].X)
                    {
                        return true;
                    }
                    break;
            }
            if (rotateWait > 0)
            {
                --rotateWait;
            }
            else
            {
                bits |= BitOfInput(KeyIndex.A);
                rotateWait = rotateCount;
            }
            return false;
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
