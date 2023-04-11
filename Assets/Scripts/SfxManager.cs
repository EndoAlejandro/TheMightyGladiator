using CustomUtils;
using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : Singleton<SfxManager>
{
    [Header("Audio Mixer Group")]
    [SerializeField] private AudioMixerGroup masterGroup;

    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup fXGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioSource fxSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetMixerGroupVolume(masterGroup, SaveSystem.GetVolume(SaveSystem.PrefsField.Master));
        SetMixerGroupVolume(musicGroup, SaveSystem.GetVolume(SaveSystem.PrefsField.Music));
        SetMixerGroupVolume(fXGroup, SaveSystem.GetVolume(SaveSystem.PrefsField.Fx));
    }

    public void SetMixerGroupVolume(SaveSystem.PrefsField type, float value)
    {
        SaveSystem.SetVolume(type, value);
        switch (type)
        {
            case SaveSystem.PrefsField.Master:
                SetMixerGroupVolume(masterGroup, value);
                break;
            case SaveSystem.PrefsField.Music:
                SetMixerGroupVolume(musicGroup, value);
                break;
            case SaveSystem.PrefsField.Fx:
                SetMixerGroupVolume(fXGroup, value);
                break;
        }
    }

    public void SetMixerGroupVolume(AudioMixerGroup group, float value) =>
        group.audioMixer.SetFloat(group.name + "Volume", FromNormalizedToLog(value));

    private static float FromNormalizedToLog(float value) => Mathf.Log10(value / 10) * 20;

    public void PlayClip(AudioClip clip)
    {
        fxSource.Stop();
        fxSource.PlayOneShot(clip);
    }
}