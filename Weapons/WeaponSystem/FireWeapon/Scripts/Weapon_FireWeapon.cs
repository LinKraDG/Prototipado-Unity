using UnityEngine;

public class Weapon_FireWeapon : WeaponBase
{
    public enum FireWeaponMode
    {
        ShotByShot,
        ContinuousShot
    }

    [SerializeField]
    FireWeaponMode fireWeaponMode = FireWeaponMode.ShotByShot;

    [SerializeField]
    float shotsPerSecond = 3f;
    
    BarrelBase[] barrels;

    float timeOfLastShoot;
    bool isShooting;

    #region Debug
    [Header("Debug")]
    [SerializeField]
    bool debugShoot;

    [SerializeField]
    bool debugStartShooting;
    [SerializeField]
    bool debugStopShooting;

    [SerializeField]
    bool debugShootBurst;
    [SerializeField]
    bool debugCancelBurst;


    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            Shoot();
        }
    }
    #endregion

    internal override void Init()
    {
        base.Init();
        barrels = GetComponentsInChildren<BarrelBase>();

        timeOfLastShoot = Time.time - (1f / shotsPerSecond);
    }

    private void Update()
    {
        if (isShooting)
        {
            PerformShoot();
        }
    }

    public void Shoot()
    {
        if (fireWeaponMode == FireWeaponMode.ShotByShot)
        {
            PerformShoot();
        }
    }

    private void PerformShoot()
    {
        if ((Time.time - timeOfLastShoot) > (1f / shotsPerSecond))
        {
            foreach (BarrelBase bb in barrels)
            {
                bb.Shoot();
            }
        }
    }

    public void StartShooting()
    {
        if (fireWeaponMode == FireWeaponMode.ContinuousShot)
        {
            isShooting = true;
        }
    }

    public void StopShooting()
    {
        if (fireWeaponMode == FireWeaponMode.ContinuousShot)
        {
            isShooting = false;
        }
    }

    internal override void PerformAttack()
    {
        //throw new System.NotImplementedException();
    }

    internal override void Deselect(Animator animator)
    {
        base.Deselect(animator);
        isShooting = false;
    }

    internal bool CanShootByShoot()
    {
        return fireWeaponMode == FireWeaponMode.ShotByShot;
    }

    internal bool CanContinuousShoot()
    {
        return fireWeaponMode == FireWeaponMode.ContinuousShot;
    }
}
