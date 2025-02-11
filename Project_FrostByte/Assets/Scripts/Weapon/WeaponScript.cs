using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Aiming Mechanics")]
    public Transform weapon;
    public Transform defaultPos;
    public Transform aimingPos;
    public float aimSpeed;

    float AimTime;


    [Header("Shooting Mechanics")]
    public int damage;
    public float fireRate;
    public float range;
    public GameObject shootingLine;
    public Transform shootingPoint;
    private Camera _cam;
    private WaitForSeconds _shotDuration = new WaitForSeconds(0.07f);

    public float bulletSpread;

    private float nextFire;

    private void Start()
    {
        _cam = GetComponentInParent<Camera>();

        weapon.position = defaultPos.position;
        weapon.rotation = defaultPos.rotation;
    }

    private void Update()
    {
        Aiming();
        Shoot();
    }

    private void Aiming()
    {
        if (Input.GetKey(KeyCode.Mouse1) && AimTime < 1)
        {
            AimTime += Time.deltaTime * aimSpeed;
        }
        else if(!Input.GetKey(KeyCode.Mouse1) && AimTime > 0)
        {
            AimTime -= Time.deltaTime * aimSpeed;
        }

        weapon.position = Vector3.Lerp(defaultPos.position, aimingPos.position, AimTime);
        weapon.rotation = Quaternion.Lerp(defaultPos.rotation, aimingPos.rotation, AimTime);
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 fwd = _cam.transform.forward;
            fwd = fwd + _cam.transform.TransformDirection(new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread)));


            if (Physics.Raycast(_cam.transform.position, fwd, out RaycastHit Hit))
            {

            }

            GameObject Tracer = Instantiate(shootingLine, shootingPoint.position, shootingPoint.rotation);
            Tracer.transform.LookAt(fwd * 100);
        }
    }

}
