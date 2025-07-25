﻿using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    ///     Tutorial display menu behaviour.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialMenu : MonoBehaviour
    {
        private static TutorialMenu _instance;
        [SerializeField] private GameObject tutorialBox;
        [SerializeField] private TMP_Text tutorialTextbox;
        [SerializeField] private Animator animator;

        [SerializeField] [Min(0)] [Tooltip("Fade duration (sec)")]
        private float fadeDuration = 0.5f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeInCoroutine;
        private Coroutine _fadeOutCoroutine;

        /// <summary>
        ///     Access the 'singleton' instance of this object.
        /// </summary>
        public static TutorialMenu Instance
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<TutorialMenu>();
                return _instance;
            }
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        /// <summary>
        ///     Called on scene change: force hides the tutorial menu.
        /// </summary>
        /// <param name="current">Current scene</param>
        /// <param name="next">next scene</param>
        private void OnSceneChanged(Scene current, Scene next)
        {
            _canvasGroup.alpha = 0;
            tutorialBox.SetActive(false);
        }

        /// <summary>
        ///     Shows the tutorial with the given tutorial name (as defined in the animation controller).
        /// </summary>
        /// <param name="tutorialName">Tutorial name as specified in the animation controller</param>
        /// <param name="tutorialText">Text to display for tutorial move</param>
        /// <param name="tutorialPosition">Position to show the tutorial, relative to the center of the screen</param>
        public void ShowTutorial(Vector2 tutorialPosition, string tutorialName, string tutorialText = "")
        {
            int tutorialNameHash = Animator.StringToHash(tutorialName);
            tutorialBox.SetActive(true);
            // reading parameters requires the animator object to be active
            if (animator.parameters.All(parameter => parameter.nameHash != tutorialNameHash))
            {
                Debug.LogError("Invalid tutorial move set " + tutorialName);
                tutorialBox.SetActive(false);
                return;
            }

            tutorialBox.transform.localPosition = new Vector3(
                tutorialPosition.x,
                tutorialPosition.y,
                tutorialBox.transform.position.z
            );
            animator.SetTrigger(tutorialNameHash);
            tutorialTextbox.text = tutorialText.Replace("\n", "\n");

            if (_fadeInCoroutine != null)
                StopCoroutine(_fadeInCoroutine);
            _fadeInCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        ///     Hides the currently playing tutorial.
        /// </summary>
        public void HideTutorial()
        {
            if (_fadeOutCoroutine != null)
                StopCoroutine(_fadeOutCoroutine);
            _fadeOutCoroutine = StartCoroutine(FadeOut());
        }

        /// <summary>
        ///     Fades in the UI.
        /// </summary>
        /// <returns>Coroutine to slowly fade in the UI</returns>
        private IEnumerator FadeIn()
        {
            for (float elapsedTime = 0f; elapsedTime < fadeDuration; elapsedTime += Time.deltaTime)
            {
                _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = 1;
        }

        /// <summary>
        ///     Fades out the UI.
        /// </summary>
        /// <returns>Coroutine to slowly fade out the UI</returns>
        private IEnumerator FadeOut()
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, 1))
                yield return null;

            for (float elapsedTime = 0f; elapsedTime < fadeDuration; elapsedTime += Time.deltaTime)
            {
                _canvasGroup.alpha = Mathf.Clamp01(1f - elapsedTime / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = 0;
            tutorialBox.SetActive(false);
        }
    }
}
