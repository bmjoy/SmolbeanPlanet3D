using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class SoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public class SoundClipSpec
    {
        public string name;

        public AudioClip clip;

        public bool loop = false;

        [Range(0f, 1f)]
        public float volume = 1f;

        public bool ignoreGamePause = false;
    }

    public SoundClipSpec[] clips;
    public AudioMixerGroup mixerGroup;

    private Dictionary<string, AudioSource> players;
    private Dictionary<string, SoundClipSpec> clipLookup;

    void Awake()
    {
        players = new Dictionary<string, AudioSource>();
        clipLookup = clips.ToDictionary(c => c.name, c => c);

        if(!mixerGroup)
        {
            Debug.LogWarning("No AudioMixer assigned to " + name);
        }
    }

    private AudioSource CreateAudioSource(SoundClipSpec clip)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clip.clip;
        audioSource.loop = clip.loop;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 60f;
        audioSource.volume = clip.volume;

        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.bypassEffects = true;

        audioSource.ignoreListenerPause = clip.ignoreGamePause;

        players.Add(clip.name, audioSource);
        return audioSource;
    }

    private AudioSource CreateAudioSource(string clipName)
    {
        if(clipLookup.TryGetValue(clipName, out var spec))
            return CreateAudioSource(spec);
        
        return null;
    }

    public void Play(string clipName)
    {
        AudioSource player = GetPlayer(clipName);
        if(player)
        {
            player.Play();
        }
    }

    private AudioSource GetPlayer(string clipName)
    {
        AudioSource player;
        if (!players.TryGetValue(clipName, out player))
        {
            player = CreateAudioSource(clipName);
        }

        return player;
    }

    public bool IsPlaying(string clipName)
    {
        var player = GetPlayer(clipName);
        return player == null ? false : player.isPlaying;
    }

    public void PlayOneShot(string clipName)
    {
        if(players.TryGetValue(clipName, out var player))
        {
            AudioSource.PlayClipAtPoint(player.clip, transform.position, player.volume);
        }
    }

    public void Stop(string clipName)
    {
        if(players.TryGetValue(clipName, out var player))
        {
            player.Stop();
        }
    }
}
