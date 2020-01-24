using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pzldm
{
    public class AttackEditorWindow : EditorWindow
    {
        private AttackEditorSetting setting;

        //private PlayFieldSetting playFieldSetting;
        class RowCol
        {
            public int columnsCount;
            public int rowsCount;
            public int patternRowsCount;
        }
        private RowCol playFieldSetting;

        private Texture2D[] tamaSprites;

        private AttackPatternData pattern;

        private AttackPatternPreset preset;

        private ColorType[,] dropPattern;

        private ColorType[,] liftingPattern;

        private string[] presetNames;

        private bool isAutoSave;
        private string assetPath;

        private bool usePreset;
        private int presetIndex;

        private int ColumnsCount { get { return (playFieldSetting == null) ? 6 : playFieldSetting.columnsCount; } }
        private int RowsCount { get { return (playFieldSetting == null) ? 15 : playFieldSetting.rowsCount; } }
        private int PatternRowsCount { get { return (playFieldSetting == null) ? 10 : playFieldSetting.patternRowsCount; } }

        [MenuItem("Window/Pzldm/AttackEditor")]
        public static void Create()
        {
            EditorWindow.GetWindow<AttackEditorWindow>();
        }
        private void Awake()
        {
            setting = Resources.Load<AttackEditorSetting>("AttackEditorSetting");
            //playFieldSetting = AssetDatabase.LoadAssetAtPath<PlayFieldSetting>("Assets/Scenes/Battle/Scripts/Setting.asset");

            tamaSprites = new Texture2D[4];
            tamaSprites[(int)ColorType.Red] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scenes/Battle/Datas/s01.png");
            tamaSprites[(int)ColorType.Blue] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scenes/Battle/Datas/s02.png");
            tamaSprites[(int)ColorType.Green] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scenes/Battle/Datas/s03.png");
            tamaSprites[(int)ColorType.Yellow] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scenes/Battle/Datas/s04.png");

            preset = Resources.Load<AttackPatternPreset>("AttackPatternPreset");
            if (preset != null)
            {
                presetNames = new string[preset.presets.Length];
                for (int i = 0; i < preset.presets.Length; ++i)
                {
                    presetNames[i] = preset.presets[i].CharacterName;        
                }
            }

            dropPattern = new ColorType[RowsCount, ColumnsCount];

        }
        public void OnGUI()
        {
            AssetField();
            if (pattern == null)
            {
                CreateNewPattern();
            }
            else
            {
                bool changed = false;
                EditorGUILayout.LabelField(assetPath);
                changed |= EditCharacterName();
                SetUsingPreset();
                if (usePreset)
                {
                    changed |= SelectPreset();
                }
                else
                {
                    changed |= TamaSelectButton();
                }
                SaveButton(changed);
                DrawAttackPattern();
            }
        }
        /// <summary>
        /// 編集するアセットを設定する
        /// </summary>
        private void AssetField()
        {
            var p = EditorGUILayout.ObjectField("パターンデータ", pattern, typeof(AttackPatternData), false) as AttackPatternData;
            if (pattern != p)
            {
                assetPath = AssetDatabase.GetAssetPath(p);
                pattern = p;
            }
        }
        /// <summary>
        /// パターンを新規作成
        /// </summary>
        private void CreateNewPattern()
        {
            EditorGUILayout.LabelField("アセットを指定するか新規作成してください");
            assetPath = EditorGUILayout.TextField("アセットパス", assetPath);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(assetPath) && (pattern == null));
            if (GUILayout.Button("作成"))
            {
                pattern = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AttackPatternData)) as AttackPatternData;
                if (pattern == null)
                {
                    pattern = new AttackPatternData();
                    AssetDatabase.CreateAsset(pattern, assetPath);
                    EditorUtility.SetDirty(pattern);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        /// <summary>
        /// キャラクター名を編集
        /// </summary>
        /// <returns></returns>
        private bool EditCharacterName()
        {
            bool changed = false;
            string s = EditorGUILayout.TextField("キャラ名", pattern.CharacterName);
            if (s != pattern.CharacterName)
            {
                changed = true;
                pattern.CharacterName = s;
            }
            return changed;
        }
        /// <summary>
        /// 保存ボタン
        /// </summary>
        /// <param name="changed"></param>
        private void SaveButton(bool changed)
        {
            EditorGUILayout.BeginHorizontal();
            isAutoSave = EditorGUILayout.Toggle("AutoSave", isAutoSave);
            EditorGUI.BeginDisabledGroup(isAutoSave);
            if (GUILayout.Button("Save") || (changed && isAutoSave))
            {
                EditorUtility.SetDirty(pattern);
                AssetDatabase.SaveAssets();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 編集時のたま選択ボタン
        /// </summary>
        /// <returns></returns>
        private bool TamaSelectButton()
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            foreach (Texture2D s in tamaSprites)
            {
                GUIContent c = new GUIContent();
                c.image = s;
                GUILayout.Button(c, GUILayout.Height(24), GUILayout.Width(24));
            }
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        /// <summary>
        /// プリセットを使うかどうか選択
        /// </summary>
        private void SetUsingPreset()
        {
            usePreset = EditorGUILayout.Toggle("プリセットで設定", usePreset);
        }
        /// <summary>
        /// プリセットの選択
        /// </summary>
        /// <returns></returns>
        private bool SelectPreset()
        {
            bool changed = false;
            var index = EditorGUILayout.Popup("プリセット", presetIndex, presetNames);
            if (index != presetIndex)
            {
                changed = true;
                presetIndex = index;
            }
            bool disabled = (pattern == null);
            EditorGUI.BeginDisabledGroup(disabled);
            if (GUILayout.Button("プリセット設定"))
            {
                pattern.SetupWithPreset(preset.presets[presetIndex], ColumnsCount, RowsCount, PatternRowsCount);
                UpdateDropPattern();
            }
            EditorGUI.EndDisabledGroup();
            return changed;
        }
        /// <summary>
        /// 落下こうげきだまパターン更新
        /// </summary>
        private void UpdateDropPattern()
        {
            if (pattern == null) return;
            AttackPatternData.AttackTamaData[] datas = pattern.DropAttackTamaPattern;
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                dropPattern[data.Position.y, data.Position.x] = data.Color;
            }
            Repaint();
        }
        /// <summary>
        /// こうげきだまパターンを描画
        /// </summary>
        private void DrawDropPattern(float ox, float oy, float size, string label)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            {
                var rect = new Rect(ox, oy, size * ColumnsCount, size);
                EditorGUI.LabelField(rect, label, style);
            }
            oy += size;
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow};
            var datas = pattern.DropAttackTamaPattern;
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                var color = colors[(int)data.Color];
                var x = data.Position.x;
                var y = RowsCount - 1 - data.Position.y;
                var rect = new Rect(ox + size * x + 1, oy + size * y + 1, size - 2, size - 2);

                EditorGUI.DrawRect(rect, color);
                EditorGUI.LabelField(rect, (i + 1).ToString(), style);
            }
        }
        private void DrawLiftPattern(float ox, float oy, float size, string label)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            {
                var rect = new Rect(ox, oy, size * ColumnsCount, size);
                EditorGUI.LabelField(rect, label, style);
            }
            oy += size;
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow};
            var datas = pattern.LiftAttackTamaPattern;
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                var color = colors[(int)data.Color];
                var x = data.Position.x;
                var y = data.Position.y;
                var rect = new Rect(ox + size * x + 1, oy + size * y + 1, size - 2, size - 2);

                EditorGUI.DrawRect(rect, color);
                EditorGUI.LabelField(rect, (i + 1).ToString(), style);
            }
        }
        private void DrawAttackPattern()
        {
            if (pattern == null) return;
            
            Rect rect = EditorGUILayout.GetControlRect();
            float size = 24;
            DrawDropPattern(rect.x, rect.y, size, "落下パターン");
            if (pattern.HasLifting)
            {
                DrawLiftPattern(rect.x + size * (ColumnsCount + 1), rect.y, size, "せり上げパターン");
            }
        }
    }
}
