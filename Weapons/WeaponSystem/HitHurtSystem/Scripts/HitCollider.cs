using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour, IHitter
{
    [Header("Configuration")]
    public float damage;
    public string[] affectedTags;

    [Header("Events")]
    public UnityEvent onHit;

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollider(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollider(other);
    }

    private void CheckCollider(Collider otherCollider)
    {
        if (affectedTags.Contains(otherCollider.tag) && otherCollider.GetComponent<Collider>().TryGetComponent<HurtCollider>(out HurtCollider hurtCollider))
        {
            hurtCollider.NotifyHit(this);
            onHit.Invoke();
        }
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}
