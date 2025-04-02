
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

[RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
public class CutsceneManager : MonoBehaviour
{
    private static AudioSource _source;
    [SerializeField] private string nextScene;

    private void Awake()
    {
        var allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        if (allAudioSources == null) return;
        foreach (var audioSource in allAudioSources)
            audioSource.Stop();
        
        _source = GetComponent<AudioSource>();
        _source.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) NextScene();
    }

    [YarnCommand("next_scene")]
    public void NextScene()
    {
        if (nextScene != "") SceneManager.LoadScene(nextScene);
    }

    [YarnCommand("play_sound")]
    public static IEnumerator PlaySound(string soundName, bool blockUntilDone = true)
    {
        if (_source.volume == 0)
            _source.volume = 1;
        
        _source.PlayOneShot(Resources.Load<AudioClip>(soundName));
        if (blockUntilDone)
            yield return new WaitUntil(() => !_source.isPlaying);
    }

    [YarnCommand("stop_sound")]
    public static IEnumerator StopSound(float fadeDuration = 0)
    {
        if (fadeDuration == 0)
        {
            _source.Stop();
            yield return null;
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                _source.volume = Mathf.Lerp(_source.volume, 0, elapsedTime / fadeDuration);
                yield return null;
            }
        }
    }
}
