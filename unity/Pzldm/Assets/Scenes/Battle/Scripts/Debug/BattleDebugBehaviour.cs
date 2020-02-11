using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public class BattleDebugBehaviour : MonoBehaviour
    {
        public enum DisplayType
        {
            /// <summary>
            /// プレイフレーム数
            /// </summary>
            Frames,
            /// <summary>
            /// たま落下カウントダウン
            /// </summary>
            TamaFallCount,
            /// <summary>
            /// たま生成数
            /// </summary>
            TamaGeneratedCount,
        }
        [SerializeField]
        private DisplayType display;

        [SerializeField]
        private CounterBehaviour frame1p;
        [SerializeField]
        private CounterBehaviour frame2p;
        [SerializeField]
        private PlayField field1p;
        [SerializeField]
        private PlayField field2p;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            switch (display)
            {
                case DisplayType.Frames:
                    UpdateFrameCount();
                    break;
                case DisplayType.TamaFallCount:
                    UpdateTamaFallCount();
                    break;
                case DisplayType.TamaGeneratedCount:
                    UpdateTamaGeneratedCount();
                    break;
            }
        }
        private void UpdateFrameCount()
        {
            //if (frame1p != null) frame1p.Label = "Frame";
            //if (frame2p != null) frame2p.Label = "Frame";
            //if (frame1p != null && field1p != null)
            //{
            //    frame1p.Count = field1p.PlayFrameCount;
            //}
            //if (frame2p != null && field2p != null)
            //{
            //    frame2p.Count = field2p.PlayFrameCount;
            //}
            UpdateCounter(frame1p, field1p, "Frame", "PlayFrameCount");
            UpdateCounter(frame2p, field2p, "Frame", "PlayFrameCount");
        }
        private void UpdateTamaFallCount()
        {
            //if (frame1p != null) frame1p.Label = "FallFrame";
            //if (frame2p != null) frame2p.Label = "FallFrame";
            //if (frame1p != null && field1p != null)
            //{
            //    frame1p.Count = field1p.TamaFallFrame;
            //}
            //if (frame2p != null && field2p != null)
            //{
            //    frame2p.Count = field2p.TamaFallFrame;
            //}
            UpdateCounter(frame1p, field1p, "FallFrame", "TamaFallFrame");
            UpdateCounter(frame2p, field2p, "FallFrame", "TamaFallFrame");
        }
        private void UpdateTamaGeneratedCount()
        {
            UpdateCounter(frame1p, field1p, "GenTama", "TamaGeneratedCount");
            UpdateCounter(frame2p, field2p, "GenTama", "TamaGeneratedCount");
        }
        private void UpdateCounter(CounterBehaviour counter, PlayField playField, string label, string propName)
        {
            if (counter != null)
            {
                counter.Label = label;
                if (playField != null)
                {
                    var prop = typeof(PlayField).GetProperty(propName);
                    counter.Count = (int)prop.GetValue(playField);
                }
            }
        }

        private void OnEnable()
        {
            frame1p.IsActive = true;
            frame2p.IsActive = true;
        }
        private void OnDisable()
        {
            frame1p.IsActive = false;
            frame2p.IsActive = false;
        }
    }
}
