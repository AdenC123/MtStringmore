using UnityEngine;

/// <summary>
/// Class represents a bouncy platform that is a 2D collider
/// </summary>
public class BouncyPlatform : MonoBehaviour
{
    #region Serialized Public Fields
    [Header("Bouncing")] 
    [SerializeField] public float yBounceForce;
    [SerializeField] public float xBounceForce;
    #endregion
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the object
        if (other.CompareTag("Player"))
        {
            // Trigger the animation
            animator.SetTrigger("Bounce");
        }
    }
    
}
