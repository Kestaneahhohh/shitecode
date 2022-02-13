using UnityEngine;

[CreateAssetMenu(menuName = "Framework/GunData")]
public class GunData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed;
    public float walkSpeedAim;
    public float sprintSpeed;
    public float jumpHeight;
    [Space(20)]

    [Header("Weapon Stats")]
    public float damage;
    public float range;
    public float fireRate;
    public AudioClip fireSound;
    public GameObject gunModel;
    [Space(20)]

    [Header("Ammo")]
    public float magAmmo;
    public float reserveAmmo;
    public float reloadTimeTactical;
    public float reloadTimeEmpty;
    [Space(20)]

    [Header("ADS")]
    public float fovMain = 70.0f;
    public float fovScoped = 20.0f;
    public float scopeSpeed = 10.0f;
    [Space(20)]

    [Header("Offsets")]
    public Vector3 normalPosition;
    public Vector3 sprintPosition;
    public Vector3 aimedPosition;
    [Space(20)]
    public Vector3 lArmOffset;
    public Vector3 rArmOffset;
    [Space(20)]
    public Vector3 weaponPosOffset;
    public Vector3 weaponRotOffset;
    [Space(20)]
    public Vector3 boltOffset;
    public float boltSpeed;
    [Space(20)]
    public Vector3 weaponKick;
    public Vector3 weaponKickRot;
    [Space(20)]
    public Vector3 weaponAimKick;
    public Vector3 weaponAimKickRot;

    [Header("Sway")]
    public float amount;
    public float maxAmount;
    public float smoothAmount;
    [Space(20)]
    public float rotationAmount;
    public float maxRotationAmount;
    public float smoothRotation;
    [Space(20)]
    public bool rotationX;
    public bool rotationY;
    public bool rotationZ;

    [Header("Recoil")]
    public Vector3 minRecoil;
    public Vector3 maxRecoil;
    [Space(20)]
    public Vector3 aimMinRecoil;
    public Vector3 aimMaxRecoil;
    [Space(20)]
    public float snappiness;
    public float returnSpeed;
}
