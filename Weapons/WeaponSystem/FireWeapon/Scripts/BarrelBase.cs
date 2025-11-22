using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    #region Debug
    [Header("Debug")]
    [SerializeField]
    bool debugShoot;

    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            Shoot();
        }
    }
    #endregion
    public abstract void Shoot();

}
