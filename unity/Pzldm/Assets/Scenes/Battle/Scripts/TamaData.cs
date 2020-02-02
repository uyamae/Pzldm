using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    /// <summary>
    /// たまの色
    /// </summary>
    public enum ColorType
    {
        Red, Blue, Green, Yellow, Invalid
    }
    /// <summary>
    /// たまの状態
    /// </summary>
    public enum TamaStateType
    {
        /// <summary>
        /// たま無し
        /// </summary>
        None,
        /// <summary>
        /// おおだま
        /// </summary>
        Large,
        /// <summary>
        /// こだま
        /// </summary>
        Small
    }
    /// <summary>
    /// たまのデータ
    /// </summary>
    public class TamaData
    {
        /// <summary>
        /// 色
        /// </summary>
        public ColorType Color { get; set; }
        /// <summary>
        /// 状態
        /// </summary>
        public TamaStateType State { get; set; }
        /// <summary>
        /// たま表示用スプライト
        /// </summary>
        public SpriteRenderer Sprite { get; set; }
        /// <summary>
        /// たまを消すときのエフェクトのスプライト
        /// </summary>
        public SpriteRenderer Spark { get; set; }

        /// <summary>
        /// ゲームオーバー時の演出用
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// 現在の行
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 現在の列
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 半分浮いている
        /// </summary>
        public bool Half { get; set; }
    }
}
