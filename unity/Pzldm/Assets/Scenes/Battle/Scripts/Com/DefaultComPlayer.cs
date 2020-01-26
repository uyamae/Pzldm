using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public class DefaultComPlayer : ComPlayer
    {
        public override uint GenerateBits()
        {
            int column = DecideColumn();
            return DecideDirection(column);
        }

        private int DecideColumn()
        {
            int column = 0;
            int height = 0;
            int rowsCount = Self.TamaField.GetLength(0);
            int columnsCount = Self.TamaField.GetLength(1);
            for (int i = 0; i < columnsCount; ++i)
            {
                for (int j = 0; j < rowsCount; ++j)
                {
                    if (Self.GetTamaFromField(i, j) == null)
                    {
                        if (height < j)
                        {
                            column = i;
                            height = j;
                        }
                        break;
                    }
                }
            }
            return column;
        }
        /// <summary>
        /// 動かす方向を決める
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private uint DecideDirection(int column)
        {
            if ((Self.CurrentTamaPair == null) || (Self.CurrentTamaPair[0] == null)) return 0;
            int index = 0;
            if (Self.CurrentTamaPair[0].X < column)
            {
                index = (int)KeyIndex.Right;
            }
            else if (Self.CurrentTamaPair[0].X > column)
            {
                index = (int)KeyIndex.Left;
            }
            return (uint)(1 << index);
        }
    }
}
