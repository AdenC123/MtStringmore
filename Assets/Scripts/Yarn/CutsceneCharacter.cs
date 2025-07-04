using System.Collections;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using Yarn.Unity;

namespace Yarn
{
    public class CutsceneCharacter : MonoBehaviour
    {
        [CanBeNull] private PlayerAnimator _playerAnimator;
        private Animator _animator;
        private static readonly int YVelocityKey = Animator.StringToHash("YVelocity");
        private SpriteRenderer _spriteRenderer;
        private bool _isMoving;

        private void Awake()
        {
            _playerAnimator = gameObject.GetComponentInChildren<PlayerAnimator>();
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
            _isMoving = true;
            while (transform.position != position)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
                yield return null;
            }
            _isMoving = false;
            if (disableAnimation) yield return SetAnimation(false);
        }
        
        // leap character within cutscene
        // yarn syntax is <<leap CharacterObjectName xVel yVelInitial duration [grounded] [gravity] [flip] [disable]>>
        // e.g. <<leap Knitby 3 1 20 true -66 true false>>
        [YarnCommand("leap")]
        public IEnumerator LeapCoroutine(float xVel, float yVelInitial, float duration, bool grounded = false, float gravity = -66, bool flipSprite = false,
            bool disableAnimation = false)
        {
            float elapsedTime = 0;
            _animator.enabled = true;
            if (flipSprite) _spriteRenderer.flipX = !_spriteRenderer.flipX;
            _isMoving = true;
            
            float yVel = yVelInitial;
            
            // animate
            _playerAnimator?.OnWallChanged(false);
            _playerAnimator?.OnJumped();
            _playerAnimator?.OnGroundedChanged(false, 0f);
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                // do scuffed kinematics
                transform.position = new Vector3(
                    transform.position.x + Time.deltaTime * xVel,
                    transform.position.y + yVel * Time.deltaTime + 0.5f * gravity * Mathf.Pow(Time.deltaTime, 2.0f),
                    transform.position.z);
                // update new yVel
                yVel += Time.deltaTime * gravity;
                _animator.SetFloat(YVelocityKey, yVel);
                yield return null;
            }
            _isMoving = false;
            if (grounded) _playerAnimator?.OnGroundedChanged(true, 0f);
            else _playerAnimator?.OnWallChanged(true);
            if (disableAnimation) yield return SetAnimation(false);
        }
        
        // leap_nonblock is a non-blocking version of leap with identical syntax
        [YarnCommand("leap_nonblock")]
        public void Leap(float xVel, float yVelInitial, float duration, bool grounded = false, float gravity = -66, bool flipSprite = false,
            bool disableAnimation = false)
        {
            StartCoroutine(LeapCoroutine(xVel, yVelInitial, duration, grounded, gravity, flipSprite, disableAnimation));
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

        [YarnCommand("wait_move_finish")]
        public IEnumerator WaitMoveFinish()
        {
            yield return new WaitUntil(() => !_isMoving);
        }
    }
}
