using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public class CharacterPanel : MonoBehaviour
    {
        private Text text;
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
            text = GetComponentInChildren<Text>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
