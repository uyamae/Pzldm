using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Pzldm
{
    /// <summary>
    /// たまの組み合わせ
    /// </summary>
    public enum TamaPairType
    {
        /// <summary>
        /// おおだま・おおだま
        /// </summary>
        LL,
        /// <summary>
        /// おおだま・こだま
        /// </summary>
        LS,
        /// <summary>
        /// こだま・こだま
        /// </summary>
        SS,
    }
    /// <summary>
    /// プレイヤー一人分のフィールド操作
    /// </summary>
    public partial class PlayField : MonoBehaviour, BattleInputActions.IBattleActions
    {
        /// <summary>
        /// 設定ファイル
        /// </summary>
        public PlayFieldSetting setting;

        /// <summary>
        /// つぎのたま
        /// </summary>
        private TamaData[] nextTamaPair = new TamaData[2];
        /// <summary>
        /// つぎのたま
        /// </summary>
        public TamaData[] NextTamaPair { get { return nextTamaPair; } }
        /// <summary>
        /// 操作するたま
        /// </summary>
        private TamaData[] currentTamaPair = new TamaData[2];
        /// <summary>
        /// 操作するたま
        /// </summary>
        public TamaData[] CurrentTamaPair { get { return currentTamaPair; } }
        /// <summary>
        /// たまを置くフィールド
        /// </summary>
        private TamaData[,] tamaField;
        /// <summary>
        /// たまを置くフィールド
        /// </summary>
        public TamaData[,] TamaField { get { return tamaField; } }
        /// <summary>
        /// 1P or 2P
        /// </summary>
        public int playerNumber;
        /// <summary>
        /// 連鎖数
        /// </summary>
        public int ChainCount { get; set; }
        /// <summary>
        /// 送るこうげきだまの数
        /// </summary>
        [SerializeField]
        private int sendAttackCount;
        /// <summary>
        /// 送るこうげきだまの数
        /// </summary>
        public int SendAttackCount
        {
            get { return sendAttackCount; }
            set { sendAttackCount = value; }
        }
        /// <summary>
        /// 受け取ったこうげきだまの数
        /// </summary>
        [SerializeField]
        private int recievedAttackCount;
        /// <summary>
        /// 受け取ったこうげきだまの数
        /// </summary>
        public int RecievedAttackCount
        {
            get { return recievedAttackCount; }
            set { recievedAttackCount = value; }
        }
        /// <summary>
        /// ゲームオーバーフラグ
        /// </summary>
        public bool IsGameOver { get; set; }

        /// <summary>
        /// プレイの進行フレームカウント
        /// </summary>
        public int PlayFrameCount { get; set; }

        /// <summary>
        /// フィールドのたまを取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TamaData GetTamaFromField(int x, int y)
        {
            if (x < 0 || x >= tamaField.GetLength(1) || y < 0 || y >= tamaField.GetLength(0))
            {
                Debug.LogError($"x:{x}, y:{y}");
            }
            return tamaField[y, x];
        }
        /// <summary>
        /// フィールドのたまを設置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="data"></param>
        private void PutTamaToField(int x, int y, TamaData data)
        {
            tamaField[y, x] = data;
        }
        /// <summary>
        /// たまを現在の座標に設置
        /// </summary>
        /// <param name="data"></param>
        private void PutTamaToField(TamaData data)
        {
            PutTamaToField(data.X, data.Y, data);
        }
        /// <summary>
        /// たまをフィールドから消去
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void RemoveTamaFromField(int x, int y)
        {
            tamaField[y, x] = null;
        }

        void Awake()
        {
            InitInputActions();
            InitInputEx();
        }
        // Start is called before the first frame update
        void Start()
        {
            InitTamaGenerator();
            InitPlayField();
            InitPlayingState();

            //GenerateTamaPair();
        }

        void OnDestroy() => DisableInput();
        void OnEnable() => EnableInput();
        void OnDisable() => DisableInput();

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            UpdateInputEx();
            ProcessState();
            ++PlayFrameCount;
        }
        /// <summary>
        /// フィールドを初期化
        /// </summary>
        private void InitPlayField()
        {
            tamaField = new TamaData[setting.rowsCount, setting.columnsCount];
            tamaFieldCheck = new int[setting.rowsCount, setting.columnsCount];
            tamaGroupLinkCount = new int[setting.rowsCount * setting.columnsCount];
        }
        /// <summary>
        /// たまの位置を設定
        /// </summary>
        private void SetTamaPosition(TamaData data, int col, int row)
        {
            if (data == null) return;
            data.X = col;
            data.Y = row;
            float y = (float)row;
            if (data.Half) y += 0.5f; // 半分浮いている
            data.Sprite.transform.localPosition = new Vector3(setting.cellSize * col, setting.cellSize * y, 0);
        }
        /// <summary>
        /// たまを左に動かす
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        private void MoveTamaLeft(TamaData data, int x = 1)
        {
            if (data == null) return;
            SetTamaPosition(data, data.X - x, data.Y);
        }
        /// <summary>
        /// たまを右に動かす
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        private void MoveTamaRight(TamaData data, int x = 1)
        {
            if (data == null) return;
            SetTamaPosition(data, data.X + x, data.Y);
        }
        /// <summary>
        /// たまを下に動かす
        /// </summary>
        /// <param name="data"></param>
        /// <param name="useHalf">半分ずつ動かすかどうか</param>
        private void MoveTamaDown(TamaData data, bool useHalf = true)
        {
            if (data == null) return;
            // 半分浮いてる場合
            if (data.Half)
            {
                data.Half = false;
                SetTamaPosition(data, data.X, data.Y);
            }
            else
            {
                data.Half = useHalf;
                SetTamaPosition(data, data.X, data.Y - 1);
            }
        }
        /// <summary>
        /// たまを上に動かす
        /// </summary>
        /// <param name="data"></param>
        /// <param name="useHalf">半分ずつ動かすかどうか</param>
        private void MoveTamaUp(TamaData data, bool useHalf = true)
        {
            if (data == null) return;
            if (data.Half || !useHalf)
            {
                // 半分浮いてるか、半分ずつ動かさないなら一つ上にあげる
                data.Half = false;
                SetTamaPosition(data, data.X, data.Y + 1);
            }
            else
            {
                // 半分浮かせる
                data.Half = true;
                SetTamaPosition(data, data.X, data.Y);
            }
        }
        /// <summary>
        /// たまのペアを左に動かす
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="x"></param>
        /// <returns>動かしたらtrue</returns>
        private bool MoveTamaPairLeft(TamaData main, TamaData sub, int x = 1)
        {
            if (!CheckMovingHorizontal(main, -1) || !CheckMovingHorizontal(sub, -1)) return false;

            MoveTamaLeft(main, x);
            MoveTamaLeft(sub, x);
            return true;
        }
        /// <summary>
        /// たまのペアを右に動かす
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="x"></param>
        /// <returns>動かしたらtrue</returns>
        private bool MoveTamaPairRight(TamaData main, TamaData sub, int x = 1)
        {
            if (!CheckMovingHorizontal(main, 1) || !CheckMovingHorizontal(sub, 1)) return false;

            MoveTamaRight(main, x);
            MoveTamaRight(sub, x);
            return true;
        }
        /// <summary>
        /// たまのペアを下に動かす
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="y"></param>
        /// <returns>動かしたらtrue</returns>
        private bool MoveTamaPairDown(TamaData main, TamaData sub)
        {
            if (!CheckMovingVertical(main, -1) || !CheckMovingVertical(sub, -1)) return false;

            MoveTamaDown(main);
            MoveTamaDown(sub);
            return true;
        }
        /// <summary>
        /// たまのペアを上に動かす
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="y"></param>
        /// <returns>動かしたらtrue</returns>
        private bool MoveTamaPairUp(TamaData main, TamaData sub)
        {
            if (!CheckMovingVertical(main, 1) || !CheckMovingVertical(sub, 1)) return false;

            MoveTamaUp(main);
            MoveTamaUp(sub);
            return true;
        }
        /// <summary>
        /// 横方向に移動できるかどうかチェックする
        /// </summary>
        /// <param name="data"></param>
        /// <param name="moveX"></param>
        /// <returns></returns>
        private bool CheckMovingHorizontal(TamaData data, int moveX)
        {
            int newX = data.X + moveX;
            // 枠のチェック
            if ((newX < 0) || (newX >= setting.columnsCount)) return false;
            // フィールドのチェック
            if (GetTamaFromField(newX, data.Y) != null) return false;

            return true;
        }
        /// <summary>
        /// 縦方向に移動できるかチェック
        /// </summary>
        /// <param name="data"></param>
        /// <param name="moveY"></param>
        /// <returns></returns>
        private bool CheckMovingVertical(TamaData data, int moveY)
        {
            if (data == null)
            {
                return false;
            }

            int newY = data.Y + moveY;
            // 半分浮いているなら縦に移動可能
            if (data.Half) return true;
            // 枠のチェック
            if ((newY < 0) || (newY >= setting.rowsCount)) return false;
            // フィールドのチェック
            if (GetTamaFromField(data.X, newY) != null) return false;

            return true;
        }
        public void OnDpad(InputAction.CallbackContext context)
        {
        }
        /// <summary>
        /// ポーズ中かどうか
        /// </summary>
        public bool IsPaused { get; set; }
    }
}
