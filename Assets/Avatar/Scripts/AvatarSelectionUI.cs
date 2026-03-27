using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Two-step avatar selection UI.
/// Step 1 — choose gender (Male / Female).
/// Step 2 — choose skin tone (Light / Black / Tan).
/// Saves choice to GameData, then transitions to Emotion Selection scene.
/// </summary>
public class AvatarSelectionUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject genderPanel;
    [SerializeField] private GameObject skinTonePanel;

    [Header("Gender Buttons")]
    [SerializeField] private Button maleButton;
    [SerializeField] private Button femaleButton;

    [Header("Skin-Tone Buttons")]
    [SerializeField] private Button lightButton;
    [SerializeField] private Button blackButton;
    [SerializeField] private Button tanButton;

    private GameData.Gender _selectedGender;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        // Auto-find panels
        if (genderPanel == null)
            genderPanel = transform.Find("GenderPanel")?.gameObject;
        if (skinTonePanel == null)
            skinTonePanel = transform.Find("SkinTonePanel")?.gameObject;

        // Auto-find buttons
        if (genderPanel != null)
        {
            if (maleButton   == null) maleButton   = FindButton(genderPanel, "MaleButton");
            if (femaleButton == null) femaleButton  = FindButton(genderPanel, "FemaleButton");
        }
        if (skinTonePanel != null)
        {
            if (lightButton == null) lightButton = FindButton(skinTonePanel, "LightButton");
            if (blackButton == null) blackButton = FindButton(skinTonePanel, "BlackButton");
            if (tanButton   == null) tanButton   = FindButton(skinTonePanel, "TanButton");
        }

        ShowGenderPanel();

        if (maleButton)   maleButton.onClick.AddListener(()   => OnGenderSelected(GameData.Gender.Male));
        if (femaleButton) femaleButton.onClick.AddListener(() => OnGenderSelected(GameData.Gender.Female));
        if (lightButton)  lightButton.onClick.AddListener(()  => OnSkinToneSelected(GameData.SkinTone.Light));
        if (blackButton)  blackButton.onClick.AddListener(()  => OnSkinToneSelected(GameData.SkinTone.Black));
        if (tanButton)    tanButton.onClick.AddListener(()    => OnSkinToneSelected(GameData.SkinTone.Tan));
    }

    private static Button FindButton(GameObject parent, string childName)
    {
        Transform t = parent.transform.Find(childName);
        return t != null ? t.GetComponent<Button>() : null;
    }

    private void OnGenderSelected(GameData.Gender gender)
    {
        _selectedGender = gender;
        genderPanel.SetActive(false);
        skinTonePanel.SetActive(true);
    }

    private void OnSkinToneSelected(GameData.SkinTone tone)
    {
        // Save to persistent data
        GameData.Instance.SelectedGender   = _selectedGender;
        GameData.Instance.SelectedSkinTone = tone;
        GameData.Instance.AvatarChosen     = true;

        // Go to emotion selection
        SceneFlow.GoToEmotionSelect();
    }

    private void ShowGenderPanel()
    {
        genderPanel.SetActive(true);
        skinTonePanel.SetActive(false);
    }
}
