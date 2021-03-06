﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pzldm
{
    /// <summary>
    /// キー入力とビットフラグ順
    /// </summary>
    public enum KeyIndex
    {
        Up,
        Down,
        Left,
        Right,
        A,
        B,
        Start,

        Count,
    }
    public partial class PlayField
    {
        /// <summary>
        /// キーリピート開始フレーム数
        /// </summary>
        public int RepeatStart { get { return setting.keyRepeatStartFrame; } }
        /// <summary>
        /// キーリピートフレーム数
        /// </summary>
        public int RepeatSpan { get { return setting.keyRepeatSpanFrame; } }

        /// <summary>
        /// 入力アクション
        /// </summary>
        private BattleInputActions.BattleActions inputActions;
        /// <summary>
        /// アセット
        /// </summary>
        public InputActionAsset inputActionAsset;
        /// <summary>
        /// 方向キー
        /// </summary>
        private InputAction inputDpad;
        /// <summary>
        /// ボタン
        /// </summary>
        private (InputAction A, InputAction B, InputAction Start) inputButtons;

        /// <summary>
        /// 常時更新されるボタン押し状態
        /// </summary>
        private uint realTimePressed;
        /// <summary>
        /// 現在押されている
        /// </summary>
        private uint currentPressed;
        /// <summary>
        /// 前回押されていた
        /// </summary>
        private uint prevPressed;
        /// <summary>
        /// 現在トリガーされている
        /// </summary>
        private uint currentTriggered;
        /// <summary>
        /// 現在放されたボタン
        /// </summary>
        private uint currentReleased;
        /// <summary>
        /// 現在キーリピートされているボタン
        /// </summary>
        private uint currentRepeated;
        /// <summary>
        /// キーリピートのフレーム
        /// </summary>
        private int[] repeatFrames = new int[(int)KeyIndex.Count];

        /// <summary>
        /// 操作用COM
        /// </summary>
        public IInput ComPlayer{ get; set; }

        /// <summary>
        /// 入力アクションを初期化
        /// </summary>
        private void InitInputActions()
        {
            if (inputActionAsset != null)
            {
                inputDpad = inputActionAsset.FindAction("Dpad");
                var rotR = inputActionAsset.FindAction("RotateRight");
                var rotL = inputActionAsset.FindAction("RotateLeft");
                var start = inputActionAsset.FindAction("Pause");
                rotR.performed += OnRotateRight;
                rotR.canceled += OnRotateRight;
                rotL.performed += OnRotateLeft;
                rotL.canceled += OnRotateLeft;
                start.performed += OnPause;
                start.canceled += OnPause;
                inputButtons = (rotR, rotL, start);
            }
            else
            {
                inputActions = new BattleInputActions.BattleActions(new BattleInputActions());
                inputActions.SetCallbacks(this);
                inputDpad = inputActions.Dpad;
                inputButtons = (inputActions.RotateRight, inputActions.RotateLeft, inputActions.RotateRight);
            }
        }
        private void EnableInput()
        {
            if (inputActionAsset != null)
            {
                inputActionAsset.Enable();
            }
            else
            {
                inputActions.Enable();
            }
        }
        private void DisableInput()
        {
            if (inputActionAsset != null)
            {
                inputActionAsset.Disable();
            }
            else
            {
                inputActions.Disable();
            }
        }
        /// <summary>
        /// ボタンが押されているかチェック
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckPressedKey(KeyIndex key)
        {
            return ((uint)(1 << (int)key) & currentPressed) != 0;
        }
        /// <summary>
        /// ボタンがトリガーされたかチェック
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckTriggeredKey(KeyIndex key)
        {
            return ((uint)(1 << (int)key) & currentTriggered) != 0;
        }
        /// <summary>
        /// ボタンが放されたかチェック
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckReleasedKey(KeyIndex key)
        {
            return ((uint)(1 << (int)key) & currentReleased) != 0;
        }
        /// <summary>
        /// ボタンがキーリピートされたかチェック
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckRepeatedKey(KeyIndex key)
        {
            return ((uint)(1 << (int)key) & currentRepeated) != 0;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void InitInputEx()
        {
            currentPressed = 0;
            prevPressed = 0;
            currentTriggered = 0;
            currentReleased = 0;
            currentRepeated = 0;
            for (int i = 0; i < repeatFrames.Length; ++i)
            {
                repeatFrames[i] = 0;
            }
        }
        public void OnRotateLeft(InputAction.CallbackContext context)
        {
            OnButtonInput(context, KeyIndex.B);
        }
        public void OnRotateRight(InputAction.CallbackContext context)
        {
            OnButtonInput(context, KeyIndex.A);
        }
        public void OnPause(InputAction.CallbackContext context)
        {
            OnButtonInput(context, KeyIndex.Start);
        }
        private void OnButtonInput(InputAction.CallbackContext context, KeyIndex index)
        {
            uint bit = (uint)(1 << (int)index);
            if (context.ReadValue<float>() > 0.5f)
            {
                realTimePressed |= bit;
            }
            else
            {
                realTimePressed &= ~bit;
            }
           
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void UpdateInputEx()
        {
            if (stateMachine.CurrentState != PlayingState.ControlTama)
            {
                return;
            }
            // 履歴更新
            prevPressed = currentPressed;
            currentPressed = GenerateBits();
            // トリガー更新
            currentTriggered = currentPressed & ~prevPressed;
            currentReleased = ~currentPressed & prevPressed;
            // キーリピートの更新
            currentRepeated = 0;
            uint bit = 1;
            for (int i = 0; i < (int)KeyIndex.Count; ++i, bit <<= 1)
            {
                if ((currentTriggered & bit) != 0)
                {
                    currentRepeated |= bit;
                    repeatFrames[i] = RepeatStart;
                }
                else if ((currentPressed & bit) != 0)
                {
                    if (repeatFrames[i] == 0)
                    {
                        currentRepeated |= bit;
                        repeatFrames[i] = RepeatSpan;
                    }
                    else
                    {
                        // 念のための補正
                        if (repeatFrames[i] > RepeatSpan)
                        {
                            repeatFrames[i] = RepeatSpan;
                        }
                        --repeatFrames[i];
                    }
                }
            }
        }
        private uint GenerateBits()
        {
            // ComPlayer が設定されていたらそちらに任せる
            if (ComPlayer != null)
            {
                return ComPlayer.GenerateBits();
            }

            uint bits = 0;
            Vector2 v = inputDpad.ReadValue<Vector2>();
            if (v.x < 0)
            {
                bits |= (uint)(1 << (int)KeyIndex.Left);
            }
            else if (v.x > 0)
            {
                bits |= (uint)(1 << (int)KeyIndex.Right);
            }
            if (v.y < 0)
            {
                bits |= (uint)(1 << (int)KeyIndex.Down);
            }
            else if (v.y > 0)
            {
                bits |= (uint)(1 << (int)KeyIndex.Up);
            }
            // ボタンの状態を反映
            bits |= realTimePressed;

            return bits;
        }
    }
}
