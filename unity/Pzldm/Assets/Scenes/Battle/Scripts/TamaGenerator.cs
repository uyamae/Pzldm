using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pzldm
{
    /// <summary>
    /// たまを生成する
    /// </summary>
    public class TamaGenerator : MonoBehaviour
    //public partial class PlayField
    {
        /// <summary>
        /// たまオブジェクトprefab
        /// </summary>
        public SpriteRenderer tamaPrefab;
        /// <summary>
        /// 使用するテクスチャアトラス
        /// </summary>
        public UnityEngine.U2D.SpriteAtlas atlas;
        /// <summary>
        /// たまのスプライトのキャッシュ
        /// </summary>
        private Sprite[] tamaSprites;
        /// <summary>
        /// たまのスプライトを取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sprite GetTamaSprite(int index)
        {
            return tamaSprites[index];
        }
        /// <summary>
        /// たまが消えるときのエフェクトスプライト
        /// </summary>
        private Sprite[] tamaSparkSprites;
        /// <summary>
        /// たまが消えるときのエフェクトスプライト取得
        /// </summary>
        public Sprite GetTamaSparkSprite(int index)
        {
            return tamaSparkSprites[index];
        }
        public int TamaSparkSpriteCount { get { return tamaSparkSprites.Length; } }

        /// <summary>
        /// たまを作っておく
        /// </summary>
        private Stack<TamaData> tamaCache;
        /// <summary>
        /// たま生成用乱数
        /// </summary>
        private System.Random tamaRand;

        /// <summary>
        /// スプライトをキャッシュ
        /// </summary>
        public void InitTamaSpriteCache(PlayFieldSetting setting)
        {
            tamaSprites = CreateSpriteCache(setting.tamaSpriteNames);
            tamaSparkSprites = CreateSpriteCache(setting.sparkSpriteNames);
        }
        /// <summary>
        /// たま表示用スプライト初期化
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        private Sprite[] CreateSpriteCache(string[] names)
        {
            Sprite[] sprites = new Sprite[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                sprites[i] = atlas.GetSprite(names[i]);
            }
            return sprites;
        }
        /// <summary>
        /// たまをキャッシュ
        /// </summary>
        public void InitTamaCache(PlayFieldSetting setting, Transform transform)
        {
            if (tamaCache != null) return;
            var count = setting.columnsCount * setting.rowsCount + 4;
            tamaCache = new Stack<TamaData>(count);
            for (int i = 0; i < count; ++i)
            {
                var s = GameObject.Instantiate<SpriteRenderer>(tamaPrefab);
                //s.sprite = atlas.GetSprite("l01");
                s.transform.parent = transform;
                s.transform.localPosition = Vector3.zero;
                s.transform.localScale = Vector3.one;
                s.enabled = false;
                var e = s.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                var tama = new TamaData() { Sprite = s, Spark = e };
                tamaCache.Push(tama);
            }
        }
        /// <summary>
        /// 次回リセット時に使用するたま生成乱数シード
        /// </summary>
        private int tamaRandomSeed;
        /// <summary>
        /// たま生成乱数の初期化
        /// </summary>
        public void InitTamaRandom(PlayFieldSetting setting)
        {
            InitTamaRandom(setting.tamaRandomSeed);
        }
        /// <summary>
        /// たま生成乱数の初期化
        /// </summary>
        public void InitTamaRandom(int seed)
        {
            tamaRand = new System.Random(seed);
            tamaRandomSeed = tamaRand.Next();
        }
        /// <summary>
        /// たま生成乱数のリセット
        /// </summary>
        public void ResetTamaRandom()
        {
            tamaRand = new System.Random(tamaRandomSeed);
            tamaRandomSeed = tamaRand.Next();
        }
        /// <summary>
        /// たまのペアを生成
        /// </summary>
        public void GenerateTamaPair(PlayFieldSetting setting, TamaData[] pair)
        {
            // たまをキャッシュから取得し色を決める
            pair[0] = GenerateTama();
            pair[1] = GenerateTama();
            // たまの組み合わせを決める
            TamaStateType mt, st;
            GenerateTamaPairType(setting, out mt, out st);
            pair[0].State = mt;
            pair[1].State = st;
            // たまのスプライトを設定する
            SetupTamaSprite(pair[0]);
            SetupTamaSprite(pair[1]);
        }
        /// <summary>
        /// たまを１つ生成
        /// </summary>
        /// <returns></returns>
        private TamaData GenerateTama()
        {
            TamaData data = tamaCache.Pop();
            data.Color = (ColorType)tamaRand.Next(4);
            data.Half = false;
            return data;
        }
        /// <summary>
        /// こうげきだまを１つ生成
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public TamaData GenerateAttackTama(ColorType color)
        {
            TamaData data = tamaCache.Pop();
            data.Color = color;
            data.Half = false;
            data.State = TamaStateType.Small;
            SetupTamaSprite(data);
            return data;
        }
        /// <summary>
        /// たまの組み合わせを決める
        /// </summary>
        /// <returns></returns>
        private void GenerateTamaPairType(PlayFieldSetting setting, out TamaStateType mainState, out TamaStateType subState)
        {
            int sum = setting.tamaPairRate.Sum();
            int r = tamaRand.Next(sum);
            TamaPairType pair = TamaPairType.LL;
            for (int i = 0; i < setting.tamaPairRate.Length; ++i)
            {
                int rate = setting.tamaPairRate[i];
                if (r < rate)
                {
                    pair = (TamaPairType)i;
                    break;
                }
                r -= rate;
            }
            switch (pair)
            {
                case TamaPairType.LL:
                    mainState = TamaStateType.Large;
                    subState = TamaStateType.Large;
                    break;
                case TamaPairType.LS:
                    mainState = TamaStateType.Large;
                    subState = TamaStateType.Small;
                    break;
                case TamaPairType.SS:
                    mainState = TamaStateType.Small;
                    subState = TamaStateType.Small;
                    break;
                default:
                    throw new System.ArgumentException();
            }
        }
        /// <summary>
        /// たまの見た目を設定
        /// </summary>
        /// <param name="data"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void SetupTamaSprite(TamaData data)
        {
            int index = (int)data.Color;
            if (data.State == TamaStateType.Small) index += 4;
            data.Sprite.sprite = tamaSprites[index];
            data.Sprite.enabled = true;
            //data.Sprite.transform.localPosition = new Vector3(setting.cellSize * col, setting.cellSize * row, 0);
            data.Sprite.transform.localScale = Vector3.one;

            data.Spark.enabled = false;
        }
        /// <summary>
        /// たまをキャッシュに戻す
        /// </summary>
        /// <param name="data"></param>
        public void ReleaseTama(TamaData data)
        {
            data.Sprite.enabled = false;
            data.Spark.enabled = false;

            tamaCache.Push(data);
        }
    }
    /// <summary>
    /// たま生成関連処理
    /// </summary>
    public partial class PlayField
    {
        [SerializeField]
        private TamaGenerator tamaGeneratorSource;
        private TamaGenerator tamaGenerator;
        /// <summary>
        /// たまジェネレータを初期化
        /// </summary>
        public void InitTamaGenerator()
        {
            tamaGenerator = GameObject.Instantiate<TamaGenerator>(tamaGeneratorSource);
            int seed = setting.tamaRandomSeed;
            if (PzldmManager.Instance != null)
            {
                seed = PzldmManager.Instance.RandomSeed;
            }
            tamaGenerator.InitTamaRandom(seed);
            tamaGenerator.InitTamaSpriteCache(setting);
            tamaGenerator.InitTamaCache(setting, this.transform);
        }
        /// <summary>
        /// つぎのたまを生成
        /// </summary>
        private void GenerateNextTamaPair()
        {
            tamaGenerator.GenerateTamaPair(setting, nextTamaPair);
            // たまの位置を設定する
            Vector2Int pos = (playerNumber >= setting.nextTamaPositions.Length) ? setting.nextTamaPosition : setting.nextTamaPositions[playerNumber];
            SetTamaPosition(nextTamaPair[0], pos.x, pos.y);
            SetTamaPosition(nextTamaPair[1], pos.x, pos.y + 1);
        }
        /// <summary>
        /// フィールドのたまを消す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ReleaseTamaInField(int x, int y)
        {
            if (tamaField[y, x] == null) return;
            tamaGenerator.ReleaseTama(tamaField[y, x]);
            tamaField[y, x] = null;
            // まわりのこだまをおおだまにする
            (int x, int y)[] offsets = new (int x, int y)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            for (int i = 0; i < offsets.Length; ++i)
            {
                (int ox, int oy) = offsets[i];
                int nx = x + ox;
                int ny = y + oy;
                if ((nx < 0) || (nx >= setting.columnsCount) || (ny < 0) || (ny >= setting.rowsCount))
                {
                    continue;
                }
                if (tamaField[ny, nx]?.State == TamaStateType.Small)
                {
                    tamaField[ny, nx].State = TamaStateType.Large;
                    int index = (int)tamaField[ny, nx].Color;
                    tamaField[ny, nx].Sprite.sprite = tamaGenerator.GetTamaSprite(index);
                }
            }
        }
    }
}
