using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    [CreateAssetMenu(menuName = "Pzldm/AttackPatternPreset")]
    public class AttackPatternPreset : ScriptableObject
    {
        [System.Serializable]
        public class Preset
        {
            public string CharacterName;

            public AttackPatternData.DropOrderPresetType DropOrder;

            public AttackPatternData.ColorLineType ColorLine;

            public AttackPatternData.ColorOrderType ColorOrder;
            
            public AttackPatternData.LiftingColorLineType LiftingColorLine;

            public AttackPatternData.AttackDirectionType[] DirectionPattern;
        }

        public Preset[] presets;
    }
}
