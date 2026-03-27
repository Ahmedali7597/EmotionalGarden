using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Emotion selection screen — user picks one of four emotions.
/// Saves to GameData, then transitions to MiniGame (placeholder) → Garden.
/// </summary>
public class EmotionSelectionUI : MonoBehaviour
{
    [Header("Emotion Buttons")]
    [SerializeField] private Button sadButton;
    [SerializeField] private Button calmButton;
    [SerializeField] private Button energeticButton;
    [SerializeField] private Button anxiousButton;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        if (sadButton == null)       sadButton       = FindButton("SadButton");
        if (calmButton == null)      calmButton      = FindButton("CalmButton");
        if (energeticButton == null) energeticButton = FindButton("EnergeticButton");
        if (anxiousButton == null)   anxiousButton   = FindButton("AnxiousButton");

        if (sadButton)       sadButton.onClick.AddListener(()       => OnEmotionSelected(GameData.Emotion.Sad));
        if (calmButton)      calmButton.onClick.AddListener(()      => OnEmotionSelected(GameData.Emotion.Calm));
        if (energeticButton) energeticButton.onClick.AddListener(() => OnEmotionSelected(GameData.Emotion.Energetic));
        if (anxiousButton)   anxiousButton.onClick.AddListener(()   => OnEmotionSelected(GameData.Emotion.Anxious));
    }

    private void OnEmotionSelected(GameData.Emotion emotion)
    {
        GameData.Instance.SelectedEmotion = emotion;
        GameData.Instance.EmotionChosen   = true;

        // Per design: after emotion selection a generic minigame starts.
        // For now go straight to Garden (placeholder).
        // When minigame scenes are ready, change to: SceneFlow.GoToMiniGame();
        SceneFlow.GoToGarden();
    }

    private static Button FindButton(string name)
    {
        var go = GameObject.Find(name);
        return go != null ? go.GetComponent<Button>() : null;
    }
}
