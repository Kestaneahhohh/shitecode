using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform playerBody, camTransform, holder, swayer;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private AudioSource am;
    [SerializeField]
    private GameObject impactEffect;
    [SerializeField]
    private GunData gunData;
    [HideInInspector]
    private Recoil Recoil;
    [HideInInspector]
    public float mouseSensitivity = 1f, scopedSensitivity = 0.25f;
    private float xRotation = 0f, mouseX, mouseY;
    [SerializeField]
    private GameObject lArm, rArm, gunModel;

    //Sway bullshit
    private float amount, maxAmount, smoothAmount, rotationAmount, maxRotationAmount, smoothRotation;
    private bool rotationX, rotationY, rotationZ;
    private Vector3 initSwayPos;
    private Quaternion initSwayRot;

    //Recoil bullshit
    [HideInInspector]
    public float minRecoilX, minRecoilY, minRecoilZ, maxRecoilX, maxRecoilY, maxRecoilZ, aimMinRecoilX, aimMinRecoilY, aimMinRecoilZ, aimMaxRecoilX, aimMaxRecoilY, aimMaxRecoilZ, snappiness, returnSpeed;

    //Movement bullshit
    [HideInInspector]
    private float gravity = -36.0f, fovMain, fovScoped, smoothingFactor, walkSpeed, walkSpeedAim, sprintSpeed, jumpHeight;

    [HideInInspector]
    public bool walking, aiming, sprinting = false;
    [HideInInspector]
    public Vector3 velocity, move;

    //Weapon bullshit
    [HideInInspector]
    public float currentAmmo, reserveAmmo, health;
    private float nextTimeToFire, damage, range, fireRate, magAmmo, reloadTimeEmpty, reloadTimeTactical;
    private AudioClip fireSound;
    private Vector3 normalPosition, aimedPosition, weaponKick, weaponAimKick, weaponKickRot, weaponAimKickRot, boltOffset, initGunBodyPos, initGunMagPos, initGunBoltPos;
    private Transform gunBody, gunMag, gunBolt;
    private bool reloading = false;

    [HideInInspector]
    public bool paused;

    void Start()
    {
        Recoil = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
        am = GameObject.Find("player").GetComponent<AudioSource>();

        initSwayPos = swayer.localPosition;
        initSwayRot = swayer.localRotation;

        health = 100f;

        GetValues();
        LockCursor();

        StartCoroutine(SetGunTransforms());

        IEnumerator SetGunTransforms()
        {
            yield return StartCoroutine(SpawnGun());

            gunBody = gunModel.transform.Find("Body");
            gunMag = gunModel.transform.Find("Mag");
            gunBolt = gunModel.transform.Find("Bolt");

            initGunBodyPos = gunBody.localPosition;
            initGunMagPos = gunMag.localPosition;
            initGunBoltPos = gunBolt.localPosition;
        }

        IEnumerator SpawnGun()
        {
            yield return StartCoroutine(SpawnArms());

            rArm = holder.Find("Right Arm(Clone)").gameObject;
            gunModel = Instantiate(gunModel);
            gunModel.transform.parent = rArm.transform;
            rArm.transform.GetChild(0).localPosition = Vector3.zero;
            rArm.transform.GetChild(0).localEulerAngles = gunData.weaponRotOffset;
            rArm.transform.GetChild(0).localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }

        IEnumerator SpawnArms()
        {
            lArm = Instantiate(lArm);
            lArm.transform.parent = holder;
            lArm.transform.localPosition = gunData.lArmOffset;
            lArm.transform.localEulerAngles = new Vector3(-90f, -90, 0);
            lArm.transform.localScale = new Vector3(50f, 50f, 50f);

            rArm = Instantiate(rArm);
            rArm.transform.parent = holder;
            rArm.transform.localPosition = gunData.rArmOffset;
            rArm.transform.localEulerAngles = new Vector3(-90f, -90, 0);
            rArm.transform.localScale = new Vector3(50f, 50f, 50f);

            yield return null;
        }
    }

    void Update()
    {
        PlayerMove();

        if (!paused)
        {
            if (Input.GetButton("Fire2") && !reloading)
            {
                mouseX = Input.GetAxis("Mouse X") * scopedSensitivity;
                mouseY = Input.GetAxis("Mouse Y") * scopedSensitivity;

                holder.localPosition = Vector3.Slerp(holder.localPosition, aimedPosition, smoothingFactor * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovScoped, smoothingFactor * Time.deltaTime);
            }
            else
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                holder.localPosition = Vector3.Slerp(holder.localPosition, normalPosition, smoothingFactor * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovMain, smoothingFactor * Time.deltaTime);
            }

            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0f && !reloading)
            {
                nextTimeToFire = Time.time + 60f / fireRate;
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo != magAmmo && reserveAmmo != 0f && !reloading)
            {
                StartCoroutine(Reload());
            }

            CameraLook(mouseX, mouseY);

            TiltAndSway(mouseX, mouseY);

            gunBolt.localPosition = Vector3.Lerp(gunBolt.localPosition, initGunBoltPos, 20f * Time.deltaTime);

            if (reloading)
            {
                gunMag.transform.localPosition = Vector3.Slerp(gunMag.transform.localPosition, new Vector3(0.333f, 0, 0), Time.deltaTime * 10f);
                lArm.transform.localPosition = Vector3.Slerp(lArm.transform.localPosition, gunData.lArmOffset - new Vector3(1f, 0, 0), Time.deltaTime * 10f);
            }
            else if (!reloading)
            {
                gunMag.transform.localPosition = Vector3.Slerp(gunMag.transform.localPosition, Vector3.zero, Time.deltaTime * 20f);
                lArm.transform.localPosition = Vector3.Slerp(lArm.transform.localPosition, gunData.lArmOffset, Time.deltaTime * 20f);
            }
        }
    }

    void Shoot()
    {
        am.pitch = Random.Range(1.1f, 1.2f);
        am.PlayOneShot(fireSound, 0.5f);
        Recoil.RecoilFire();
        gunBolt.localPosition = boltOffset;

        if (!aiming)
        {
            swayer.localPosition += weaponKick;
            swayer.localEulerAngles += weaponKickRot;
        }
        else
        {
            swayer.localPosition += weaponAimKick;
            swayer.localEulerAngles += weaponAimKickRot;
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.5f);
        }

        currentAmmo--;
    }

    IEnumerator Reload()
    {
        reloading = true;

        if (currentAmmo != magAmmo && currentAmmo > 0f)
        {
            yield return new WaitForSeconds(reloadTimeTactical);

            if (reserveAmmo + currentAmmo > magAmmo)
            {
                reserveAmmo -= (magAmmo - currentAmmo);
                currentAmmo = magAmmo;
            }
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0f;
            }
        }
        else if (currentAmmo == 0f)
        {
            yield return new WaitForSeconds(reloadTimeEmpty);

            if (reserveAmmo + currentAmmo > magAmmo)
            {
                reserveAmmo -= (magAmmo - currentAmmo);
                currentAmmo = magAmmo;
            }
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0f;
            }
        }
        yield return reloading = false;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void GetValues()
    {
        //movement bullshit
        walkSpeed = gunData.walkSpeed;
        walkSpeedAim = gunData.walkSpeedAim;
        sprintSpeed = gunData.sprintSpeed;
        jumpHeight = gunData.jumpHeight;

        //weapon bullshit
        damage = gunData.damage;
        range = gunData.range;
        fireRate = gunData.fireRate;
        fireSound = gunData.fireSound;

        currentAmmo = gunData.magAmmo;
        magAmmo = gunData.magAmmo;
        reserveAmmo = gunData.reserveAmmo;
        reloadTimeEmpty = gunData.reloadTimeEmpty;
        reloadTimeTactical = gunData.reloadTimeTactical;

        fovMain = gunData.fovMain;
        fovScoped = gunData.fovScoped;
        smoothingFactor = gunData.smoothingFactor;

        normalPosition = gunData.normalPosition;
        aimedPosition = gunData.aimedPosition;
        weaponKick = gunData.weaponKick;
        weaponAimKick = gunData.weaponAimKick;
        weaponKickRot = gunData.weaponKickRot;
        weaponAimKickRot = gunData.weaponAimKickRot;
        boltOffset = gunData.boltOffset;

        //sway bullshit
        amount = gunData.amount;
        maxAmount = gunData.maxAmount;

        rotationAmount = gunData.rotationAmount;
        maxRotationAmount = gunData.maxRotationAmount;
        
        smoothAmount = gunData.smoothAmount;
        smoothRotation = gunData.smoothRotation;

        //recoil bullshit
        rotationX = gunData.rotationX;
        rotationY = gunData.rotationY;
        rotationZ = gunData.rotationZ;

        minRecoilX = gunData.minRecoil.x;
        minRecoilY = gunData.minRecoil.y;
        minRecoilZ = gunData.minRecoil.z;
        
        maxRecoilX = gunData.maxRecoil.x;
        maxRecoilY = gunData.maxRecoil.y;
        maxRecoilY = gunData.maxRecoil.z;

        aimMinRecoilX = gunData.aimMinRecoil.x;
        aimMinRecoilY = gunData.aimMinRecoil.y;
        aimMinRecoilZ = gunData.aimMinRecoil.z;
                                            
        aimMaxRecoilX = gunData.aimMaxRecoil.x;
        aimMaxRecoilY = gunData.aimMaxRecoil.y;
        aimMaxRecoilZ = gunData.aimMaxRecoil.z;

        snappiness = gunData.snappiness;
        returnSpeed = gunData.returnSpeed;
    }

    void PlayerMove()
    {
        bool grounded = characterController.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -0.5f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (!paused)
        {
            move = transform.right * x + transform.forward * z;

            if (Input.GetButton("Fire2"))
            {
                characterController.Move(move * walkSpeedAim * Time.deltaTime);
                walking = false;
                sprinting = false;
                aiming = true;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                characterController.Move(move * sprintSpeed * Time.deltaTime);
                if (move == Vector3.zero)
                {
                    sprinting = false;
                }
                else
                {
                    sprinting = true;
                }
                walking = false;
                aiming = false;
            }
            else
            {
                characterController.Move(move * walkSpeed * Time.deltaTime);
                if (move == Vector3.zero)
                {
                    walking = false;
                }
                else
                {
                    walking = true;
                }
                sprinting = false;
                aiming = false;
            }
        }

        if (grounded && Input.GetButton("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    void CameraLook(float x, float y)
    {
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * x);
    }

    private void TiltAndSway(float x, float y)
    {
        float tiltY = Mathf.Clamp(-x * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(-y * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            rotationX ? -tiltX : 0f,
            rotationY ? tiltY : 0f,
            rotationZ ? tiltY : 0
            ));

        swayer.localRotation = Quaternion.Slerp(swayer.localRotation, finalRotation * initSwayRot, Time.smoothDeltaTime * smoothRotation);

        float moveX = Mathf.Clamp(-x * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(-y * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        swayer.localPosition = Vector3.Lerp(swayer.localPosition, finalPosition + initSwayPos, Time.smoothDeltaTime * smoothAmount);
    }
}