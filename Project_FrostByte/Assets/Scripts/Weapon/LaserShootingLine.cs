using UnityEngine;

public class LaserShootingLine : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeTime = timer;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
