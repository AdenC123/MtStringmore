using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance ??= FindObjectOfType<SoundManager>();

        [Header("Audio Control")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private float startBgmVolume = 0.5f;
        [SerializeField] private float startSfxVolume = 0.5f;
        [SerializeField] private float startMasterVolume = 0.5f;

        [Header("Collectable SFX")]
        [SerializeField] private AudioClip[] collectableClips;
        [SerializeField, Tooltip("Max time between collections to continue combo (in seconds)")]
        private float collectableComboWindow = 1f;

        private float _timeSinceLastCollect = Mathf.Infinity;
        private int _audioIndex = 0;
        private AudioSource _collectableAudioSource;

        private void Awake()
        {
            if (Instance != this) Destroy(gameObject);

            _collectableAudioSource = gameObject.AddComponent<AudioSource>();
            _collectableAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            float savedMasterVolume = PlayerPrefs.GetFloat("Master", startMasterVolume);
            float savedBgmVolume = PlayerPrefs.GetFloat("BGM", startBgmVolume);
            float savedSfxVolume = PlayerPrefs.GetFloat("SFX", startSfxVolume);
            SetMasterVolume(savedMasterVolume);
            SetBgmVolume(savedBgmVolume);
            SetSfxVolume(savedSfxVolume);
        }

        private void Update()
        {
            _timeSinceLastCollect += Time.unscaledDeltaTime;
        }

        public static float SliderToVolume(float sliderValue)
        {
            return Mathf.Log10(sliderValue) * 20;
        }

        public void SetMasterVolume(float volume)
        {
            masterSlider.value = volume;
            PlayerPrefs.SetFloat("Master", volume);
            PlayerPrefs.Save();
        }

        public void SetBgmVolume(float volume)
        {
            audioMixer.SetFloat("BGM", SliderToVolume(volume));
            bgmSlider.value = volume;
            PlayerPrefs.SetFloat("BGM", volume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float volume)
        {
            audioMixer.SetFloat("SFX", SliderToVolume(volume));
            sfxSlider.value = volume;
            PlayerPrefs.SetFloat("SFX", volume);
            PlayerPrefs.Save();
        }

        public void SetMute(bool isMuted)
        {
            audioMixer.SetFloat("Master",
                isMuted ? -80f : SliderToVolume(PlayerPrefs.GetFloat("Master", startMasterVolume)));
        }

        /// <summary>
        /// Plays a collectable sound in sequence with combo logic using PlayOneShot.
        /// </summary>
        public void PlayCollectableComboSound()
        {
            if (collectableClips == null || collectableClips.Length == 0) return;

            if (_timeSinceLastCollect > collectableComboWindow)
            {
                _audioIndex = 0;
            }
            else
            {
                if (_audioIndex >= collectableClips.Length - 1)
                    _audioIndex = collectableClips.Length - 1;
                else
                    _audioIndex++;

            }

            _collectableAudioSource.PlayOneShot(collectableClips[_audioIndex]);
            _timeSinceLastCollect = 0f;
        }
    }
}
