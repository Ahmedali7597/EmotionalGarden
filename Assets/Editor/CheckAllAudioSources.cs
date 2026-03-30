using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CheckAllAudioSources
{
    public static void Execute()
    {
        string[] scenes = new string[]
        {
            "Assets/Minigame/CloudGame/CloudGame.unity",
            "Assets/Minigame/DarkGame/DarkGame.unity",
            "Assets/Minigame/PuzzleGame/PuzzleGame.unity",
            "Assets/Minigame/RainGame/RainGame.unity",
            "Assets/Minigame/RunGame/RunGame.unity",
            "Assets/Main Garden/Scenes/GardenScene.unity"
        };

        foreach (string scenePath in scenes)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in sources)
            {
                Debug.Log($"[AudioCheck] Scene: {scene.name}, AudioSource on: {source.gameObject.name}, loop: {source.loop}");
            }
        }
    }
}
