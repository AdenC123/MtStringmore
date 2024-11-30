using UnityEngine;

/// <summary>
/// Handles Knitby's animation
/// </summary>
public class KnitbyAnimator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject poofSmoke;
    private SpriteRenderer _spriteRenderer;
    private KnitbyController _knitbyController;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _knitbyController = GetComponentInParent<KnitbyController>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleSwing();
        _spriteRenderer.flipX = (_knitbyController.currentPathPosition - transform.position).x < 0;
    }

    private void HandleSwing()
    {
        if (_spriteRenderer.enabled != !lineRenderer.isVisible)
        {
            _spriteRenderer.enabled = !lineRenderer.isVisible;
            if (!_spriteRenderer.enabled)
                Instantiate(poofSmoke, transform.position, new Quaternion());
        }
    }
}