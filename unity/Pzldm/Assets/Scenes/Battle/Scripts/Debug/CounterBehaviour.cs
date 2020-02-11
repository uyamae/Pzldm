using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public class CounterBehaviour : MonoBehaviour
    {
        private int count;

        [SerializeField]
        private Text label;
        [SerializeField]
        private Text number;
        /// <summary>
        /// ラベル文字列
        /// </summary>
        public string Label
        {
            get { return label?.text; }
            set
            {
                if (label != null)
                {
                    label.text = value;
                }
            }
        }
        /// <summary>
        /// 値
        /// </summary>
        public int Count
        {
            get { return count; }
            set
            {
                if (count != value)
                {
                    count = value;
                    if (number != null)
                    {
                        number.text = $"{count}";
                    }
                }
            }
        }
        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsActive
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
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
