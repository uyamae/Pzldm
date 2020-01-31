using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Pzldm
{
    public class ChainCount : MonoBehaviour
    {
        /// <summary>
        /// 表示に使うスプライト
        /// </summary>
        [SerializeField]
        private Sprite[] sprites;
        /// <summary>
        /// 表示する数値
        /// </summary>
        [SerializeField]
        private int count;
        /// <summary>
        /// 表示するイメージ
        /// </summary>
        private Image[,] digits;
        /// <summary>
        /// 表示する数を設定する
        /// </summary>
        /// <param name="value"></param>
        public void SetCount(int value)
        {
            if (count == value) return;
            count = value;
            if (count < 10)
            {
                digits[0, 0].sprite = sprites[count];
                digits[0, 0].gameObject.SetActive(true);
                digits[1, 0].gameObject.SetActive(false);
                digits[1, 1].gameObject.SetActive(false);
            }
            else
            {
                digits[1, 0].sprite = sprites[count % 10];
                digits[1, 1].sprite = sprites[count / 10];
                digits[0, 0].gameObject.SetActive(false);
                digits[1, 0].gameObject.SetActive(true);
                digits[1, 1].gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 表示秒数
        /// </summary>
        private float displaySeconds;
        /// <summary>
        /// 表示開始
        /// </summary>
        /// <param name="seconds"></param>
        public void StartDisplay(float seconds, int count)
        {
            SetCount(count);
            displaySeconds = seconds;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        // Start is called before the first frame update
        void Start()
        {
            digits = new Image[2,2];
            var images = GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                var m = Regex.Match(image.name, @"^d(?<N>\d)_(?<D>\d)$");
                if (m.Success)
                {
                    int n = int.Parse(m.Groups["N"].Value) - 1;
                    int d = int.Parse(m.Groups["D"].Value);
                    digits[n, d] = image;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            displaySeconds -= Time.deltaTime;
            if (displaySeconds <= 0)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
