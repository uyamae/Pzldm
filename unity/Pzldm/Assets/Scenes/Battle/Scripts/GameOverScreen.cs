using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Pzldm
{

    public class GameOverScreen : MonoBehaviour
    {
        public bool IsActive
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
            }
        }
        public enum ResultType
        {
            None,
            Continue,
            Exit,
        }
        public ResultType Result { get; set; }

        public void StartAskContinue()
        {
            Result = ResultType.None;
            IsActive = true;
        }
        // Start is called before the first frame update
        void Start()
        {
            Result = ResultType.None;
            var buttons = GetComponentsInChildren<Button>(true);
            buttons.FirstOrDefault<Button>(x => x.name == "btn_continue").onClick.AddListener(() => Result = ResultType.Continue);
            buttons.FirstOrDefault<Button>(x => x.name == "btn_exit").onClick.AddListener(() => Result = ResultType.Exit);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            GetComponentInChildren<Button>(true).Select();
        }
    }
}
