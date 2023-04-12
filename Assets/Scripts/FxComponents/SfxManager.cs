using System.Collections.Generic;
using CustomUtils;
using UnityEngine;
using UnityEngine.Audio;

namespace FxComponents
{
    public class SfxManager : Singleton<SfxManager>
    {
        [SerializeField] private PooledSoundFx pooledSoundPrefab;

        [Header("Audio Mixer Group")]
        [SerializeField] private AudioMixerGroup masterGroup;

        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup fXGroup;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;

        [SerializeField] private AudioSource fxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip swordSlashClip;

        [SerializeField] private AudioClip shieldHitClip;
        [SerializeField] private AudioClip dashClip;
        [SerializeField] private AudioClip playerHitClip;
        [SerializeField] private AudioClip playerDeathClip;
        [SerializeField] private AudioClip enemyHitClip;
        [SerializeField] private AudioClip bulletShotClip;
        [SerializeField] private AudioClip bulletHitClip;
        [SerializeField] private AudioClip mortarShotClip;
        [SerializeField] private AudioClip mortarHitClip;
        [SerializeField] private AudioClip enemyDeathClip;
        [SerializeField] private AudioClip jumpStartClip;
        [SerializeField] private AudioClip jumpEndClip;
        [SerializeField] private AudioClip laserClip;
        [SerializeField] private AudioClip healClip;
        [SerializeField] private AudioClip upgradeClip;
        [SerializeField] private AudioClip upgradeSpawnClip;
        [SerializeField] private AudioClip uiClick;

        private Dictionary<Sfx, AudioClip> _listedSfx;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            _listedSfx = new Dictionary<Sfx, AudioClip>
            {
                { Sfx.SwordSlash, swordSlashClip },
                { Sfx.ShieldHit, shieldHitClip },
                { Sfx.Dash, dashClip },
                { Sfx.PlayerHit, playerHitClip },
                { Sfx.PlayerDeath, playerDeathClip },
                { Sfx.EnemyHit, enemyHitClip },
                { Sfx.BulletShot, bulletShotClip },
                { Sfx.BulletHit, bulletHitClip },
                { Sfx.MortarShot, mortarShotClip },
                { Sfx.MortarHit, mortarHitClip },
                { Sfx.EnemyDeath, enemyDeathClip },
                { Sfx.JumpStart, jumpStartClip },
                { Sfx.JumpEnd, jumpEndClip },
                { Sfx.Laser, laserClip },
                { Sfx.Heal, healClip },
                { Sfx.Upgrade, upgradeClip },
                { Sfx.UpgradeSpawn, upgradeSpawnClip },
            };
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

        public void PlayFx(Sfx sfx, Vector3 position)
        {
            var sound = pooledSoundPrefab.Get<PooledSoundFx>(position, Quaternion.identity);
            sound.Setup(_listedSfx[sfx]);
        }

        public void PlayUI() => fxSource.PlayOneShot(uiClick);

        public void PlayMusic(AudioClip clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}