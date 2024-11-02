using UnityEngine;

namespace DefaultNamespace
{
    public class SmallBoulderController : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;
        private Collider2D collider2D;
        private float bounceDamping = 0.9f;
        private float minBounciness = 0.1f;
        private float bounceThreshold = 0.5f;

        private void Start()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (collider2D.sharedMaterial.bounciness > minBounciness)
            {
                collider2D.sharedMaterial.bounciness *= bounceDamping * Time.deltaTime;
            }

            if (rigidbody2D.velocity.magnitude < bounceThreshold)
            {
                rigidbody2D.velocity = Vector2.zero;
                collider2D.sharedMaterial.bounciness = 0;
            }
        }
    }
}