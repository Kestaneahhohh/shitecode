using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Framework : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform playerBody, camTransform, holder, swayer, bobber;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private AudioSource am;
    [SerializeField]
    private GameObject impactEffect;
    [SerializeField]
    private GunDB guns;
    private GunData gunData;
    [HideInInspector]
    public float mouseSensitivity = 1f, scopedSensitivity = 0.25f;
    private float xRotation = 0f, mouseX, mouseY;
    [SerializeField]
    private GameObject lArm, rArm;
    private GameObject gunModel;

    //Sway bullshit
    private Vector3 initSwayPos;
    private Quaternion initSwayRot;

    //Recoil bullshit
    private Transform RecoilTransform;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Movement bullshit
    [HideInInspector]
    private float gravity = -36.0f;

    [HideInInspector]
    public bool walking, aiming, sprinting = false;
    [HideInInspector]
    public Vector3 velocity, move;

    //Weapon bullshit
    [HideInInspector]
    public float currentAmmo, reserveAmmo, health;
    private float nextTimeToFire;
    private AudioClip fireSound;
    private Vector3 initGunBodyPos, initGunMagPos, initGunBoltPos;
    private Transform gunBody, gunMag, gunBolt;
    private bool reloading = false;

    public int frames = 120;

    [HideInInspector]
    public bool paused;

    private void Awake()
    {
        Application.targetFrameRate = frames;
    }

    void Start()
    {
        RecoilTransform = GameObject.Find("CameraRecoil").transform;
        am = GetComponent<AudioSource>();

        initSwayPos = swayer.localPosition;
        initSwayRot = swayer.localRotation;

        health = 100f;

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
            gunModel = Instantiate(gunData.gunModel);
            gunModel.transform.parent = rArm.transform;
            rArm.transform.GetChild(0).localPosition = gunData.weaponPosOffset;
            rArm.transform.GetChild(0).localEulerAngles = gunData.weaponRotOffset;
            rArm.transform.GetChild(0).localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }

        IEnumerator SpawnArms()
        {
            yield return StartCoroutine(WhichGunAreWeSpawning());

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
        }

        IEnumerator WhichGunAreWeSpawning()
        {
            var toFind = PlayerPrefs.GetString("currentGun");

            foreach (var gun in guns.DB)
            {
                if (gun.name == toFind)
                {
                    gunData = gun;

                    break;
                }
            }

            GetValues();

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

                holder.localPosition = Vector3.Lerp(holder.localPosition, gunData.aimedPosition, gunData.scopeSpeed * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, gunData.fovScoped, gunData.scopeSpeed * Time.deltaTime);
            }
            else
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                if (!sprinting && !aiming)
                {
                    holder.localPosition = Vector3.Lerp(holder.localPosition, gunData.normalPosition, gunData.scopeSpeed * Time.deltaTime);
                }
                else if (sprinting && !aiming)
                {
                    holder.localPosition = Vector3.Lerp(holder.localPosition, gunData.sprintPosition, gunData.scopeSpeed * Time.deltaTime);
                }
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, gunData.fovMain, gunData.scopeSpeed * Time.deltaTime);
            }

            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0f && !reloading)
            {
                nextTimeToFire = Time.time + 60f / gunData.fireRate;
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo != gunData.magAmmo && reserveAmmo != 0f && !reloading)
            {
                StartCoroutine(Reload());
            }

            CameraLook(mouseX, mouseY);

            TiltAndSway(mouseX, mouseY);

            gunBolt.localPosition = Vector3.Lerp(gunBolt.localPosition, initGunBoltPos, gunData.boltSpeed * Time.deltaTime);

            targetRotation = Vector3.Slerp(targetRotation, Vector3.zero, gunData.returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, gunData.snappiness * Time.deltaTime);
            RecoilTransform.localRotation = Quaternion.Euler(currentRotation);

            if (reloading)
            {
                Vector3 v = Vector3.zero;
                gunMag.transform.localPosition = Vector3.SmoothDamp(gunMag.transform.localPosition, new Vector3(0.333f, 0, 0), ref v, Time.deltaTime * 10f);
                lArm.transform.localPosition = Vector3.SmoothDamp(lArm.transform.localPosition, gunData.lArmOffset - new Vector3(1f, 0, 0), ref v, Time.deltaTime * 10f);
            }
            else if (!reloading)
            {
                Vector3 v = Vector3.zero;
                gunMag.transform.localPosition = Vector3.SmoothDamp(gunMag.transform.localPosition, Vector3.zero, ref v, Time.deltaTime * 5f);
                lArm.transform.localPosition = Vector3.SmoothDamp(lArm.transform.localPosition, gunData.lArmOffset, ref v, Time.deltaTime * 5f);
            }
        }
    }

    void Shoot()
    {
        Recoil();
        am.pitch = Random.Range(1f, 1.15f);
        am.PlayOneShot(fireSound, 0.5f);

        if (!aiming)
        {
            gunBolt.localPosition = gunData.boltOffset;
            swayer.localPosition += gunData.weaponKick;
            swayer.localEulerAngles += gunData.weaponKickRot;
        }
        else
        {
            gunBolt.localPosition = gunData.boltOffset;
            swayer.localPosition += gunData.weaponAimKick;
            swayer.localEulerAngles += gunData.weaponAimKickRot;
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, gunData.range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(gunData.damage);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.5f);
        }

        currentAmmo--;
    }

    IEnumerator Reload()
    {
        reloading = true;

        if (currentAmmo != gunData.magAmmo && currentAmmo > 0f)
        {
            yield return new WaitForSeconds(gunData.reloadTimeTactical);

            if (reserveAmmo + currentAmmo > gunData.magAmmo)
            {
                reserveAmmo -= (gunData.magAmmo - currentAmmo);
                currentAmmo = gunData.magAmmo;
            }
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0f;
            }
        }
        else if (currentAmmo == 0f)
        {
            yield return new WaitForSeconds(gunData.reloadTimeEmpty);

            if (reserveAmmo + currentAmmo > gunData.magAmmo)
            {
                reserveAmmo -= (gunData.magAmmo - currentAmmo);
                currentAmmo = gunData.magAmmo;
            }
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0f;
            }
        }
        yield return reloading = false;
    }

    void Recoil()
    {
        if (aiming)
        {
            targetRotation += new Vector3(Random.Range(gunData.aimMinRecoil.x, gunData.aimMaxRecoil.x), Random.Range(gunData.aimMinRecoil.y, gunData.aimMaxRecoil.y), Random.Range(gunData.aimMinRecoil.z, gunData.aimMaxRecoil.z));
        }
        else
        {
            targetRotation += new Vector3(Random.Range(gunData.minRecoil.x, gunData.maxRecoil.x), Random.Range(gunData.minRecoil.y, gunData.maxRecoil.y), Random.Range(gunData.minRecoil.z, gunData.maxRecoil.z));
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void GetValues()
    {
        fireSound = gunData.fireSound;
        currentAmmo = gunData.magAmmo;
        reserveAmmo = gunData.reserveAmmo;
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
                characterController.Move(move * gunData.walkSpeedAim * Time.deltaTime);
                walking = false;
                sprinting = false;
                aiming = true;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                characterController.Move(move * gunData.sprintSpeed * Time.deltaTime);
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
                characterController.Move(move * gunData.walkSpeed * Time.deltaTime);
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
            velocity.y = Mathf.Sqrt(gunData.jumpHeight * -2f * gravity);
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

    void TiltAndSway(float x, float y)
    {
        float tiltY = Mathf.Clamp(-x * gunData.rotationAmount, -gunData.maxRotationAmount, gunData.maxRotationAmount);
        float tiltX = Mathf.Clamp(-y * gunData.rotationAmount, -gunData.maxRotationAmount, gunData.maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            gunData.rotationX ? -tiltX : 0f,
            gunData.rotationY ? tiltY : 0f,
            gunData.rotationZ ? tiltY : 0
            ));

        swayer.localRotation = Quaternion.Slerp(swayer.localRotation, finalRotation * initSwayRot, Time.fixedDeltaTime * gunData.smoothRotation);

        float moveX = Mathf.Clamp(-x * gunData.amount, -gunData.maxAmount, gunData.maxAmount);
        float moveY = Mathf.Clamp(-y * gunData.amount, -gunData.maxAmount, gunData.maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        swayer.localPosition = Vector3.Lerp(swayer.localPosition, finalPosition + initSwayPos, Time.fixedDeltaTime * gunData.smoothAmount);
    }
}