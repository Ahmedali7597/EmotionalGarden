using UnityEngine;

/// <summary>
/// Spawns the avatar prefab based on GameData selection.
/// Used in the Garden scene — reads gender/skin from GameData.Instance.
/// </summary>
public class AvatarSpawner : MonoBehaviour
{
    [Header("Avatar Prefabs")]
    [SerializeField] private GameObject maleLightPrefab;
    [SerializeField] private GameObject maleBlackPrefab;
    [SerializeField] private GameObject maleTanPrefab;
    [SerializeField] private GameObject femaleLightPrefab;
    [SerializeField] private GameObject femaleBlackPrefab;
    [SerializeField] private GameObject femaleTanPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnCenter = Vector2.zero;

    private GameObject _currentAvatarGO;
    public AvatarController CurrentController { get; private set; }

    /// <summary>
    /// Spawn the avatar matching the current GameData selection.
    /// Returns the AvatarController, or null on failure.
    /// </summary>
    public AvatarController SpawnFromGameData()
    {
        if (GameData.Instance == null || !GameData.Instance.AvatarChosen)
        {
            Debug.LogError("[AvatarSpawner] No avatar selection in GameData.");
            return null;
        }

        if (_currentAvatarGO != null)
            Destroy(_currentAvatarGO);

        GameObject prefab = PickPrefab(GameData.Instance.SelectedGender,
                                       GameData.Instance.SelectedSkinTone);
        if (prefab == null)
        {
            Debug.LogError("[AvatarSpawner] No prefab for current selection.");
            return null;
        }

        _currentAvatarGO = Instantiate(prefab, spawnCenter, Quaternion.identity);
        CurrentController = _currentAvatarGO.GetComponent<AvatarController>();

        if (CurrentController == null)
            Debug.LogError("[AvatarSpawner] Prefab missing AvatarController.");

        return CurrentController;
    }

    private GameObject PickPrefab(GameData.Gender g, GameData.SkinTone t)
    {
        return (g, t) switch
        {
            (GameData.Gender.Male,   GameData.SkinTone.Light) => maleLightPrefab,
            (GameData.Gender.Male,   GameData.SkinTone.Black) => maleBlackPrefab,
            (GameData.Gender.Male,   GameData.SkinTone.Tan)   => maleTanPrefab,
            (GameData.Gender.Female, GameData.SkinTone.Light) => femaleLightPrefab,
            (GameData.Gender.Female, GameData.SkinTone.Black) => femaleBlackPrefab,
            (GameData.Gender.Female, GameData.SkinTone.Tan)   => femaleTanPrefab,
            _ => null
        };
    }
}
