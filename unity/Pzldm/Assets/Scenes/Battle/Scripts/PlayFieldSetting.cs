using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    [CreateAssetMenu(menuName = "Pzldm/PlayFieldSetting")]
    public class PlayFieldSetting : ScriptableObject
    {
        /// <summary>
        /// フィールドの幅
        /// </summary>
        public int columnsCount;
        /// <summary>
        /// フィールドの高さ
        /// </summary>
        public int rowsCount;
        /// <summary>
        /// こうげきだまがキャラごとパターンで降ってくる高さ
        /// </summary>
        public int patternRowsCount;
        /// <summary>
        /// たま１つ動かすときの移動値
        /// </summary>
        public float cellSize;
        /// <summary>
        /// たまが落下するときに動かす移動値
        /// </summary>
        public float fallSize;
        /// <summary>
        /// たまの組み合わせの割合(TamaPairType の順序通り)
        /// </summary>
        public int[] tamaPairRate;
        /// <summary>
        /// たま生成の乱数シード
        /// </summary>
        public int tamaRandomSeed;
        /// <summary>
        /// つぎのたまの表示位置(1P, メイン)
        /// </summary>
        public Vector2Int nextTamaPosition;
        /// <summary>
        /// つぎのたまの表示位置
        /// </summary>
        public Vector2Int[] nextTamaPositions;
        /// <summary>
        /// たま開始位置(メイン)
        /// </summary>
        public Vector2Int startTamaPosition;

        /// <summary>
        /// Ready ステートの待ち時間秒数
        /// </summary>
        public float readyWaitSeconds;


        /// <summary>
        /// キーリピート初回の待ちフレーム数
        /// </summary>
        public int keyRepeatStartFrame;
        /// <summary>
        /// キーリピートの感覚フレーム数
        /// </summary>
        public int keyRepeatSpanFrame;

        /// <summary>
        /// たまが自由落下するフレーム数
        /// </summary>
        public int tamaFallFrame;
        /// <summary>
        /// 操作たま設置時の待ち時間
        /// </summary>
        public int tamaPutWait;

        /// <summary>
        /// ゲームオーバー時のばらまき演出の速度最小値
        /// </summary>
        public Vector2 gameOverDirectionVelocityMin;
        /// <summary>
        /// ゲームオーバー時のばらまき演出の速度最大値
        /// </summary>
        public Vector2 gameOverDirectionVelocityMax;
        /// <summary>
        /// ゲームオーバー時のばらまき演出の重力加速度
        /// </summary>
        public float gameOverDirectionGravity;

        /// <summary>
        /// こうげきだませり上げ時のウェイトフレーム数
        /// </summary>
        public int attackLiftUpWaitFrame;
        /// <summary>
        /// こうげきだま落下時のウェイトフレーム数
        /// </summary>
        public int attackDropWaitFrame;

        /// <summary>
        /// スプライト名
        /// </summary>
        public string[] tamaSpriteNames;
        /// <summary>
        /// 消えるときのエフェクトのスプライト名
        /// </summary>
        public string[] sparkSpriteNames;
    }
}
