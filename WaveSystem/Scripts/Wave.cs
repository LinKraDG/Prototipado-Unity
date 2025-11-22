using UnityEngine;
using UnityEngine.Events;

public class Wave : MonoBehaviour
{
    public UnityEvent onWaveFinished;

    void Start()
    {
       gameObject.SetActive(false);
    }

    private void Update()
    {
       if(transform.childCount == 0)
        {
            gameObject.SetActive(false);
            onWaveFinished.Invoke();
        } 
    }
}
