using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pzldm
{
    /// <summary>
    /// 数字を表示する
    /// </summary>
    public class NumberDisplay : MonoBehaviour
    {
        private int number;

        private SpriteRenderer[] renderers;

        [SerializeField]
        private Sprite[] numSprites;
        [SerializeField]
        private Texture colorTexture;

        public int Number
        {
            get { return number; }
            set
            {
                if (number == value) return;
                number = value;
                ApplyNumber();
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>().Where(x => x.name.StartsWith("n")).ToArray();
            System.Array.Sort<SpriteRenderer>(renderers, (a, b) => string.Compare(a.name, b.name));
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 数値を表示に反映させる
        /// </summary>
        public void ApplyNumber()
        {
            int n = number;
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].sprite = numSprites[n % 10];
                n /= 10;
            }
        }
    }
}
