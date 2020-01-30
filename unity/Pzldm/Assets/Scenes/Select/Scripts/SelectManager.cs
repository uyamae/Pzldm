using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pzldm
{
    /// <summary>
    /// セレクト管理
    /// </summary>
    public class SelectManager : MonoBehaviour
    {
        enum SelectState
        {
            Init,
            SelectPlayer,
            Select1pCharacter,
            Select2pCharacter,
        }
        private StateMachine<SelectState> stateMachine;

        private PlayingModeType playingMode;
        [SerializeField]
        private GameObject playingModeMenu;
        [SerializeField]
        private SelectCharacterMenu selectCharacterMenu;

        // Start is called before the first frame update
        void Start()
        {
            var selectCharacterState = new StateMachine<SelectState>.State()
                    {
                        Enter = SelectCharacterEnter,
                        Update = SelectCharacterUpdate,
                        Leave = SelectCharacterLeave,
                    };

            stateMachine = new StateMachine<SelectState>(
                new StateMachine<SelectState>.State[] {
                    // Init
                    new StateMachine<SelectState>.State()
                    {
                        Enter = InitEnter,
                        Update = InitUpdate,
                    },
                    // SelectPlayer
                    new StateMachine<SelectState>.State()
                    {
                        Enter = SelectPlayerEnter,
                        Update = SelectPlayerUpdate,
                        Leave = SelectPlayerLeave,
                    },
                    // Select1pCharacter
                    selectCharacterState,
                    // Select2pCharacter
                    selectCharacterState,
                }
            );
            InitSelectPlayerMenu();
            InitCharacterPanels();
        }
        #region 初期化
        private void InitEnter()
        {
            playingMode = PlayingModeType.SinglePlay;
            playingModeMenu.SetActive(false);
            selectCharacterMenu.gameObject.SetActive(false);
        }
        private void InitUpdate()
        {
            stateMachine.ChangeState(SelectState.SelectPlayer);
        }
        #endregion
        #region プレイヤー選択
        private void SelectPlayerEnter()
        {
            playingModeMenu.SetActive(true);
        }
        private void SelectPlayerUpdate()
        {

        }
        private void SelectPlayerLeave()
        {
            playingModeMenu.SetActive(false);
        }
        private void InitSelectPlayerMenu()
        {
            var menu = playingModeMenu;
            var buttons = menu.GetComponentsInChildren<Button>();
            foreach (var btn in buttons)
            {
                if (btn.name == "btn_single")
                {
                    btn.onClick.AddListener(CallbackSinlePlayButton);
                }
                else if (btn.name == "btn_versus")
                {
                    btn.onClick.AddListener(CallbackVersusPlayButton);
                }
            }
        }
        public void CallbackSinlePlayButton()
        {
            playingMode = PlayingModeType.SinglePlay;
            stateMachine.ChangeState(SelectState.Select1pCharacter);
        }
        public void CallbackVersusPlayButton()
        {
            playingMode = PlayingModeType.VirsusPlay;
            stateMachine.ChangeState(SelectState.Select1pCharacter);
        }
        #endregion
        #region キャラクター選択
        private void SelectCharacterEnter()
        {
            selectCharacterMenu.gameObject.SetActive(true);
            SetCharacterSelectMenuTitleText();
        }
        private void SelectCharacterUpdate()
        {

        }
        private void SelectCharacterLeave()
        {
            selectCharacterMenu.gameObject.SetActive(false);
        }

        private void InitCharacterPanels()
        {
            if (selectCharacterMenu == null) return;
            //if (characterPanelPrefab == null) return;
            var mgr = GameObject.Find("PzldmManager")?.GetComponent<PzldmManager>();
            if (mgr == null) return;
            for (int i = 0; i < mgr.AttackPatterns.Length; ++i)
            {
                int index = i;
                selectCharacterMenu.Add(mgr.AttackPatterns[i], () => CallbackSelectCharacter(index));
            }
        }
        private void SetCharacterSelectMenuTitleText()
        {
            if (stateMachine.CurrentState == SelectState.Select1pCharacter)
            {
                if (playingMode == PlayingModeType.SinglePlay)
                {
                    selectCharacterMenu.TitleText = "Select Your Character";
                }
                else
                {
                    selectCharacterMenu.TitleText = "Select 1P Character";
                }
            }
            else
            {
                if (playingMode == PlayingModeType.SinglePlay)
                {
                    selectCharacterMenu.TitleText = "Select Opponent Character";
                }
                else
                {
                    selectCharacterMenu.TitleText = "Select 2P Character";
                }
            }
        }
        private void CallbackSelectCharacter(int index)
        {
            var mgr = GameObject.Find("PzldmManager")?.GetComponent<PzldmManager>();
            if (stateMachine.CurrentState == SelectState.Select1pCharacter)
            {
                mgr.SetPlayerAttackPattern(0, mgr.AttackPatterns[index]);
                stateMachine.ChangeState(SelectState.Select2pCharacter);
            }
            else if (stateMachine.CurrentState == SelectState.Select2pCharacter)
            {
                mgr.PlayingMode = playingMode;
                mgr.SetPlayerAttackPattern(1, mgr.AttackPatterns[index]);
                SceneManager.LoadScene("battle");
            }
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            stateMachine.UpdateState();
        }
    }
}
