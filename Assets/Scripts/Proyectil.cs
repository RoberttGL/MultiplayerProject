using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Proyectil : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;
    [SerializeField] float proyectilDamage;

    public float ProyectilDamage { get => proyectilDamage; }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            StartCoroutine(TimeToDespawnProyectile());

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner)
            return;
        if (collision.transform.tag != "Player")
        {
            DestroyProyectilServerRpc();
        }
    }

    [ServerRpc]
    private void DestroyProyectilServerRpc()
    {
        networkObject.Despawn();
    }

    IEnumerator TimeToDespawnProyectile()
    {
        yield return new WaitForSeconds(5f);
        DestroyProyectilServerRpc();
    }

}
