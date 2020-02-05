using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pzldm
{
    /// <summary>
    /// 対戦モード
    /// </summary>
    public enum PlayingModeType
    {
        /// <summary>
        /// ひとりプレイ
        /// </summary>
        SinglePlay,
        /// <summary>
        /// ２人対戦プレイ
        /// </summary>
        VersusPlay,
    }
    /// <summary>
    /// 常駐して管理
    /// </summary>
    public class PzldmManager : MonoBehaviour
    {
        public static PzldmManager theInstance = null;
        /// <summary>
        /// PzldmManager のインスタンス
        /// </summary>
        public static PzldmManager Instance
        {
            get
            {
                if (theInstance == null)
                {
                    theInstance = GameObject.Find("PzldmManager")?.GetComponent<PzldmManager>();
                }
                return theInstance;
            }
        }


        public enum GameMode
        {
            Boot,
            Select,
            Battle,
        }

        StateMachine<GameMode> stateMachine;

        [SerializeField]
        private AttackPatternData[] selectedPatterns;
        /// <summary>
        /// 選択されたこうげきだまパターンの画像
        /// </summary>
        private Sprite[] selectedPatternSprites;
        /// <summary>
        /// 選択されたこうげきだまパターンを設定
        /// </summary>
        /// <param name="playerNo"></param>
        /// <param name="data"></param>
        public void SetPlayerAttackPattern(int playerNo, AttackPatternData data, Sprite sprite)
        {
            if ((playerNo < 0) || (playerNo >= selectedPatterns.Length)) return;

            selectedPatterns[playerNo] = data;
            selectedPatternSprites[playerNo] = sprite;
        }
        /// <summary>
        /// 選択されたこうげきだまパターンを取得
        /// </summary>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public AttackPatternData GetPlayerAttackPattern(int playerNo)
        {
            if (playerNo < 0 || playerNo >= selectedPatterns.Length) return null;
            return selectedPatterns[playerNo];
        }
        /// <summary>
        /// 選択されたこうげきだまパターンのスプライトを取得
        /// </summary>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public Sprite GetPlayerAttackPatternSprite(int playerNo)
        {
            if (playerNo < 0 || playerNo >= selectedPatterns.Length) return null;
            return selectedPatternSprites[playerNo];
        }

        [SerializeField]
        private AttackPatternData[] attackPatterns;

        /// <summary>
        /// こうげきだまパターン配列
        /// </summary>
        public AttackPatternData[] AttackPatterns { get { return attackPatterns; } }

        [SerializeField]
        private PlayingModeType playingMode;
        /// <summary>
        /// プレイモード
        /// </summary>
        public PlayingModeType PlayingMode
        {
            get { return playingMode; }
            set { playingMode = value; }
        }
        /// <summary>
        /// たま生成用の乱数シード初期値(起動時に設定)
        /// </summary>
        public int RandomSeed { get; set; }
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        // Start is called before the first frame update
        void Start()
        {
            stateMachine = new StateMachine<GameMode>(
                new StateMachine<GameMode>.State[] {
                    // Boot
                    new StateMachine<GameMode>.State()
                    {
                        Update = BootUpdate
                    },
                    // Select
                    new StateMachine<GameMode>.State()
                    {
                        Enter = SelectEnter,
                        Update = SelectUpdate,
                    },
                    // Battle
                    new StateMachine<GameMode>.State()
                    {
                        Enter = BattleEnter,
                        Update = BattleUpdate,
                    }
                }
                );
            selectedPatternSprites = new Sprite[selectedPatterns.Length];
        }

        private void BootUpdate()
        {
            ResetRandomSeed();
            SceneManager.LoadScene("select");
            stateMachine.ChangeState(GameMode.Select);

        }
        public void ResetRandomSeed()
        {
            // 現在時刻を乱数シードに
            var now = System.DateTime.Now;
            int seed = 0;
            seed = seed * 365 + now.Month;
            seed = seed * 31 + now.Day;
            seed = seed * 24 + now.Hour;
            seed = seed * 60 + now.Minute;
            seed = seed * 60 + now.Second;
            RandomSeed = seed * 1000 + now.Millisecond;
        }
        private void SelectEnter()
        {

        }
        private void SelectUpdate()
        {
            
        }
        private void BattleEnter()
        {

        }
        private void BattleUpdate()
        {

        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.UpdateState();
        }
    }
}
