using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public float range;
    public TrailRenderer shootingLine;
    public Transform shootingPoint;
    private Camera _cam;
    private WaitForSeconds _shotDuration = new WaitForSeconds(0.07f);

    private float nextFire;

    private void Start()
    {
        _cam = GetComponentInParent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(shootingLine, shootingPoint.position, shootingPoint.rotation);
            if(Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit Hit))
            {
                
            }
        }
    }

}
