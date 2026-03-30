using UnityEditor;
using UnityEngine;

public class CheckBuildScenes
{
    public static void Execute()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            Debug.Log($"Scene in Build Settings: {scene.path}");
        }
    }
}
