using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Start screen — shows a Start button only.
/// Start → Avatar Selection scene.
/// </summary>
public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        if (startButton == null)
            startButton = FindButton("StartButton");

        if (startButton) startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        SceneFlow.GoToAvatarSelect();
    }

    private Button FindButton(string name)
    {
        var go = GameObject.Find(name);
        return go != null ? go.GetComponent<Button>() : null;
    }
}
