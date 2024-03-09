using System;
using UnityEngine;

public class POI : MonoBehaviour
{
    [SerializeField] protected float interactionRange = 1;
    public event Action<Transform> interactionEvent;
    public void Interact(Transform agent)
    {
        interactionEvent?.Invoke(agent);
    }

    public bool IsInRange(Transform agent)
    {
        return Vector3.Distance(agent.position, transform.position) <= interactionRange;
    }
}
