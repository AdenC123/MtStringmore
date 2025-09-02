using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using Util;
using Random = System.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Interactables.LetterBlock
{
    /// <summary>
    /// Choose letter block color, handle shake and break animation
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LetterBlockVisual : MonoBehaviour
    {
        [SerializeField] private GameObject letter;
        [SerializeField] private GameObject particles;
        [SerializeField] private SpriteRenderer[] childRenderers;
        [SerializeField] [Min(0)] private float blockBreakDelay;
        [SerializeField] [Range(0f, 0.1f)] private float delayBetweenShakes = 0.01f;
        [SerializeField] [Range(0f, 2f)] private float distance = 0.1f;
        [SerializeField] private CollectableSpriteInfo[] possibleSprites;

        private SpriteRenderer _renderer;

        private void OnValidate()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            GameManager.Instance.Reset += OnReset;
        }

        /// <summary>
        /// Called on reset: hides the particles and enables the renderers.
        /// </summary>
        private void OnReset()
        {
            particles.SetActive(false);
            _renderer.enabled = true;
            letter.SetActive(true);
            foreach (SpriteRenderer childRenderer in childRenderers) childRenderer.enabled = true;
        }

        private void OnDisable()
        {
            GameManager.Instance.Reset -= OnReset;
        }

        public void Crack()
        {
            particles.SetActive(true);
            StartCoroutine(RandomUtil.RandomJitterRoutine(transform, blockBreakDelay, delayBetweenShakes, distance));
            StartCoroutine(Break());
        }

        private IEnumerator Break()
        {
            // Hide block and letter after half the block break time
            yield return new WaitForSeconds(blockBreakDelay / 2);
            _renderer.enabled = false;
            letter.SetActive(false);
            // Give time for particles to spawn, then destroy object and children
            yield return new WaitForSeconds(blockBreakDelay / 2f);
            particles.SetActive(false);
            foreach (SpriteRenderer childRenderer in childRenderers) childRenderer.enabled = false;
        }
        
        /// <summary>
        /// DO NOT call this outside of the editor!
        /// Randomizes sprite and letter.
        /// </summary>
        public void RandomizeSprite()
        {
#if UNITY_EDITOR
            CollectableSpriteInfo info = RandomUtil.SelectRandom(possibleSprites);
            _renderer.sprite = info.sprite;
            char randomChar = (char) new Random().Next(65, 91);  // uppercase ascii from A to Z
            TextMeshPro textMesh = letter.GetComponent<TextMeshPro>();
            textMesh.text = $"{randomChar}";
            textMesh.color = info.letterColor;
            EditorUtility.SetDirty(textMesh);
#endif
        }
        
        [Serializable]
        private struct CollectableSpriteInfo
        {
            public Sprite sprite;
            public Color letterColor;
        }
    }
}
