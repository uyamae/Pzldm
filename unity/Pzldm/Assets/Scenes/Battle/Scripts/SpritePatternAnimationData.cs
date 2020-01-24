using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paldm
{
    [CreateAssetMenu(menuName = "Pzldm/SpritePatternAnimationData")]
    public class SpritePatternAnimationData : ScriptableObject
    {
        public SpritePattern[] patterns;
    }
}
