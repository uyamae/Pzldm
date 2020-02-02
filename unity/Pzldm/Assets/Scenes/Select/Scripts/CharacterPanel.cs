using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public class CharacterPanel : MonoBehaviour
    {
        private Text text;
        private Image image;
        [SerializeField]
        private PlayFieldSetting setting;
        /// <summary>
        /// キャラクター名
        /// </summary>
        public string CharacterName
        {
            get { return (text == null) ? "" : text.text; }
            set
            {
                if (text != null)
                {
                    text.text = value;
                }
            }
        }
        private void Awake()
        {
            text = GetComponentInChildren<Text>(true);
            image = transform.Find("Button/Image")?.GetComponentInChildren<Image>(true);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// こうげきだまパターンデータから設定する
        /// </summary>
        /// <param name="data"></param>
        public void SetAttackPattern(AttackPatternData data)
        {
            CharacterName = data.CharacterName;
            if (image != null)
            {
                var texture = AttackPatternSampleGenerator.Generate(data, 80, 80, setting);
                image.sprite = Sprite.Create(texture, new Rect(0, 0, 80, 80), Vector2.zero);
            }
        }
    }
}
