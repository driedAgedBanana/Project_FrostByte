using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;

    public GameObject bulletPrefab;
    public int poolSize = 7;

    private Queue<GameObject> _bulletPool;

    private void Awake()
    {
        if (instance == null) // Singleton but with extra steps
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bulletPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            // Instantiate the bullet and ensure its position and rotation are reset
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);

            bullet.transform.position = Vector3.zero; // Reset position
            bullet.transform.rotation = Quaternion.identity; // Reset rotation

            _bulletPool.Enqueue(bullet); // Add it to the pool
        }
    }


    public GameObject GetBullet()
    {
        if (_bulletPool.Count > 0)
        {
            GameObject bullet = _bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);

        if (_bulletPool.Count < poolSize)
        {
            _bulletPool.Enqueue(bullet);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
