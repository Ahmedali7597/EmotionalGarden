using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach this to the SettingsCanvas GameObject.
/// It wires up the buttons and sliders to SettingsUI static methods at runtime.
/// The canvas starts hidden (inactive) and is shown/hidden by SettingsUI.Toggle().
/// 
/// Wiring happens in OnEnable so it works when the canvas is activated by SettingsUI.Show().
/// </summary>
public class SettingsCanvasWiring : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider gameSoundSlider;
    [SerializeField] private Slider bgSoundSlider;

    [Header("Buttons")]
    [SerializeField] private Button reselectEmotionBtn;
    [SerializeField] private Button mainGardenBtn;
    [SerializeField] private Button resumeBtn;

    private bool hasWired = false;

    private void OnEnable()
    {
        // Wire listeners only once
        if (!hasWired)
        {
            WireListeners();
            hasWired = true;
        }

        // Every time the canvas is shown, handle garden-specific layout
        ApplyGardenLayout();
    }

    private void WireListeners()
    {
        Debug.Log("[SettingsCanvasWiring] Wiring listeners...");

        // Wire up slider listeners
        if (gameSoundSlider != null)
        {
            gameSoundSlider.onValueChanged.AddListener(SettingsUI.OnGameVolumeChanged);
            Debug.Log("[SettingsCanvasWiring] GameSoundSlider wired.");
        }
        else
        {
            Debug.LogWarning("[SettingsCanvasWiring] gameSoundSlider is null!");
        }

        if (bgSoundSlider != null)
        {
            bgSoundSlider.onValueChanged.AddListener(SettingsUI.OnBGVolumeChanged);
            Debug.Log("[SettingsCanvasWiring] BGSoundSlider wired.");
        }
        else
        {
            Debug.LogWarning("[SettingsCanvasWiring] bgSoundSlider is null!");
        }

        // Wire up button listeners
        if (reselectEmotionBtn != null)
        {
            reselectEmotionBtn.onClick.AddListener(() =>
            {
                Debug.Log("[SettingsCanvasWiring] Reselect Emotion clicked!");
                SettingsUI.OnReselectEmotion();
            });
            Debug.Log("[SettingsCanvasWiring] ReselectEmotionBtn wired.");
        }
        else
        {
            Debug.LogWarning("[SettingsCanvasWiring] reselectEmotionBtn is null!");
        }

        if (mainGardenBtn != null)
        {
            mainGardenBtn.onClick.AddListener(() =>
            {
                Debug.Log("[SettingsCanvasWiring] Main Garden clicked!");
                SettingsUI.OnMainGarden();
            });
            Debug.Log("[SettingsCanvasWiring] MainGardenBtn wired.");
        }
        else
        {
            Debug.LogWarning("[SettingsCanvasWiring] mainGardenBtn is null!");
        }

        if (resumeBtn != null)
        {
            resumeBtn.onClick.AddListener(() =>
            {
                Debug.Log("[SettingsCanvasWiring] Resume clicked!");
                SettingsUI.OnResume();
            });
            Debug.Log("[SettingsCanvasWiring] ResumeBtn wired.");
        }
        else
        {
            Debug.LogWarning("[SettingsCanvasWiring] resumeBtn is null!");
        }

        Debug.Log("[SettingsCanvasWiring] All listeners wired.");
    }

    /// <summary>
    /// In the garden scene, hide the Main Garden button and move Resume up to its position.
    /// In minigame scenes, show all buttons at their default positions.
    /// </summary>
    private void ApplyGardenLayout()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        bool inGarden = sceneName == SceneFlow.GardenScene || sceneName == "GardenScene";

        if (mainGardenBtn != null)
        {
            mainGardenBtn.gameObject.SetActive(!inGarden);
        }

        // Move Resume button up to Main Garden button's position when in garden
        if (resumeBtn != null && mainGardenBtn != null && inGarden)
        {
            RectTransform resumeRect = resumeBtn.GetComponent<RectTransform>();
            RectTransform gardenRect = mainGardenBtn.GetComponent<RectTransform>();
            resumeRect.anchoredPosition = gardenRect.anchoredPosition;
        }
    }
}
