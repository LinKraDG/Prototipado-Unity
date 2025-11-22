using DG.Tweening;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    Transform weaponsParent;

    /*[Header("IK")]
    [SerializeField]
    Rig armsRig;*/

    [Header("Inputs Combat")]
    [SerializeField]
    InputActionReference attack;
    bool mustAttack = false;

    [SerializeField]
    InputActionReference nextPrevWeapon;

    [Header("Inputs Combat")]
    [SerializeField]
    InputActionReference shoot;
    [SerializeField]
    InputActionReference continuousShoot;
    [SerializeField]
    InputActionReference aim;


    Animator animator;
    RuntimeAnimatorController originalAnimatorController;

    WeaponBase[] weapons;
    int currentWeapon = 0;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        originalAnimatorController = animator.runtimeAnimatorController;
        weapons = weaponsParent.GetComponentsInChildren<WeaponBase>(true);
        foreach (WeaponBase wb in weapons)
        {
            wb.Init();
        }
        weapons[currentWeapon].Select(animator);
    }

    private void OnEnable()
    {
        attack.action.Enable();
        nextPrevWeapon.action.Enable();

        shoot.action.Enable();
        continuousShoot.action.Enable();
        aim.action.Enable();

        attack.action.performed += OnAttack;
        nextPrevWeapon.action.performed += OnNextPrevWeapon;

        shoot.action.performed += OnShoot;
        continuousShoot.action.started += OnContinuousShoot;
        continuousShoot.action.canceled += OnContinuousShoot;
        aim.action.started += OnAim;
        aim.action.canceled += OnAim;

        foreach (AnimationEventForwarder aef in GetComponentsInChildren<AnimationEventForwarder>())
        {
            aef.onKnifeAttackEvent.AddListener(OnKnifeAttackEvent);
        }
    }

    private void Update()
    {
        UpdateCombat();
    }

    private void OnDisable()
    {
        attack.action.Disable();
        nextPrevWeapon.action.Disable();

        shoot.action.Disable();
        continuousShoot.action.Disable();
        aim.action.Disable();

        attack.action.performed -= OnAttack;
        nextPrevWeapon.action.performed -= OnNextPrevWeapon;

        shoot.action.performed -= OnShoot;
        continuousShoot.action.started -= OnContinuousShoot;
        continuousShoot.action.canceled -= OnContinuousShoot;
        aim.action.started -= OnAim;
        aim.action.canceled -= OnAim;

        foreach (AnimationEventForwarder aef in GetComponentsInChildren<AnimationEventForwarder>())
        {
            aef.onKnifeAttackEvent.RemoveListener(OnKnifeAttackEvent);
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        mustAttack = true;
    }

    private void OnNextPrevWeapon(InputAction.CallbackContext context)
    {
        Vector2 readValue = context.ReadValue<Vector2>();
        int weaponToSet = currentWeapon;
        if (readValue.y > 0f)
        {
            weaponToSet++;
            if (weaponToSet >= weapons.Length)
            {
                weaponToSet = 0;
            }
        } 
        else
        {
            weaponToSet--;
            if (weaponToSet < 0)
            {
                weaponToSet = weapons.Length -1;
            }
        }
        if (weaponToSet != currentWeapon)
        {
            SelectWeapon(weaponToSet);
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if((currentWeapon != -1) && (weapons[currentWeapon] is Weapon_FireWeapon) && ((Weapon_FireWeapon)weapons[currentWeapon]).CanShootByShoot())
        {
            ((Weapon_FireWeapon)weapons[currentWeapon])?.Shoot();
        }
        
    }

    private void OnContinuousShoot(InputAction.CallbackContext context)
    {
        if ((currentWeapon != -1) && (weapons[currentWeapon] is Weapon_FireWeapon) && ((Weapon_FireWeapon)weapons[currentWeapon]).CanContinuousShoot())
        {
            float value = context.ReadValue<float>();
            if (value > 0f)
            {
                ((Weapon_FireWeapon)weapons[currentWeapon])?.StartShooting();
            }else
            {
                ((Weapon_FireWeapon)weapons[currentWeapon])?.StopShooting();
            }
        }
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        if (currentWeapon != -1)
        {
            float value = context.ReadValue<float>();
            animator.SetBool("IsAiming", value > 0f);
        }
    }

    private void OnKnifeAttackEvent()
    {
        weapons[currentWeapon].PerformAttack();
    }

    void SelectWeapon(int weaponToSet)
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Deselect(animator);
            if (weapons[currentWeapon] is Weapon_FireWeapon)
            {
                animator.SetBool("IsAiming", false);
            }
        }

        currentWeapon = weaponToSet;

        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Select(animator);
            animator.SetBool("IsHoldingFireWeapon", weapons[currentWeapon] is Weapon_FireWeapon);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
        }

        //AnimateArmRigsWeight();
    }

    /*private void AnimateArmRigsWeight()
    {
        DOTween.To(
            () => armsRig.weight,
            (x) => armsRig.weight = x,
            (currentWeapon != -1) && (weapons[currentWeapon] is Weapon_FireWeapon) ? 1f : 0f,
            0.25f
            );
    }*/

    private void UpdateCombat()
    {
        if (mustAttack)
        {
            mustAttack = false;
            animator.SetTrigger("Attack");
        }
    }
}
