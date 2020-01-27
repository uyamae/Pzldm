﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public class ComPlayer : IInput 
    {
        /// <summary>
        /// 自分のPlayField
        /// </summary>
        public PlayField Self { get; set; }
        /// <summary>
        /// 相手のPlayField
        /// </summary>
        public PlayField Opponent { get; set; }
        /// <summary>
        /// 入力ビットフラグ生成
        /// </summary>
        /// <returns></returns>
        public virtual uint GenerateBits()
        {
            return 0;
        }
    }
}