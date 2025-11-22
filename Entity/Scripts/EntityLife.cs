using UnityEngine;
using UnityEngine.Events;

public class EntityLife : MonoBehaviour
{
    [SerializeField]
    float startingLife = 1f;

    public UnityEvent <float> onLifeChange;
    public UnityEvent onDeath;

    float currentLife;

    HurtCollider hurtCollider;

    private void Awake()
    {
        hurtCollider = GetComponent<HurtCollider>();
        currentLife = startingLife;
    }

    private void OnEnable()
    {
        hurtCollider.onHitWithDamage.AddListener(OnHitWithDamage);
    }

    void OnHitWithDamage(float damage)
    {
        currentLife -= damage;
        onLifeChange.Invoke(currentLife);
        if (currentLife <= 0)
        {
            onDeath.Invoke();
        }
    }

    private void OnDisable()
    {
        hurtCollider.onHitWithDamage.RemoveListener(OnHitWithDamage);
    }

    public float GetMaxLife()
    {
        return startingLife;
    }
}
