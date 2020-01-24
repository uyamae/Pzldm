using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Pzldm
{
    [CreateAssetMenu(menuName = "Pzldm/AttackEditorSetting")]
    public class AttackEditorSetting : ScriptableObject
    {
        [SerializeField]
        private SpriteAtlas atlas;
        [SerializeField]
        private PlayFieldSetting setting;
        [SerializeField]
        private SpriteRenderer tamaPrefab;
    }
}
