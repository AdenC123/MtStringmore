using UnityEngine;

/// <summary>
///     Handles Knitby's animation
/// </summary>
public class KnitbyAnimator : MonoBehaviour
{
    private static readonly int IdleKey = Animator.StringToHash("Idle");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int GroundedKey = Animator.StringToHash("Land");
    private static readonly int YVelocityKey = Animator.StringToHash("YVelocity");
    private static readonly int SwingKey = Animator.StringToHash("InSwing");
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject deathSmoke;
    private KnitbyController _knitbyController;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _knitbyController = GetComponentInParent<KnitbyController>();
    }

    private void OnEnable()
    {
        _knitbyController.DirectionUpdated += OnMove;
        _knitbyController.GroundedChanged += OnGroundedChanged;
        _knitbyController.Swing += OnSwing;
        _knitbyController.PlayerDeath += OnPlayerDeath;
        _knitbyController.SetIdle += OnIdle;
    }

    private void OnDisable()
    {
        _knitbyController.DirectionUpdated -= OnMove;
        _knitbyController.GroundedChanged -= OnGroundedChanged;
        _knitbyController.Swing -= OnSwing;
        _knitbyController.PlayerDeath -= OnPlayerDeath;
        _knitbyController.SetIdle += OnIdle;
    }

    private void OnIdle(bool value)
    {
        Debug.Log($"on idle called with val {value}");
        anim.SetBool(IdleKey, value);
    }

    private void OnMove(float x, float y)
    {
        anim.SetFloat(YVelocityKey, y);
        _spriteRenderer.flipX = x < 0;
    }

    private void OnGroundedChanged(bool grounded)
    {
        anim.SetTrigger(grounded ? GroundedKey : JumpKey);
    }

    private void OnSwing(bool inSwing)
    {
        anim.SetBool(SwingKey, inSwing);
    }

    private void OnPlayerDeath()
    {
        Instantiate(deathSmoke, transform);
        anim.enabled = false;
    }
}
