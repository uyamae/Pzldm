using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    /// <summary>
    /// 開始演出
    /// </summary>
    public class ReadyFightDirection : MonoBehaviour
    {
        private Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
        }
        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// 演出開始
        /// </summary>
        public void StartDirection()
        {
            animator.SetBool("is_playing", true);
            animator.SetBool("go_to_fight", false);
        }
        /// <summary>
        /// 再生中かどうか
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return animator?.GetBool("is_playing") == true;
            }
        }
    }
}
