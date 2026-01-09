using Managers;
using UnityEngine;

namespace Knitby
{
    /// <summary>
    ///     Handles Knitby's animation
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class KnitbyAnimator : MonoBehaviour
    {
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int LandKey = Animator.StringToHash("Land");
        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int YVelocityKey = Animator.StringToHash("YVelocity");
        private static readonly int HitWallKey = Animator.StringToHash("HitWall");
        private static readonly int LeaveWallKey = Animator.StringToHash("LeaveWall");
        private static readonly int SwingKey = Animator.StringToHash("InSwing");
        private static readonly int IdleKey = Animator.StringToHash("Idle");
        private static readonly int WaitKey = Animator.StringToHash("Wait");
        private static readonly int PlayerDeadKey = Animator.StringToHash("PlayerDead");
        private static readonly int FadeControl = Shader.PropertyToID("_FadeControl");
        private static readonly string SpinStateName = "Spin";
        
        [SerializeField] private Animator anim;
        [SerializeField] private LineRenderer ropeRenderer;
        [SerializeField] private GameObject deathSmoke;
        
        private KnitbyController _knitbyController;
        private Material _material;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _swingPos;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _material = _spriteRenderer.material;
            _knitbyController = GetComponentInParent<KnitbyController>();
        }

        private void OnEnable()
        {
            _knitbyController.DirectionUpdated += OnMove;
            _knitbyController.GroundedChanged += OnGroundedChanged;
            _knitbyController.WallHitChanged += OnWallHitChanged;
            _knitbyController.Swing += OnSwing;
            _knitbyController.CanDash += OnPlayerCanDash;
            _knitbyController.PlayerDeath += OnPlayerDeath;
            _knitbyController.SetIdle += OnIdle;
            _knitbyController.SetWait += OnWait;

            GameManager.Instance.Reset += OnReset;
        }

        private void OnDisable()
        {
            _knitbyController.DirectionUpdated -= OnMove;
            _knitbyController.GroundedChanged -= OnGroundedChanged;
            _knitbyController.WallHitChanged -= OnWallHitChanged;
            _knitbyController.Swing -= OnSwing;
            _knitbyController.CanDash -= OnPlayerCanDash;
            _knitbyController.PlayerDeath -= OnPlayerDeath;
            _knitbyController.SetIdle -= OnIdle;
            _knitbyController.SetWait -= OnWait;

            GameManager.Instance.Reset -= OnReset;
        }

        private void Update()
        {
            RedrawRope();
        }

        private void RedrawRope()
        {
            if (!ropeRenderer.enabled) return;
            
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, transform.position);
            
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(SpinStateName))
            {
                Vector3 currToPos = _swingPos - transform.position;
                float ropeProgress = Mathf.Min(1, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                Vector3 ropeEndPos = transform.position + ropeProgress * currToPos;
                ropeRenderer.SetPosition(1, ropeEndPos);
            }
            else
            {
                ropeRenderer.SetPosition(1, _swingPos);
            }
        }

        private void OnIdle(bool value)
        {
            anim.SetBool(IdleKey, value);
        }
        
        private void OnWait(bool value)
        {
            anim.SetBool(WaitKey, value);
        }

        private void OnMove(float x, float y)
        {
            anim.SetFloat(YVelocityKey, y);
            const float threshold = 0.01f;
            if (x > threshold)
                _spriteRenderer.flipX = false;
            else if (x < -threshold)
                _spriteRenderer.flipX = true;
        }

        private void OnGroundedChanged(bool grounded)
        {
            anim.SetTrigger(grounded ? LandKey : JumpKey);
            anim.SetBool(GroundedKey, grounded);
        }

        private void OnWallHitChanged(bool wallHit)
        {
            anim.SetTrigger(wallHit ? HitWallKey : LeaveWallKey);
        }

        private void OnSwing(bool inSwing, Vector3 swingPos)
        {
            anim.SetBool(SwingKey, inSwing);
            ropeRenderer.enabled = inSwing;
            _swingPos = swingPos;
        }

        private void OnPlayerCanDash(bool canDash)
        {
            if (canDash && Mathf.Approximately(_material.GetFloat(FadeControl), 0))
                _material.SetFloat(FadeControl, 1);
            else if (!canDash && Mathf.Approximately(_material.GetFloat(FadeControl), 1))
                _material.SetFloat(FadeControl, 0);
        }

        private void OnPlayerDeath()
        {
            Instantiate(deathSmoke, transform);
            anim.SetBool(PlayerDeadKey, true);
        }

        /// <summary>
        ///     On reset, re-enable animation
        /// </summary>
        private void OnReset()
        {
            anim.SetBool(SwingKey, false);
            anim.SetBool(WaitKey, false);
            anim.SetBool(PlayerDeadKey, false);
            ropeRenderer.enabled = false;
        }
    }
}
