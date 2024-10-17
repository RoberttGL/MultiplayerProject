using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FluidosDeSlime : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;

    [ServerRpc]
    public void OnCollectServerRpc()
    {
        networkObject.Despawn();
    }
}
