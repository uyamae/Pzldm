using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pzldm
{
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

        [SerializeField]
        private AttackPatternData[] attackPatterns;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
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
