using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent onHit;
    public UnityEvent <float> onHitWithDamage;

    internal void NotifyHit(IHitter hitter)
    {
        onHit.Invoke();
        onHitWithDamage.Invoke(hitter.GetDamage());
    }

}
