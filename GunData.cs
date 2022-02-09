using UnityEngine;

[CreateAssetMenu(menuName = "Framework/GunData")]
public class GunData : ScriptableObject
{
    public float walkSpeed { get { return _walkSpeed; } private set { _walkSpeed = value; } }
    [SerializeField] private float _walkSpeed;

    public float walkSpeedAim { get { return _walkSpeedAim; } private set { _walkSpeedAim = value; } }
    [SerializeField] private float _walkSpeedAim;

    public float sprintSpeed { get { return _sprintSpeed; } private set { _sprintSpeed = value; } }
    [SerializeField] private float _sprintSpeed;

    public float jumpHeight { get { return _jumpHeight; } private set { _jumpHeight = value; } }
    [SerializeField] private float _jumpHeight;

    public float damage { get { return _damage; } private set { _damage = value; } }
    [SerializeField] private float _damage;

    public float range { get { return _range; } private set { _range = value; } }
    [SerializeField] private float _range;

    public float fireRate { get { return _fireRate; } private set { _fireRate = value; } }
    [SerializeField] private float _fireRate;

    public AudioClip fireSound { get { return _fireSound; } private set { _fireSound = value; } }
    [SerializeField] private AudioClip _fireSound;

    public float magAmmo { get { return _magAmmo; } private set { _magAmmo = value; } }
    [SerializeField] private float _magAmmo;

    public float reserveAmmo { get { return _reserveAmmo; } private set { _reserveAmmo = value; } }
    [SerializeField] private float _reserveAmmo;

    public float reloadTimeEmpty { get { return _reloadTimeEmpty; } private set { _reloadTimeEmpty = value; } }
    [SerializeField] private float _reloadTimeEmpty;

    public float reloadTimeTactical { get { return _reloadTimeTactical; } private set { _reloadTimeTactical = value; } }
    [SerializeField] private float _reloadTimeTactical;

    public float fovMain { get { return _fovMain; } private set { _fovMain = value; } }
    [SerializeField] private float _fovMain = 70.0f;

    public float fovScoped { get { return _fovScoped; } private set { _fovScoped = value; } }
    [SerializeField] private float _fovScoped = 20.0f;

    public float smoothingFactor { get { return _smoothingFactor; } private set { _smoothingFactor = value; } }
    [SerializeField] private float _smoothingFactor = 10.0f;

    public Vector3 normalPosition { get { return _normalPosition; } private set { _normalPosition = value; } }
    [SerializeField] private Vector3 _normalPosition;

    public Vector3 aimedPosition { get { return _aimedPosition; } private set { _aimedPosition = value; } }
    [SerializeField] private Vector3 _aimedPosition;
    
    public Vector3 weaponRotOffset { get { return _weaponRotOffset; } private set { _weaponRotOffset = value; } }
    [SerializeField] private Vector3 _weaponRotOffset;

    public Vector3 weaponKick { get { return _weaponKick; } private set { _weaponKick = value; } }
    [SerializeField] private Vector3 _weaponKick;
    
    public Vector3 weaponAimKick { get { return _weaponAimKick; } private set { _weaponAimKick = value; } }
    [SerializeField] private Vector3 _weaponAimKick;
    
    public Vector3 weaponKickRot { get { return _weaponKickRot; } private set { _weaponKickRot = value; } }
    [SerializeField] private Vector3 _weaponKickRot;
    
    public Vector3 weaponAimKickRot { get { return _weaponAimKickRot; } private set { _weaponAimKickRot = value; } }
    [SerializeField] private Vector3 _weaponAimKickRot;

    public float amount { get { return _amount; } private set { _amount = value; } }
    [SerializeField] private float _amount;

    public float maxAmount { get { return _maxAmount; } private set { _maxAmount = value; } }
    [SerializeField] private float _maxAmount;

    public float smoothAmount { get { return _smoothAmount; } private set { _smoothAmount = value; } }
    [SerializeField] private float _smoothAmount;

    public float rotationAmount { get { return _rotationAmount; } private set { _rotationAmount = value; } }
    [SerializeField] private float _rotationAmount;

    public float maxRotationAmount { get { return _maxRotationAmount; } private set { _maxRotationAmount = value; } }
    [SerializeField] private float _maxRotationAmount;

    public float smoothRotation { get { return _smoothRotation; } private set { _smoothRotation = value; } }
    [SerializeField] private float _smoothRotation;

    public bool rotationX { get { return _rotationX; } private set { _rotationX = value; } }
    [SerializeField] private bool _rotationX;

    public bool rotationY { get { return _rotationY; } private set { _rotationY = value; } }
    [SerializeField] private bool _rotationY;

    public bool rotationZ { get { return _rotationZ; } private set { _rotationZ = value; } }
    [SerializeField] private bool _rotationZ;

    public Vector3 minRecoil { get { return _minRecoil; } private set { _minRecoil = value; } }
    [SerializeField] private Vector3 _minRecoil;

    public Vector3 maxRecoil { get { return _maxRecoil; } private set { _maxRecoil = value; } }
    [SerializeField] private Vector3 _maxRecoil;
    
    public Vector3 aimMinRecoil { get { return _aimMinRecoil; } private set { _aimMinRecoil = value; } }
    [SerializeField] private Vector3 _aimMinRecoil;

    public Vector3 aimMaxRecoil { get { return _aimMaxRecoil; } private set { _aimMaxRecoil = value; } }
    [SerializeField] private Vector3 _aimMaxRecoil;

    public float snappiness { get { return _snappiness; } private set { _snappiness = value; } }
    [SerializeField] private float _snappiness;

    public float returnSpeed { get { return _returnSpeed; } private set { _returnSpeed = value; } }
    [SerializeField] private float _returnSpeed;

    public Vector3 lArmOffset { get { return _lArmOffset; } private set { _lArmOffset = value; } }
    [SerializeField] private Vector3 _lArmOffset = new Vector3(-0.195f, 0.1f, 0.645f);

    public Vector3 rArmOffset { get { return _rArmOffset; } private set { _rArmOffset = value; } }
    [SerializeField] private Vector3 _rArmOffset = new Vector3(-0.035f, 0.035f, 0.285f);

    public Vector3 boltOffset { get { return _boltOffset; } private set { _boltOffset = value; } }
    [SerializeField] private Vector3 _boltOffset;
}
