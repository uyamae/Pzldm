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
        VirsusPlay,
    }
    /// <summary>
    /// 常駐して管理
    /// </summary>
    public class PzldmManager : MonoBehaviour
    {
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
        /// 選択されたこうげきだまパターンを設定
        /// </summary>
        /// <param name="playerNo"></param>
        /// <param name="data"></param>
        public void SetPlayerAttackPattern(int playerNo, AttackPatternData data)
        {
            if (playerNo < 0 || playerNo >= selectedPatterns.Length) return;

            selectedPatterns[playerNo] = data;
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
        }

        private void BootUpdate()
        {
            SceneManager.LoadScene("select");
            stateMachine.ChangeState(GameMode.Select);
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
