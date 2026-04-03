using UnityEngine;

/// <summary>
/// Manages emotion-based background music in the Garden scene.
/// Loads the appropriate music clip from Resources based on the selected emotion
/// and plays it on loop. Respects the SettingsUI background volume.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GardenMusicManager : MonoBehaviour
{
    [Header("Music Clips (assign in Inspector)")]
    [SerializeField] private AudioClip calmMusic;
    [SerializeField] private AudioClip sadMusic;
    [SerializeField] private AudioClip anxiousMusic;
    [SerializeField] private AudioClip energeticMusic;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayEmotionMusic();
    }

    /// <summary>
    /// Selects and plays the music clip matching the current emotion from GameData.
    /// </summary>
    public void PlayEmotionMusic()
    {
        GameData.Emotion emotion = GameData.Emotion.Calm;
        if (GameData.Instance != null && GameData.Instance.EmotionChosen)
        {
            emotion = GameData.Instance.SelectedEmotion;
        }

        AudioClip clip = emotion switch
        {
            GameData.Emotion.Sad       => sadMusic,
            GameData.Emotion.Anxious   => anxiousMusic,
            GameData.Emotion.Energetic => energeticMusic,
            _                          => calmMusic
        };

        if (clip == null)
        {
            Debug.LogWarning($"[GardenMusicManager] No music clip assigned for emotion: {emotion}");
            return;
        }

        // Apply saved background volume from SettingsUI
        _audioSource.volume = SettingsUI.GetBGVolume();

        // If already playing the same clip, don't restart
        if (_audioSource.clip == clip && _audioSource.isPlaying)
            return;

        _audioSource.clip = clip;
        _audioSource.Play();

        Debug.Log($"[GardenMusicManager] Playing {emotion} music: {clip.name}");
    }
}
