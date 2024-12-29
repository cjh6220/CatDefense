using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class StartSceneChecker
{
#if UNITY_EDITOR
    static StartSceneChecker()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            //string introSceneName = "Intro";
            string loadingSceneName = "LoadingScene";
            //string loginSceneName = "Login";
            string battleSceneName = "BattleScene";
            //string startSceneName = "Start"; // Start 씬의 이름

            string currentSceneName = SceneManager.GetActiveScene().name;

            // 현재 씬이 Login 씬이거나 Main 씬인 경우
            if (currentSceneName == battleSceneName)// || currentSceneName == loadingSceneName)
            {
                // Start 씬을 로드
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene("Assets/Scenes/" + loadingSceneName + ".unity");
                }
            }
        }
    }
#endif
}