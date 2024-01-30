using UnityEngine;
using UnityEngine.Events;

public class POI : MonoBehaviour
{
    [SerializeField] private float interactionRange = 1;
    public UnityEvent interactionEvent;
    public void Interact(Transform agent)
    {
        if(Vector3.Distance(agent.position, transform.position) <= interactionRange)
        {
            interactionEvent?.Invoke();
            Debug.Log("Interaction");
        }
    }
}
