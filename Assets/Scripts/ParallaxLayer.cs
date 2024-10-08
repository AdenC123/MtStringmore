using UnityEngine;

/// <summary>
/// Controls parallax background layer to move with respect to camera movement
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;

        transform.localPosition = newPos;
    }
}
