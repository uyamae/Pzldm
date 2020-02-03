using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public partial class PlayField
    {
        private void StateGameOverEnter()
        {
            SetupGameOverDirection();
        }
        private void StateGameOverUpdate()
        {
            if (UpdateGameOverDirection())
            {
                if (IsStateManaged)
                {
                    ChangeState(PlayingState.AskContinue);
                }
                else
                {
                    ChangeState(PlayingState.Ready);
                }
            }
        }
        private void StateGameOverLeave()
        {

        }
        // ゲームオーバー演出初期化
        private void SetupGameOverDirection()
        {
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    var data = GetTamaFromField(x, y);
                    if (data == null) continue;
                    float vx = UnityEngine.Random.Range(setting.gameOverDirectionVelocityMin.x, setting.gameOverDirectionVelocityMax.x);
                    float vy = UnityEngine.Random.Range(setting.gameOverDirectionVelocityMin.y, setting.gameOverDirectionVelocityMax.y);
                    data.Velocity = new Vector2(vx, vy);
                }
            }
            // 操作するたまとつぎのたまはリリース
            System.Action<TamaData[]> release = pair =>
            {
                for (int i = 0; i < pair.Length; ++i)
                {
                    if (pair[i] == null) continue;
                    tamaGenerator.ReleaseTama(pair[i]);
                    pair[i] = null;
                }
            };
            release(currentTamaPair);
            release(nextTamaPair);
        }
        // ゲームオーバー演出更新
        private bool UpdateGameOverDirection()
        {
            bool finished = true;
            for (int x = 0; x < setting.columnsCount; ++x)
            {
                for (int y = 0; y < setting.rowsCount; ++y)
                {
                    var data = GetTamaFromField(x, y);
                    if (data == null) continue;
                    var v = data.Sprite.transform.localPosition;
                    v.x += data.Velocity.x;
                    v.y += data.Velocity.y;
                    if (v.y < -5)
                    {
                        RemoveTamaFromField(x, y);
                        tamaGenerator.ReleaseTama(data);
                    }
                    else
                    {
                        data.Sprite.transform.localPosition = v;
                        data.Velocity = new Vector2(data.Velocity.x, data.Velocity.y + setting.gameOverDirectionGravity);
                        finished = false;
                    }
                }
            }
            return finished;
        }
    }
}
