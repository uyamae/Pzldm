using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public class BattleDebugBehaviour : MonoBehaviour
    {
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
            if (frame1p != null) frame1p.Label = "Frame";
            if (frame2p != null) frame2p.Label = "Frame";
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (frame1p != null && field1p != null)
            {
                frame1p.Count = field1p.PlayFrameCount;
            }
            if (frame2p != null && field2p != null)
            {
                frame2p.Count = field2p.PlayFrameCount;
            }
        }
    }
}
