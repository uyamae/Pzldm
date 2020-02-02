using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Pzldm
{
    public class SceneController : EditorWindow
    {
        [MenuItem("Window/Pzldm/SceneController")]
        public static void Create()
        {
            EditorWindow.GetWindow(typeof(SceneController));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("boot"))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
                EditorApplication.isPlaying = true;
            }
        }
    }
}
