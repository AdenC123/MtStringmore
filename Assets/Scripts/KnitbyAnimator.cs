using UnityEngine;

/// <summary>
/// Handles Knitby's animation
/// </summary>
public class KnitbyAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject poofSmoke;
    
    private SpriteRenderer _spriteRenderer;
    private KnitbyController _knitbyController;
    
    private static readonly int SwingKey = Animator.StringToHash("InSwing");
    private static readonly int YVelocityKey = Animator.StringToHash("YVelocity");
    
    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _knitbyController = GetComponentInParent<KnitbyController>();
    }

    private void Update()
    {
        anim.SetFloat(YVelocityKey, _knitbyController.direction.y);
        _spriteRenderer.flipX = _knitbyController.direction.x < 0;
        HandleSwing();
    }

    private void HandleSwing()
    {
        if (_spriteRenderer.enabled != !lineRenderer.isVisible)
        {
            _spriteRenderer.enabled = !lineRenderer.isVisible;
            anim.SetBool(SwingKey, lineRenderer.isVisible);
            if (!_spriteRenderer.enabled)
                Instantiate(poofSmoke, transform.position, new Quaternion());
        }
    }
}