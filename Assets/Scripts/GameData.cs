using UnityEngine;

/// <summary>
/// Persistent game state that survives scene loads.
/// Stores avatar selection and current emotion.
/// Access via GameData.Instance from any scene.
/// </summary>
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    // ── Avatar ────────────────────────────────────────────────────────
    public enum Gender   { Male, Female }
    public enum SkinTone { Light, Black, Tan }

    public Gender   SelectedGender   { get; set; } = Gender.Male;
    public SkinTone SelectedSkinTone { get; set; } = SkinTone.Light;
    public bool     AvatarChosen     { get; set; }

    // ── Emotion ───────────────────────────────────────────────────────
    public enum Emotion { Sad, Calm, Energetic, Anxious }

    public Emotion SelectedEmotion { get; set; } = Emotion.Calm;
    public bool    EmotionChosen   { get; set; }

    // ── Prefab path helper ────────────────────────────────────────────
    /// <summary>Returns the prefab path for the selected avatar.</summary>
    public string GetAvatarPrefabName()
    {
        string genderStr = SelectedGender == Gender.Male ? "Male" : "Female";
        string toneStr = SelectedSkinTone switch
        {
            SkinTone.Light => "Light",
            SkinTone.Black => "Black",
            SkinTone.Tan   => "Tan",
            _              => "Light"
        };
        return $"{genderStr}_{toneStr}";
    }

    // ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
