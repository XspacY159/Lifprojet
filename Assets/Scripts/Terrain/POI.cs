using UnityEngine;
using UnityEngine.Events;

public class POI : MonoBehaviour
{
    [SerializeField] private float interactionRange = 1;
    public UnityEvent interactionEvent;
    public void Interact()
    {
        interactionEvent?.Invoke();
        Debug.Log("Interaction");
    }

    public bool IsInRange(Transform agent)
    {
        return Vector3.Distance(agent.position, transform.position) <= interactionRange;
    }
}
