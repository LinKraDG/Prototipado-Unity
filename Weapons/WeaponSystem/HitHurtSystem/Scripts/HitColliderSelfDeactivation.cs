using DG.Tweening;
using UnityEngine;

public class HitColliderSelfDeactivation : MonoBehaviour
{
    [SerializeField]
    float duration = 0.25f;

    Tween selfDeactivation = null;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        selfDeactivation = DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        if (selfDeactivation != null)
        {
            selfDeactivation.Kill();
            selfDeactivation = null;
        }
    }


}
