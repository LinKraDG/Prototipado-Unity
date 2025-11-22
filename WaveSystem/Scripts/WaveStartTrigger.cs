using UnityEngine;
using UnityEngine.Events;

public class WaveStartTrigger : MonoBehaviour
{
    public UnityEvent onTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        onTriggered.Invoke();
    }
}
