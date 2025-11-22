using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForwarder : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent <string> onAnimationEvent;
    [HideInInspector]
    public UnityEvent onEnemyGroundEvent;
    [HideInInspector]
    public UnityEvent onKnifeAttackEvent;

    public void OnAnimationAttack(string hitColliderName)
    {
        onAnimationEvent.Invoke(hitColliderName);
    }

    public void KnifeAttack()
    {
        onKnifeAttackEvent.Invoke();
    }

    public void OnEnemyGroundAnimation()
    {
        onEnemyGroundEvent.Invoke();
    }
}
