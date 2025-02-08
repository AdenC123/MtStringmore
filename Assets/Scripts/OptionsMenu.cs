using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    private void Start()
    {
        var savedMasterVolume = PlayerPrefs.GetFloat("Master");
        var savedBgmVolume = PlayerPrefs.GetFloat("BGM");
        var savedSfxVolume = PlayerPrefs.GetFloat("SFX");
        MasterSlider.value = savedMasterVolume;
        bgmSlider.value = savedBgmVolume;
        sfxSlider.value = savedSfxVolume;

        MasterSlider.onValueChanged.AddListener(delegate
        {
            SoundManager.Instance.SetMasterVolume(MasterSlider.value);
            audioMixer.SetFloat("Master", SoundManager.SliderToVolume(MasterSlider.value));
        });
        bgmSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.SetBgmVolume(bgmSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.SetSfxVolume(sfxSlider.value); });
    }
}