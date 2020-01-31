using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pzldm
{

    public class SelectCharacterMenu : MonoBehaviour
    {
        [SerializeField]
        private Text titleText;
        /// <summary>
        /// タイトルテキスト
        /// </summary>
        public string TitleText
        {
            get { return (titleText == null) ? string.Empty : titleText.text; }
            set { if (titleText != null) titleText.text = value;}
        }
        [SerializeField]
        private GridLayoutGroup grid;
        [SerializeField]
        private CharacterPanel characterPanelPrefab;
        // Start is called before the first frame update
        void Start()
        {
            if (titleText == null)
            {
                titleText = GetComponentInChildren<Text>(true);
            }
            if (grid == null)
            {
                grid = GetComponentInChildren<GridLayoutGroup>(true);
            }
        }

        public CharacterPanel Add(AttackPatternData data, UnityAction listener)
        {
            var n = data ;
            var panel = GameObject.Instantiate<CharacterPanel>(characterPanelPrefab);
            panel.name = n.CharacterName;
            panel.CharacterName = n.CharacterName;
            panel.transform.SetParent(grid.transform);
            var button = panel.GetComponentInChildren<Button>();
            button.onClick.AddListener(listener);
            return panel;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
