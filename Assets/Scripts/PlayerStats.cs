using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>();
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int winKillCount = 10;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI winText;
    public NetworkVariable<int> killCount = new NetworkVariable<int>();
    private static Transform[] _cachedSpawnPoints;
    private PlayerSettings _playerSettings;

    void Start()
    {
        _playerSettings = GetComponent<PlayerSettings>(); 
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.Value = maxHealth;

        if (_cachedSpawnPoints == null)
        {
            //Gets all the respawn points and put them in the array
            _cachedSpawnPoints = GameObject.FindGameObjectsWithTag("Respawn")
                .Select(go => go.transform)
                .ToArray();
        }

        spawnPoints = _cachedSpawnPoints;

        if (IsOwner)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            UpdatePositionServerRpc(OwnerClientId, spawnIndex);
        }
    }

    public void TakeDamage(int damage, PlayerStats attacker)
    {
        if (!IsServer) return; 

        health.Value -= damage;

        if (health.Value <= 0)
        {
            if (attacker != null)
            {
                attacker.killCount.Value++;
                UpdateKillCountClientRpc(attacker.killCount.Value, attacker.OwnerClientId);

                if (attacker.killCount.Value >= winKillCount)
                {
                    string winnerName = attacker._playerSettings.networkPlayerName.Value.ToString();  
                   AnnounceWinnerClientRpc(winnerName,attacker.OwnerClientId);
                }
            }

            Respawn();
        }
    }

    private void Respawn()
    {
        if (!IsServer) return; 
        //Choose a random spawnpoint 
        Vector3 randomSpawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        health.Value = maxHealth;

        UpdatePositionClientRpc(randomSpawnPosition);
        gameObject.transform.position = randomSpawnPosition;
        ResetRbVelocity();
    }

    private void ResetRbVelocity()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc(ulong clientId, int spawnIndex)
    {
        //change position to a new transform
        var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        
        playerObject.transform.position = spawnPosition;
        playerObject.transform.rotation = Quaternion.identity; 
        
        UpdatePositionClientRpc(spawnPosition);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        gameObject.transform.position = newPosition;
        ResetRbVelocity();
    }
    
    [ClientRpc]
    private void UpdateKillCountClientRpc(int newKillCount, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (killCountText != null)
            {
                killCountText.text = "Kills: " + newKillCount;
            }
        }
    }

    [ClientRpc]
    private void AnnounceWinnerClientRpc(string winnerName, ulong winnerClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == winnerClientId)
        {
            if (winText != null)
            {
                winText.text = "You Win!";
            }
        }
        else
        {
            if (winText != null)
            {
                winText.text = winnerName + " has won the game!";
            }
        }
    }


}
