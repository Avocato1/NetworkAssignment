using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootBullet : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    void Update()
    {
        if(!IsOwner) return;
        if (Input.GetButtonDown("Fire1"))
        {
            ShootServerRpc();
        }
    }
    
    [ServerRpc]
    private void ShootServerRpc()
    {
        //spawn bullet on server
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Projectile>().parent = this;
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}
