using System.Collections;
using UnityEngine;
using Yarn.Unity;

namespace Yarn
{
    public class CutsceneCharacter : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        // move character within cutscene
        // yarn syntax is <<move CharacterObjectName x y speed [flip]>>
        // e.g. <<move Knitby 3 1 20 true>>
        [YarnCommand("move")]
        public IEnumerator MoveCoroutine(float x, float y, float speed, bool flipSprite = false)
        {
            var position = new Vector3(x, y);
            if (flipSprite) _spriteRenderer.flipX = !_spriteRenderer.flipX;
            while (transform.position != position)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
                yield return null;
            }
        }

        // run is a non-blocking version of move with identical syntax
        [YarnCommand("run")]
        public void Run(float x, float y, float speed, bool flipSprite = false)
        {
            StartCoroutine(MoveCoroutine(x, y, speed, flipSprite));
        }

        [YarnCommand("hide")]
        public void Hide()
        {
            _spriteRenderer.enabled = false;
        }

        [YarnCommand("show")]
        public void Show()
        {
            _spriteRenderer.enabled = true;
        }
    }
}