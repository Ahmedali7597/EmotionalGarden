using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Start screen — shows a Start button (and placeholder Settings button).
/// Start → Avatar Selection scene.
/// </summary>
public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        if (startButton == null)
            startButton = FindButton("StartButton");
        if (settingsButton == null)
            settingsButton = FindButton("SettingsButton");

        if (startButton)    startButton.onClick.AddListener(OnStartClicked);
        if (settingsButton) settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnStartClicked()
    {
        SceneFlow.GoToAvatarSelect();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("[StartScene] Settings placeholder — not yet implemented.");
    }

    private Button FindButton(string name)
    {
        var go = GameObject.Find(name);
        return go != null ? go.GetComponent<Button>() : null;
    }
}
