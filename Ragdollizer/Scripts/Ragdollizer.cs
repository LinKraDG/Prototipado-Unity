using UnityEngine;

public class Ragdollizer : MonoBehaviour
{
    [SerializeField]
    bool debugRagdollize;

    Collider[] colliders;
    Rigidbody[] rigidbodies;

    #region Debug
    private void OnValidate()
    {
        if (debugRagdollize)
        {
            debugRagdollize = false;
            Ragdollize();
        }
    }
    #endregion

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        UnRagdollize();
    }

    public void UnRagdollize()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    public void Ragdollize()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}
