using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider volumeSlider;

    public void SetMusicVolume()
    {
        float volume = volumeSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume)*20);
    }
}
