using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 3f;

    private float _timer;

    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        _timer = 0f; // Reset the timer when the bullet is reused
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);

        // Disable the bullet after its lifetime
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision effects if needed
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        // Reset Rigidbody velocity
        rb.linearVelocity = Vector3.zero;

        // Return the bullet to the pool
        BulletPool.instance.ReturnBullet(gameObject);
    }
}
