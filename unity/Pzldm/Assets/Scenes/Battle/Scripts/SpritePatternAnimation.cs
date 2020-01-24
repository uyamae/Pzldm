using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paldm
{
    public class SpritePatternAnimation
    {
        public SpritePatternAnimationData Data { get; set; }
        /// <summary>
        /// 現在のシーケンス番号
        /// </summary>
        private int index;
        /// <summary>
        /// 現在のシーケンスの残り待ちフレーム数
        /// </summary>
        private int waitFrame;
        /// <summary>
        /// 現在のスプライト番号
        /// </summary>
        public int SpriteIndex
        {
            get
            {
                return ((Data == null) || (index < 0) || (index >= Data.patterns.Length)) ? -1 : Data.patterns[index].spriteNo;
            }
        }

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return (Data == null)|| (index >= Data.patterns.Length);
            }
        }
        /// <summary>
        /// 再生開始
        /// </summary>
        public void Start()
        {
            if (Data == null) return;
            index = 0;
            waitFrame = Data.patterns[index].waitFrames;
        }
        /// <summary>
        /// 更新, Start() 呼出し後、１フレームに１回呼び出す
        /// </summary>
        /// <returns>シーケンス番号が変化したらtrue</returns>
        public bool Update()
        {
            bool changed = waitFrame <= 0;
            if (changed)
            {
                ++index;
                if (index < Data.patterns.Length)
                {
                    waitFrame = Data.patterns[index].waitFrames;
                }
            }
            --waitFrame;
            return changed;
        }
    }
}
