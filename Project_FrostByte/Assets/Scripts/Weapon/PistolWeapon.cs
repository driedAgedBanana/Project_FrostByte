using UnityEngine;

public class PistolWeapon : WeaponBaseManager
{
    public override void HandleFireModes()
    {
        p_canFire = Input.GetKeyDown(_shootKey) && Time.time >= fireRate;

        if (p_canFire)
        {
            Shooting();
            Debug.Log("Fire!");
        }
    }
}
