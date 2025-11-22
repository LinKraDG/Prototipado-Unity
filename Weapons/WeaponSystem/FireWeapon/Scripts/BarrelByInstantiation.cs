using UnityEngine;

public class BarrelByInstantiation : BarrelBase
{
    [SerializeField]
    GameObject projectile;
    public override void Shoot()
    {
        Instantiate(projectile, transform.position, transform.rotation);
    }
}
