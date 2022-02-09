using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement player;

    //Bools
    private bool isAiming;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Start()
    {
        player = GameObject.Find("player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        isAiming = player.aiming;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, player.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, player.snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if (isAiming)
        {
            targetRotation += new Vector3(Random.Range(player.aimMinRecoilX, player.aimMaxRecoilX), Random.Range(player.aimMinRecoilY, player.aimMaxRecoilY), Random.Range(player.aimMinRecoilZ, player.aimMaxRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(Random.Range(player.minRecoilX, player.maxRecoilX), Random.Range(player.minRecoilY, player.maxRecoilY), Random.Range(player.minRecoilZ, player.maxRecoilZ));
        }
    }
}