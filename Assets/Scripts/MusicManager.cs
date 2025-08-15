using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> musicTracks = new List<AudioClip>();
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopPlaylist = true;

    [Header("Volume Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private float fadeTime = 1f;

    [Header("Shuffle Settings")]
    [SerializeField] private bool shuffleEnabled = false;

    // Private variables
    private List<int> shuffledIndices = new List<int>();
    private int currentTrackIndex = 0;
    private int currentShuffleIndex = 0;
    private bool isPlaying = false;
    private Coroutine fadeCoroutine;

    // Events
    public System.Action<AudioClip, int> OnTrackChanged;
    public System.Action OnPlaylistFinished;

    void Start()
    {
        // Get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Create AudioSource if still null
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize settings
        audioSource.volume = volume;
        audioSource.loop = false; // We handle looping manually

        // Initialize shuffle if enabled
        if (shuffleEnabled)
            InitializeShuffle();

        // Start playing if enabled
        if (playOnStart && musicTracks.Count > 0)
            PlayCurrentTrack();
    }

    void Update()
    {
        // Check if current track finished playing
        if (isPlaying && !audioSource.isPlaying)
        {
            NextTrack();
        }
    }

    #region Public Methods

    /// <summary>
    /// Play the current track
    /// </summary>
    public void Play()
    {
        if (musicTracks.Count > 0 && audioSource.clip != null)
        {
            audioSource.Play();
            isPlaying = true;
        }
    }

    /// <summary>
    /// Pause the current track
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
        isPlaying = false;
    }

    /// <summary>
    /// Stop the current track
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
    }

    /// <summary>
    /// Play the next track in the playlist
    /// </summary>
    public void NextTrack()
    {
        if (musicTracks.Count == 0) return;

        if (shuffleEnabled)
        {
            currentShuffleIndex++;
            if (currentShuffleIndex >= shuffledIndices.Count)
            {
                if (loopPlaylist)
                {
                    InitializeShuffle(); // Reshuffle for next loop
                    currentShuffleIndex = 0;
                }
                else
                {
                    OnPlaylistFinished?.Invoke();
                    return;
                }
            }
            currentTrackIndex = shuffledIndices[currentShuffleIndex];
        }
        else
        {
            currentTrackIndex++;
            if (currentTrackIndex >= musicTracks.Count)
            {
                if (loopPlaylist)
                {
                    currentTrackIndex = 0;
                }
                else
                {
                    OnPlaylistFinished?.Invoke();
                    return;
                }
            }
        }

        PlayCurrentTrack();
    }

    /// <summary>
    /// Play the previous track in the playlist
    /// </summary>
    public void PreviousTrack()
    {
        if (musicTracks.Count == 0) return;

        if (shuffleEnabled)
        {
            currentShuffleIndex--;
            if (currentShuffleIndex < 0)
            {
                currentShuffleIndex = shuffledIndices.Count - 1;
            }
            currentTrackIndex = shuffledIndices[currentShuffleIndex];
        }
        else
        {
            currentTrackIndex--;
            if (currentTrackIndex < 0)
            {
                currentTrackIndex = musicTracks.Count - 1;
            }
        }

        PlayCurrentTrack();
    }

    /// <summary>
    /// Jump to a specific track by index
    /// </summary>
    public void PlayTrack(int index)
    {
        if (index < 0 || index >= musicTracks.Count) return;

        currentTrackIndex = index;

        // Update shuffle index if shuffle is enabled
        if (shuffleEnabled)
        {
            currentShuffleIndex = shuffledIndices.IndexOf(index);
        }

        PlayCurrentTrack();
    }

    /// <summary>
    /// Toggle shuffle mode
    /// </summary>
    public void ToggleShuffle()
    {
        shuffleEnabled = !shuffleEnabled;

        if (shuffleEnabled)
        {
            InitializeShuffle();
            // Find current track in shuffle list
            currentShuffleIndex = shuffledIndices.IndexOf(currentTrackIndex);
        }
    }

    /// <summary>
    /// Set volume with optional fade
    /// </summary>
    public void SetVolume(float newVolume, bool fade = false)
    {
        volume = Mathf.Clamp01(newVolume);

        if (fade)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(audioSource.volume, volume));
        }
        else
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// Add a new track to the playlist
    /// </summary>
    public void AddTrack(AudioClip clip)
    {
        if (clip != null)
        {
            musicTracks.Add(clip);
            if (shuffleEnabled)
                InitializeShuffle();
        }
    }

    /// <summary>
    /// Remove a track from the playlist
    /// </summary>
    public void RemoveTrack(int index)
    {
        if (index >= 0 && index < musicTracks.Count)
        {
            musicTracks.RemoveAt(index);
            if (shuffleEnabled)
                InitializeShuffle();
        }
    }

    #endregion

    #region Private Methods

    private void PlayCurrentTrack()
    {
        if (currentTrackIndex >= 0 && currentTrackIndex < musicTracks.Count)
        {
            audioSource.clip = musicTracks[currentTrackIndex];
            audioSource.Play();
            isPlaying = true;

            OnTrackChanged?.Invoke(musicTracks[currentTrackIndex], currentTrackIndex);
        }
    }

    private void InitializeShuffle()
    {
        shuffledIndices.Clear();

        // Fill with indices
        for (int i = 0; i < musicTracks.Count; i++)
        {
            shuffledIndices.Add(i);
        }

        // Shuffle using Fisher-Yates algorithm
        for (int i = shuffledIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[randomIndex];
            shuffledIndices[randomIndex] = temp;
        }

        currentShuffleIndex = 0;
    }

    private IEnumerator FadeVolume(float startVolume, float targetVolume)
    {
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;
        fadeCoroutine = null;
    }

    #endregion

    #region Properties

    public bool IsPlaying => isPlaying && audioSource.isPlaying;
    public bool IsShuffleEnabled => shuffleEnabled;
    public int CurrentTrackIndex => currentTrackIndex;
    public AudioClip CurrentTrack => currentTrackIndex >= 0 && currentTrackIndex < musicTracks.Count ? musicTracks[currentTrackIndex] : null;
    public int TrackCount => musicTracks.Count;
    public float Volume => volume;

    /// <summary>
    /// Get the previous track that will be played when PreviousTrack() is called
    /// </summary>
    public AudioClip PreviousTrackClip
    {
        get
        {
            if (musicTracks.Count == 0) return null;

            int previousIndex;

            if (shuffleEnabled)
            {
                int previousShuffleIndex = currentShuffleIndex - 1;
                if (previousShuffleIndex < 0)
                    previousShuffleIndex = shuffledIndices.Count - 1;
                previousIndex = shuffledIndices[previousShuffleIndex];
            }
            else
            {
                previousIndex = currentTrackIndex - 1;
                if (previousIndex < 0)
                    previousIndex = musicTracks.Count - 1;
            }

            return previousIndex >= 0 && previousIndex < musicTracks.Count ? musicTracks[previousIndex] : null;
        }
    }

    /// <summary>
    /// Get the next track that will be played when NextTrack() is called
    /// </summary>
    public AudioClip NextTrackClip
    {
        get
        {
            if (musicTracks.Count == 0) return null;

            int nextIndex;

            if (shuffleEnabled)
            {
                int nextShuffleIndex = currentShuffleIndex + 1;
                if (nextShuffleIndex >= shuffledIndices.Count)
                {
                    if (loopPlaylist)
                        nextShuffleIndex = 0;
                    else
                        return null; // No next track if not looping
                }
                nextIndex = shuffledIndices[nextShuffleIndex];
            }
            else
            {
                nextIndex = currentTrackIndex + 1;
                if (nextIndex >= musicTracks.Count)
                {
                    if (loopPlaylist)
                        nextIndex = 0;
                    else
                        return null; // No next track if not looping
                }
            }

            return nextIndex >= 0 && nextIndex < musicTracks.Count ? musicTracks[nextIndex] : null;
        }
    }

    /// <summary>
    /// Get the index of the previous track in the original playlist
    /// </summary>
    public int PreviousTrackIndex
    {
        get
        {
            if (musicTracks.Count == 0) return -1;

            if (shuffleEnabled)
            {
                int previousShuffleIndex = currentShuffleIndex - 1;
                if (previousShuffleIndex < 0)
                    previousShuffleIndex = shuffledIndices.Count - 1;
                return shuffledIndices[previousShuffleIndex];
            }
            else
            {
                int previousIndex = currentTrackIndex - 1;
                if (previousIndex < 0)
                    previousIndex = musicTracks.Count - 1;
                return previousIndex;
            }
        }
    }

    /// <summary>
    /// Get the index of the next track in the original playlist
    /// </summary>
    public int NextTrackIndex
    {
        get
        {
            if (musicTracks.Count == 0) return -1;

            if (shuffleEnabled)
            {
                int nextShuffleIndex = currentShuffleIndex + 1;
                if (nextShuffleIndex >= shuffledIndices.Count)
                {
                    if (loopPlaylist)
                        nextShuffleIndex = 0;
                    else
                        return -1; // No next track if not looping
                }
                return shuffledIndices[nextShuffleIndex];
            }
            else
            {
                int nextIndex = currentTrackIndex + 1;
                if (nextIndex >= musicTracks.Count)
                {
                    if (loopPlaylist)
                        nextIndex = 0;
                    else
                        return -1; // No next track if not looping
                }
                return nextIndex;
            }
        }
    }

    #endregion
}