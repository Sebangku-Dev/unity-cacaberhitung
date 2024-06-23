using UnityEngine;

public class AudioSystem : SingletonPersistent<AudioSystem>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private bool isLooping;
    [SerializeField] private bool isPlayOnStart;

    public bool IsMute
    {
        get { return musicSource.mute; }
        set { IsMute = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        if (isPlayOnStart && !IsMute) PlayMusic();
    }

    public void PlayMusic()
    {
        musicSource.Play();
        musicSource.loop = isLooping;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.loop = isLooping;
    }

    public void SetMute(bool isMute) => musicSource.mute = isMute;

}