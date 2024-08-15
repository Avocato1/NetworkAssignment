
using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public ShootBullet parent;
    public float shootForce;
    public int damage = 10;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        rb.velocity = rb.transform.forward * shootForce;
        //auto destroy the bullet if it didnt hit anything
        Invoke(nameof(DestroyIfNotDestroyed), 5f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;
        PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage, parent.GetComponent<PlayerStats>());
        }
        DestroyProjectileServerRpc();
    }

    [ServerRpc]
    private void DestroyProjectileServerRpc()
    {
        //remove the bullet from the server
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
    private void DestroyIfNotDestroyed()
    {
        
        if (NetworkObject.IsSpawned)
        {
            DestroyProjectileServerRpc();
        }
    }
}