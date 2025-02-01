using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    
    // Start is called before the first frame update
    private void Start()
    {
        var savedBgmVolume = PlayerPrefs.GetFloat("BGM");
        var savedSfxVolume = PlayerPrefs.GetFloat("SFX");
        bgmSlider.value = savedBgmVolume;
        sfxSlider.value = savedSfxVolume;
    }
}
