using UnityEngine;

/// <summary>
/// Controls parallax background layer to move with respect to camera movement
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float xParallaxFactor;
    [SerializeField] private float yParallaxFactor = 0.99f;
    private const float SmoothTime = 0.01f;
    private Vector3 _velocity;
    
    public void Move(float deltaX, float deltaY)
    {
        Vector3 newPos = transform.position;
        newPos.x -= deltaX * xParallaxFactor;
        newPos.y -= deltaY * yParallaxFactor;

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _velocity, SmoothTime);
    }
}
