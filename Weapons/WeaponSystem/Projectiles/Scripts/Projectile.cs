using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float startSpeed = 10f;
    [SerializeField]
    GameObject explosionPrefab;
    [SerializeField]
    float timeToDieAfterCollision = 0f;
    [SerializeField]
    float lifeTime = 10f;

    HitCollider hitCollider;
    Tween deathTween = null;
    private void Awake()
    {
        hitCollider = GetComponent<HitCollider>();
    }

    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * startSpeed;
        DOVirtual.DelayedCall(lifeTime, () => Destroy(gameObject));
    }

    private void OnEnable()
    {
        hitCollider?.onHit.AddListener(PerformDestruction);
    }

    private void OnDisable()
    {
        hitCollider?.onHit.RemoveListener(PerformDestruction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (deathTween == null)
        {
            deathTween = DOVirtual.DelayedCall(
                timeToDieAfterCollision,
                () =>
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                );
        }
    }

    public void PerformDestruction()
    {
        Destroy(gameObject);
    }
}
