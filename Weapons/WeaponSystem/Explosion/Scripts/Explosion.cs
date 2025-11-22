using UnityEngine;

public class Explosion : MonoBehaviour, IHitter
{
    [SerializeField]
    float force = 2000f;
    
    [SerializeField]
    float damage = 10f;
    [SerializeField]
    float radius = 10f;

    [SerializeField]
    LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField]
    LayerMask occluderLayerMask = Physics.DefaultRaycastLayers;

    [SerializeField]
    GameObject visualExplosionPrefab;

    private void Start()
    {
        foreach (Collider c in Physics.OverlapSphere(transform.position, radius, targetLayerMask))
        {
            if (!Physics.Linecast(transform.position, c.transform.position, out RaycastHit hit, occluderLayerMask) || (hit.collider == c))
            {
                c.GetComponent<HurtCollider>()?.NotifyHit(this);
            }

            c.attachedRigidbody?.AddExplosionForce(force, transform.position, radius);
        }

        Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }
}
