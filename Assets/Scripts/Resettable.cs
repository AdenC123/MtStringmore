using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Superclass for game objects that get reset when the player respawns
/// </summary>
public class Resettable: MonoBehaviour
{
    private List<Resettable> _children = new();
    protected virtual void Start()
    {
        foreach (Transform child in transform)
        {
            Resettable resettable = child.GetComponent<Resettable>();
            if (resettable is not null)
                _children.Add(resettable);
        }
    }

    /// <summary>
    /// Resets the transform of the current object to its starting transform in the scene,
    /// and recursively call reset on all resettable children
    /// </summary>
    public virtual void Reset()
    {
        Debug.Log("resetting " + gameObject.name);
        foreach (Resettable child in _children)
            child.Reset();
    }
}