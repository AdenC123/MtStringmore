using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Applies fade effects to a game object
    /// </summary>
    public class FadeEffects : MonoBehaviour
    {
        [SerializeField, Min(0)] private float fadeDuration = 0.3f;

        [Tooltip("Delay between fading in and fading out, if applicable")] [SerializeField]
        private bool deactivateOnFade;

        [SerializeField] private bool destroyOnFade;
        public Action FadeIn;

        private IFadeEffectHandler _fadeEffectHandler;

        private void Awake()
        {
            if (TryGetComponent(out Image image))
            {
                _fadeEffectHandler = new ImageFadeHandler(image);
            }
            else if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                _fadeEffectHandler = new SpriteFadeHandler(spriteRenderer);
            }
            else if (TryGetComponent(out Renderer componentRenderer))
            {
                _fadeEffectHandler = new FallbackEffectHandler(componentRenderer);
            }
            else
            {
                Debug.LogWarning("Fade effect handler not found");
            }
        }

        public void InvokeFadeOut()
        {
            StartCoroutine(FadeOutCoroutine());
        }

        public void InvokeFadeIn()
        {
            StartCoroutine(FadeInCoroutine());
        }

        public void InvokeFadeInAndOut()
        {
            StartCoroutine(FadeInAndOutCoroutine());
        }

        private IEnumerator FadeOutCoroutine()
        {
            for (float elapsedTime = 0; elapsedTime < fadeDuration; elapsedTime += Time.deltaTime)
            {
                _fadeEffectHandler?.SetAlpha(1.0f - elapsedTime / fadeDuration);
                yield return null;
            }

            _fadeEffectHandler?.SetAlpha(0);
            if (deactivateOnFade)
                gameObject.SetActive(false);
            if (destroyOnFade)
                Destroy(gameObject);
        }

        private IEnumerator FadeInCoroutine()
        {
            for (float elapsedTime = 0; elapsedTime < fadeDuration; elapsedTime += Time.deltaTime)
            {
                _fadeEffectHandler?.SetAlpha(elapsedTime / fadeDuration);
                yield return null;
            }

            _fadeEffectHandler?.SetAlpha(1);
        }

        private IEnumerator FadeInAndOutCoroutine()
        {
            yield return FadeInCoroutine();
            FadeIn?.Invoke();
            yield return FadeOutCoroutine();
        }

        /// <summary>
        /// Handles changing the alpha of sprite renderers.
        /// </summary>
        private class SpriteFadeHandler : IFadeEffectHandler
        {
            private readonly SpriteRenderer _spriteRenderer;
            private readonly float _startAlpha;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="spriteRenderer">Sprite renderer to change alpha of</param>
            public SpriteFadeHandler(SpriteRenderer spriteRenderer)
            {
                _spriteRenderer = spriteRenderer;
                _startAlpha = _spriteRenderer.color.a;
            }

            public void SetAlpha(float alpha)
            {
                _spriteRenderer.color = IFadeEffectHandler.CreateColor(_spriteRenderer.color, _startAlpha * alpha);
            }
        }

        /// <summary>
        /// Handler to fade out a UI image.
        /// </summary>
        private class ImageFadeHandler : IFadeEffectHandler
        {
            private readonly Image _image;
            private readonly float _startAlpha;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="image">Image to set the alpha of</param>
            public ImageFadeHandler(Image image)
            {
                _image = image;
                _startAlpha = _image.color.a;
            }

            /// <inheritdoc />
            public void SetAlpha(float alpha)
            {
                _image.color = IFadeEffectHandler.CreateColor(_image.color, alpha * _startAlpha);
            }
        }

        /// <summary>
        /// Fallback handler that creates a new material and modifies the alpha.
        /// <p/>
        /// Ideally shouldn't be used as the creation of materials prevents draw call batching.
        /// </summary>
        private class FallbackEffectHandler : IFadeEffectHandler
        {
            private readonly Material _material;
            private readonly float _startAlpha;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="renderer">Renderer to get material of</param>
            public FallbackEffectHandler(Renderer renderer)
            {
                _material = renderer.material;
                _startAlpha = _material.color.a;
            }

            /// <inheritdoc />
            public void SetAlpha(float alpha)
            {
                _material.color = IFadeEffectHandler.CreateColor(_material.color, alpha * _startAlpha);
            }
        }

        /// <summary>
        /// Interface to handle fading objects.
        /// </summary>
        private interface IFadeEffectHandler
        {
            /// <summary>
            /// Creates a color with a specified alpha.
            /// </summary>
            /// <param name="c">Color</param>
            /// <param name="alpha">Specific alpha</param>
            /// <returns>New color with specific alpha</returns>
            protected static Color CreateColor(Color c, float alpha)
            {
                return new Color(c.r, c.g, c.b, alpha);
            }

            /// <summary>
            /// Sets the alpha of the object.
            /// </summary>
            /// <param name="alpha">Object's alpha</param>
            void SetAlpha(float alpha);
        }
    }
}
