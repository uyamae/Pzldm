using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pzldm
{
    public class HelpScreen : MonoBehaviour
    {
        [SerializeField]
        private Text helpTextUi;

        [SerializeField]
        private TextAsset helpText;

        [SerializeField]
        private Button helpButton;

        // Start is called before the first frame update
        void Start()
        {
            helpTextUi.text = helpText.text;
            helpButton.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            bool active = !helpTextUi.transform.parent.gameObject.activeSelf;
            helpButton.GetComponentInChildren<Text>().text = active ? "×" : "?";
            helpTextUi.transform.parent.gameObject.SetActive(active);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
