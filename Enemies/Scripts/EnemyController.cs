using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class EnemyController : MonoBehaviour
{
    [Header("The Player")]
    [SerializeField]
    Transform target;

    [Header("Configuration")]
    [SerializeField]
    float attackDistance = 1.5f;
    [SerializeField]
    float timeBetweenAttacks = 2f;
    [SerializeField]
    float attackDuration = 0.25f;

    [Header("References")]
    [SerializeField]
    Transform hitCollider;

    float timeOfLastAttack;

    NavMeshAgent agent;
    Animator animator;

    //AnimationEventForwarder aef;

    EntityLife entityLife;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        entityLife = GetComponent<EntityLife>();

        //aef = GetComponent<AnimationEventForwarder>();

        agent.enabled = false;
        DOVirtual.DelayedCall(4.5f, () => agent.enabled = true);
        timeOfLastAttack = Time.time;
    }

    private void OnEnable()
    {
        entityLife.onDeath.AddListener(OnDeath);

        //aef.onEnemyGroundEvent.AddListener(OnEnemyGroundEvent);

    }

    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) < attackDistance)
        {
            agent.isStopped = true;
            if ((Time.time - timeOfLastAttack) > timeBetweenAttacks)
            {
                animator.SetTrigger("Attack");
                hitCollider.gameObject.SetActive(true);
                DOVirtual.DelayedCall(attackDuration, () => hitCollider.gameObject.SetActive(false));
                timeOfLastAttack = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.destination = target.position;
        }

    }

    private void OnDisable()
    {
        entityLife.onDeath.RemoveListener(OnDeath);

        //aef.onEnemyGroundEvent.RemoveListener(OnEnemyGroundEvent);

    }

    void OnDeath()
    {
        enabled = false;
        
        agent.enabled = false;
        animator.enabled = false;
        hitCollider.gameObject.SetActive(false);
        GetComponentInChildren<Ragdollizer>()?.Ragdollize();

        DOVirtual.DelayedCall(5f, ()=> Destroy(gameObject));
    }

    private void OnEnemyGroundEvent()
    {
        agent.enabled = true;
    }
}
