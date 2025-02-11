using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    public float lifeDuration;
    public float lifeTimer;

    private void Start()
    {
        lifeDuration = lifeTimer;
    }

    private void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if ( rb != null )
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            lifeDuration -= Time.deltaTime;
            if (lifeDuration <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("Bullet have no Rigidbody assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
