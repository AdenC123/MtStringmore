using System.Collections;
using UnityEngine;
using Yarn.Unity;

namespace Yarn
{
    public class CutsceneCharacter : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            _animator = gameObject.GetComponentInChildren<Animator>();
        }

        // move character within cutscene
        // yarn syntax is <<move CharacterObjectName x y speed [flip] [disable] [z]>>
        // e.g. <<move Knitby 3 1 20 true false 1>>
        [YarnCommand("move")]
        public IEnumerator MoveCoroutine(float x, float y, float speed, bool flipSprite = false,
            bool disableAnimation = false, float z = 0)
        {
            _animator.enabled = true;
            Vector3 position = new(x, y, z == 0 ? transform.position.z : z);
            if (flipSprite) _spriteRenderer.flipX = !_spriteRenderer.flipX;
            while (transform.position != position)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
                yield return null;
            }

            if (disableAnimation) yield return SetAnimation(false);
        }

        [YarnCommand("flip")]
        public void Flip()
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        // run is a non-blocking version of move with identical syntax
        [YarnCommand("run")]
        public void Run(float x, float y, float speed, bool flipSprite = false, bool disableAnimation = false,
            float z = 0)
        {
            StartCoroutine(MoveCoroutine(x, y, speed, flipSprite, disableAnimation, z));
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

        [YarnCommand("set_animation")]
        public IEnumerator SetAnimation(bool state)
        {
            // wait until current animation is finished
            int finishTime = Mathf.CeilToInt(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= finishTime &&
                                             !Mathf.Approximately(
                                                 _animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.0f));
            _animator.enabled = state;
        }
    }
}
