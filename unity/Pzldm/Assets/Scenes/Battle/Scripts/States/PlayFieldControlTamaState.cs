using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public partial class PlayField
    {
        /// <summary>
        /// たまが自由落下するフレーム数
        /// </summary>
        private int TamaFallFrame { get; set; }
        /// <summary>
        /// たまが自由落下するフレームのカウンター
        /// </summary>
        private int tamaFallFrameCount;
        private void StateControlTamaEnter()
        {
            // つぎのたまを操作たまにする
            currentTamaPair[0] = nextTamaPair[0];
            currentTamaPair[1] = nextTamaPair[1];
            // 操作たまの位置を設定する
            SetTamaPosition(currentTamaPair[0], setting.startTamaPosition.x, setting.startTamaPosition.y);
            SetTamaPosition(currentTamaPair[1], setting.startTamaPosition.x, setting.startTamaPosition.y + 1);
            // つぎのたまを生成する
            GenerateNextTamaPair();
            // 自由落下カウンターをリセット
            tamaFallFrameCount = TamaFallFrame;
        }
        private void StateControlTamaUpdate()
        {
            // 左右操作チェック
            MoveHorizontal();
            // 回転操作チェック
            ProcessRotate();
            // 落下チェック
            ProcessTamaFall();


            //// 仮操作
            //if (CheckRepeatedKey(KeyIndex.Up))
            //{
            //    MoveTamaPairUp(currentTamaPair[0], currentTamaPair[1]);
            //}
        }
        private void StateControlTamaLeave()
        {

        }
        /// <summary>
        /// 横方向の移動
        /// </summary>
        private void MoveHorizontal()
        {
            // メインのたまが開始位置より高い場合左右移動させない
            if (currentTamaPair[0].Y >= setting.startTamaPosition.y)
            {
                return;
            }
            
            if (CheckRepeatedKey(KeyIndex.Left))
            {
                MoveTamaPairLeft(currentTamaPair[0], currentTamaPair[1]);
            }
            else if (CheckRepeatedKey(KeyIndex.Right))
            {
                MoveTamaPairRight(currentTamaPair[0], currentTamaPair[1]);
            }
        }
        /// <summary>
        /// 回転操作
        /// </summary>
        private void ProcessRotate()
        {
            if (CheckRotateTrigger(KeyIndex.A, KeyIndex.B))
            {
                RotateTamaPair(currentTamaPair[0], currentTamaPair[1], true);
            }
            else if (CheckRotateTrigger(KeyIndex.B, KeyIndex.A))
            {
                RotateTamaPair(currentTamaPair[0], currentTamaPair[1], false);
            }
        }
        /// <summary>
        /// 片方押しながらもう片方放しても入力を受け付けるやつ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool CheckRotateTrigger(KeyIndex a, KeyIndex b)
        {
            // トリガーされてたら入力あり
            if (CheckTriggeredKey(a)) return true;
            // 反対を押しっぱなしの場合
            if (CheckPressedKey(b) && CheckReleasedKey(a)) return true;
            // それ以外
            return false;
        }

        /// <summary>
        /// たまのペアを回転
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="cw">true:時計回り</param>
        private void RotateTamaPair(TamaData main, TamaData sub, bool cw)
        {
            (int rotX, int rotY) = CalculateRotateOffset(main, sub, cw);
            int subX = main.X + rotX;
            int subY = main.Y + rotY;
            // フィールドが空ではない場合
            if (!CheckFieldBlank(subX, subY))
            {
                // メインが動かせるかチェック
                if (CheckMove(main, -rotX, -rotY))
                {
                    subX = main.X + rotX;
                    subY = main.Y + rotY;
                }
                // 動かせなくて縦なら上下入れ替え
                else if (main.X == sub.X)
                {
                    int x = main.X;
                    int mainY = sub.Y;
                    subY = main.Y;
                    SetTamaPosition(main, x, mainY);
                    SetTamaPosition(sub, x, subY);
                    return;
                }
            }
            // サブの位置を確定
            SetTamaPosition(sub, subX, subY);
        }
        private (int, int) CalculateRotateOffset(TamaData main, TamaData sub, bool cw)
        {
            // サブのオフセット
            int x = sub.X - main.X;
            int y = sub.Y - main.Y;
            // 回転後のオフセット
            return cw ? (y, -x) : (-y, x);
        }
        /// <summary>
        /// 指定フィールドが空きかどうかチェック
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckFieldBlank(int x, int y)
        {
            if (x < 0 || x >= setting.columnsCount) return false;
            if (y < 0 || y >= setting.rowsCount) return false;
            return GetTamaFromField(x, y) == null;
        }
        /// <summary>
        /// 指定方向に動かせるなら動かす
        /// </summary>
        /// <param name="main"></param>
        /// <param name="moveX"></param>
        /// <param name="moveY"></param>
        /// <returns></returns>
        private bool CheckMove(TamaData main, int moveX, int moveY)
        {
            int x = main.X + moveX;
            int y = main.Y + moveY;
            if (GetTamaFromField(x, y) != null) return false;
            // 動かせるなら動かす
            SetTamaPosition(main, x, y);
            return true;
        }
        /// <summary>
        /// 操作たまの落下処理
        /// </summary>
        private void ProcessTamaFall()
        {
            // 下に入力がなければ
            if (!CheckPressedKey(KeyIndex.Down))
            {
                // 落下カウント
                --tamaFallFrameCount;
                if (tamaFallFrameCount > 0) return;
            }
            // 落下カウントリセット
            tamaFallFrameCount = TamaFallFrame;
            // 落下処理
            if (!MoveTamaPairDown(currentTamaPair[0], currentTamaPair[1]))
            {
                // 落下できなかった場合は設置処理に遷移する
                ChangeState(PlayingState.PuttingTama);
            }
        }
        /// <summary>
        /// ゲームオーバーになっているかチェック
        /// </summary>
        private void CheckGameOver()
        {
            if (GetTamaFromField(setting.startTamaPosition.x, setting.startTamaPosition.y) != null)
            {
                IsGameOver = true;
            }
        }
    }
}
