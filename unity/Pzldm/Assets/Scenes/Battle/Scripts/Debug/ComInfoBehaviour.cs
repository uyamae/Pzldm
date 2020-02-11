using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public class ComInfoBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Text stateText;
        [SerializeField]
        private Text mainText;
        [SerializeField]
        private Text subText;
        /// <summary>
        /// ステート
        /// </summary>
        public string State
        {
            get { return stateText?.text; }
            set
            {
                if ((stateText != null) && (stateText.text != value))
                {
                    stateText.text = value;
                }
            }
        }
        /// <summary>
        /// メインたま目標位置
        /// </summary>
        public string Main
        {
            get { return mainText?.text; }
            set
            {
                if ((mainText != null) && (mainText.text != value))
                {
                    mainText.text = value;
                }
            }
        }
        /// <summary>
        /// サブたまの目標位置
        /// </summary>
        public string Sub
        {
            get { return subText?.text; }
            set
            {
                if ((subText != null) && (subText.text != value))
                {
                    subText.text = value;
                }
            }
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
