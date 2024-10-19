using UnityEngine;

/// <summary>
/// Controls the canon that shoots the boulders
/// </summary>
public class CanonController : MonoBehaviour
{
    private Rigidbody2D rb;

    private BoulderController boulder = new BoulderController();
    [SerializeField] public float minXVelocity = -20f;
    [SerializeField] public float maxXVelocity = 0f;
    [SerializeField] public float minGravityScale = 1f;
    [SerializeField] public float maxGravityScale = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}