using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CheckAudioSources
{
    public static void Execute()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/Minigame/CloudGame/CloudGame.unity", OpenSceneMode.Single);
        AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            Debug.Log($"AudioSource found on: {source.gameObject.name}, clip: {(source.clip != null ? source.clip.name : "null")}, loop: {source.loop}");
        }
    }
}
