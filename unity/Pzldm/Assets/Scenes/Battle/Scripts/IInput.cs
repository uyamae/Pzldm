using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public interface IInput
    {
        /// <summary>
        /// 入力ビットフラグ生成
        /// </summary>
        /// <returns>入力ビットフラグ</returns>
        uint GenerateBits();
    }
}
