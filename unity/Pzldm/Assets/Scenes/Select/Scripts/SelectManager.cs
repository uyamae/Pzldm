using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

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

        [SerializeField]
        private InputSystemUIInputModule uiInputModule;
        [SerializeField]
        private InputActionAsset input1P;
        [SerializeField]
        private InputActionAsset input2P;

        private Sprite[] attackPatternSprites;

        // Start is called before the first frame update
        void Start()
        {
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
                    new StateMachine<SelectState>.State()
                    {
                        Enter = Select1pCharacterEnter,
                        Update = SelectCharacterUpdate,
                        Leave = SelectCharacterLeave,
                    },
                    // Select2pCharacter
                    new StateMachine<SelectState>.State()
                    {
                        Enter = Select2pCharacterEnter,
                        Update = SelectCharacterUpdate,
                        Leave = SelectCharacterLeave,
                    },
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
            playingMode = PlayingModeType.VersusPlay;
            stateMachine.ChangeState(SelectState.Select1pCharacter);
        }
        #endregion
        #region キャラクター選択
        private void Select1pCharacterEnter()
        {
            selectCharacterMenu.gameObject.SetActive(true);
            uiInputModule.actionsAsset = input1P;
            SetCharacterSelectMenuTitleText();
        }
        private void Select2pCharacterEnter()
        {
            selectCharacterMenu.gameObject.SetActive(true);
            uiInputModule.actionsAsset = (playingMode == PlayingModeType.VersusPlay) ? input2P : input1P;
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
            var mgr = PzldmManager.Instance;
            if (mgr == null) return;

            attackPatternSprites = new Sprite[mgr.AttackPatterns.Length];
            for (int i = 0; i < mgr.AttackPatterns.Length; ++i)
            {
                int index = i;
                var panel = selectCharacterMenu.Add(mgr.AttackPatterns[i], () => CallbackSelectCharacter(index));
                if (i == 0)
                {
                    panel.GetComponentInChildren<Selectable>(true).Select();
                }
                attackPatternSprites[i] = panel?.GetAttackPatternSprite();
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
            var mgr = PzldmManager.Instance;
            if (stateMachine.CurrentState == SelectState.Select1pCharacter)
            {
                mgr.SetPlayerAttackPattern(0, mgr.AttackPatterns[index], attackPatternSprites[index]);
                stateMachine.ChangeState(SelectState.Select2pCharacter);
            }
            else if (stateMachine.CurrentState == SelectState.Select2pCharacter)
            {
                mgr.PlayingMode = playingMode;
                mgr.SetPlayerAttackPattern(1, mgr.AttackPatterns[index], attackPatternSprites[index]);
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
