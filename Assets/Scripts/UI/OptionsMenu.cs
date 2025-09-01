using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Main behaviour for the options menu.
    /// </summary>
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle timerToggle;
        [SerializeField] private TextMeshProUGUI versionNumber;

        private void Start()
        {
            float savedMasterVolume = PlayerPrefs.GetFloat("Master");
            float savedBgmVolume = PlayerPrefs.GetFloat("BGM");
            float savedSfxVolume = PlayerPrefs.GetFloat("SFX");
            int savedSpeedToggle = PlayerPrefs.GetInt("SpeedTime");

            masterSlider.value = savedMasterVolume;
            bgmSlider.value = savedBgmVolume;
            sfxSlider.value = savedSfxVolume;
            timerToggle.isOn = savedSpeedToggle == 1;
   
            versionNumber.text = Application.version;
        }
       
    }
}
