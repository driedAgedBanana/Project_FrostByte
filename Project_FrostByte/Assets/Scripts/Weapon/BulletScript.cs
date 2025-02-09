using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 3f;

    private Rigidbody rb;
    private float _timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -direction * speed; // Do not question
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            Destroy(gameObject);
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
