using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class DancingEnemyController : MonoBehaviour
{
    Animator animator;

    //AnimationEventForwarder aef;

    EntityLife entityLife;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        entityLife = GetComponent<EntityLife>();

        //aef = GetComponent<AnimationEventForwarder>();
    }

    private void OnEnable()
    {
        entityLife.onDeath.AddListener(OnDeath);

        //aef.onEnemyGroundEvent.AddListener(OnEnemyGroundEvent);

    }

    private void OnDisable()
    {
        entityLife.onDeath.RemoveListener(OnDeath);

        //aef.onEnemyGroundEvent.RemoveListener(OnEnemyGroundEvent);

    }

    void OnDeath()
    {
        enabled = false;
        
        animator.enabled = false;
        GetComponentInChildren<Ragdollizer>()?.Ragdollize();

        DOVirtual.DelayedCall(5f, ()=> Destroy(gameObject));
    }
}
