using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{

    [SerializeField]
    Transform objectsToActivateOnStartParent;

    [SerializeField]
    Transform objectsToActivateOnEndParent;

    public UnityEvent onWavesFinished;

    Wave[] waves;
    WaveStartTrigger waveStartTrigger;

    bool alreadyStarted;
    int currentWaveIndex = 0;

    private void Awake()
    {
        waves = GetComponentsInChildren<Wave>(true);
        waveStartTrigger = GetComponentInChildren<WaveStartTrigger>();
    }

    private void OnEnable()
    {
        foreach (Wave w in waves)
        {
            w.onWaveFinished.AddListener(OnWaveFinished);
        }
        waveStartTrigger.onTriggered.AddListener(OnStartTriggerTriggered);
    }

    private void OnDisable()
    {
        foreach (Wave w in waves)
        {
            w.onWaveFinished.RemoveListener(OnWaveFinished);
        }
        waveStartTrigger.onTriggered.RemoveListener(OnStartTriggerTriggered);
    }

    void OnWaveFinished()
    {
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length)
        {
            waves[currentWaveIndex].gameObject.SetActive(true);
        }
        else
        {
            objectsToActivateOnStartParent.gameObject.SetActive(false);
            onWavesFinished.Invoke();
        }

        if (currentWaveIndex >= waves.Length)
        {
            objectsToActivateOnEndParent.gameObject.SetActive(true);
        }
    }

    void OnStartTriggerTriggered()
    {
        if (!alreadyStarted)
        {
            alreadyStarted = true;
            objectsToActivateOnStartParent.gameObject.SetActive(true);
            waves[currentWaveIndex].gameObject.SetActive(true);
        }
    }
}
