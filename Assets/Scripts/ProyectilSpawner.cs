using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ProyectilSpawner : NetworkBehaviour
{
    [SerializeField] NetworkVariable<bool> onCooldown = new NetworkVariable<bool>(false);
    [SerializeField] private GameObject proyectil;
    [SerializeField] PlayerScript player;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void Update()
    {
        if (!IsOwner)
            return;

        CastProyectil();

    }

    private void CastProyectil()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.C) && !onCooldown.Value)
            SpawnProyectilServerRpc();
    }

    [ServerRpc]
    private void SpawnProyectilServerRpc(ServerRpcParams serverRpcParams = default)
    {
        onCooldown.Value = true;
        GameObject newProjectil = Instantiate(proyectil, GetComponentInParent<Transform>().position, Quaternion.identity);
        newProjectil.GetComponent<NetworkObject>().Spawn();
        if (player.orientation.transform.rotation.y == 0)
            newProjectil.GetComponent<Rigidbody2D>().velocity = new Vector2(5, 0);
        else
            newProjectil.GetComponent<Rigidbody2D>().velocity = new Vector2(-5, 0);
        StartCoroutine(onCooldownCoroutine());
    }

    IEnumerator onCooldownCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        onCooldown.Value = false;
    }
}
