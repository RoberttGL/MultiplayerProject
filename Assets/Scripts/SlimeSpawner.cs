using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] float spawnRate;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;
        SpawnSlimeServerRpc();
        StartCoroutine(SpawnSlimesCoroutine());
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }


    [ServerRpc]
    private void SpawnSlimeServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject newProjectil = Instantiate(slimePrefab, GetComponentInParent<Transform>().position, Quaternion.identity);
        newProjectil.transform.position = transform.position;
        newProjectil.GetComponent<NetworkObject>().Spawn();
    }

    IEnumerator SpawnSlimesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            SpawnSlimeServerRpc();
        }

    }
}
