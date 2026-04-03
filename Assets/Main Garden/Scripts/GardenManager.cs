using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main Garden scene controller.
/// - Spawns avatar from GameData
/// - Swaps sky background SpriteRenderers based on selected emotion
/// - Sets avatar sorting order above plants
/// - Manages minigame UI button (top-right, appears on random timer)
/// </summary>
public class GardenManager : MonoBehaviour
{
    [Header("Sky Backgrounds (SpriteRenderer GameObjects)")]
    [SerializeField] private GameObject calmBackground;
    [SerializeField] private GameObject sadBackground;
    [SerializeField] private GameObject anxiousBackground;
    [SerializeField] private GameObject energeticBackground;

    [Header("Avatar Spawning")]
    [SerializeField] private AvatarSpawner avatarSpawner;

    [Header("Movement")]
    [SerializeField] private AvatarMovementController movementController;
    [SerializeField] private AvatarRandomWander randomWander;
    [SerializeField] private Collider2D boundary;

    [Header("Avatar Sorting")]
    [Tooltip("Sorting order for the avatar sprite (plants default to 0).")]
    [SerializeField] private int avatarSortingOrder = 10;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        // Apply sky background based on selected emotion
        ApplySkyBackground();

        // Spawn avatar and wire up movement
        SpawnAndWireAvatar();
    }

    /// <summary>
    /// Show only the sky background matching the selected emotion.
    /// </summary>
    private void ApplySkyBackground()
    {
        GameData.Emotion emotion = GameData.Emotion.Calm;
        if (GameData.Instance != null && GameData.Instance.EmotionChosen)
        {
            emotion = GameData.Instance.SelectedEmotion;
        }

        // Disable all backgrounds first
        if (calmBackground != null) calmBackground.SetActive(false);
        if (sadBackground != null) sadBackground.SetActive(false);
        if (anxiousBackground != null) anxiousBackground.SetActive(false);
        if (energeticBackground != null) energeticBackground.SetActive(false);

        // Enable the matching one
        switch (emotion)
        {
            case GameData.Emotion.Sad:
                if (sadBackground != null) sadBackground.SetActive(true);
                break;
            case GameData.Emotion.Anxious:
                if (anxiousBackground != null) anxiousBackground.SetActive(true);
                break;
            case GameData.Emotion.Energetic:
                if (energeticBackground != null) energeticBackground.SetActive(true);
                break;
            case GameData.Emotion.Calm:
            default:
                if (calmBackground != null) calmBackground.SetActive(true);
                break;
        }

        Debug.Log($"[GardenManager] Sky background set to: {emotion}");
    }

    /// <summary>
    /// Spawn the avatar from GameData and wire up movement controller + wander.
    /// </summary>
    private void SpawnAndWireAvatar()
    {
        if (avatarSpawner == null)
        {
            Debug.LogWarning("[GardenManager] No AvatarSpawner assigned.");
            return;
        }

        AvatarController avatar = avatarSpawner.SpawnFromGameData();
        if (avatar == null)
        {
            Debug.LogWarning("[GardenManager] Avatar spawn failed.");
            return;
        }

        // Set avatar scale to 2
        avatar.transform.localScale = new Vector3(2f, 2f, 1f);

        // Set avatar sorting order above plants
        var sr = avatar.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = avatarSortingOrder;
        }

        // Wire garden bounds to avatar
        if (boundary != null)
        {
            avatar.SetGardenBounds(boundary);
        }

        // Wire movement controller
        if (movementController != null)
        {
            movementController.SetAvatar(avatar);
            movementController.SetBoundary(boundary);
        }

        // Wire random wander
        if (randomWander != null)
        {
            randomWander.SetAvatar(avatar);
            if (movementController != null)
            {
                movementController.SetWander(randomWander);
            }
        }

        Debug.Log("[GardenManager] Avatar spawned and wired up successfully.");
    }
}
