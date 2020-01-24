using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public partial class PlayField
    {
        public Text debugText;
        public Text stateName;
        /// <summary>
        /// ステート名
        /// </summary>
        public string StateName
        {
            get { return stateName?.text; }
            set
            {
                if (stateName != null)
                {
                    stateName.text = value;
                }
            }
        }

        public void SetDebugText(string text)
        {
            if (debugText == null) return;

            debugText.text = text;
        }
        public void AddDebugText(string text)
        {
            if (debugText == null) return;

            debugText.text = debugText.text + text;
        }
    }
}
