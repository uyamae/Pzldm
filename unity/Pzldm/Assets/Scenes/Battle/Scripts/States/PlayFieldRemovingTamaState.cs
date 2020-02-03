using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public partial class PlayField
    {
        private int TamaSparkCount { get { return 10; } }
        private int TamaSparkWaitFrame { get { return 5; } }

        /// <summary>
        /// 表示するスプライトの番号
        /// </summary>
        private int tamaSparkSpriteIndex;
        /// <summary>
        /// スプライトを切り替える回数
        /// </summary>
        private int tamaSparkCount;
        /// <summary>
        /// スプライトを切り替えるまでの待ちフレーム数
        /// </summary>
        private int tamaSparkWaitFrame;
        
        private void StateRemovingTamaEnter()
        {
            tamaSparkSpriteIndex = 0;
            tamaSparkCount = TamaSparkCount;
            tamaSparkWaitFrame = TamaSparkWaitFrame;
        }
        private void StateRemovingTamaUpdate()
        {
            UpdateSparkRemovingCount();
        }
        private void StateRemovingTamaLeave()
        {
            RemoveChainedTamaFromField();
        }
        /// <summary>
        /// 消滅アニメーション更新
        /// </summary>
        private void UpdateSparkRemovingCount()
        {
            if (tamaSparkWaitFrame <= 0)
            {
                if (tamaSparkCount <= 0)
                {
                    // つぎのステートへ
                    ChangeState(PlayingState.DroppingTama);
                    return;
                }
                --tamaSparkCount;
                ++tamaSparkSpriteIndex;
                if (tamaSparkSpriteIndex == tamaGenerator.TamaSparkSpriteCount)
                {
                    tamaSparkSpriteIndex = 0;
                }
                tamaSparkWaitFrame = TamaSparkWaitFrame;
                // スプライトを入れ替える
                Sprite sprite = tamaGenerator.GetTamaSparkSprite(tamaSparkSpriteIndex);
                for (int x = 0; x < setting.columnsCount; ++x)
                {
                    for (int y = 0; y < setting.rowsCount; ++y)
                    {
                        if (tamaField[y, x] == null) break;

                        if (tamaField[y, x].Spark.enabled)
                        {
                            tamaField[y, x].Spark.sprite = sprite;
                        }
                    }
                }
            }
            --tamaSparkWaitFrame;
        }
        /// <summary>
        /// 設置されているたまをキャッシュにもどす
        /// </summary>
        private void RemoveChainedTamaFromField()
        {
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    if (tamaField[y, x] == null) break;

                    if (tamaField[y, x].Spark.enabled)
                    {
                        ReleaseTamaInField(x, y);
                    }
                }
            }
        }
    }
}
