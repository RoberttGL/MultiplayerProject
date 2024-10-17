using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class Trader : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> fluidos = new NetworkVariable<int>();

    [SerializeField] TextMeshProUGUI textTrader;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        fluidos.OnValueChanged += FluidosCallbackClientRpc;
        textTrader.text = fluidos.Value + "";
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }


    [ClientRpc]
    private void FluidosCallbackClientRpc(int oldValue, int newValue)
    {
        textTrader.text = newValue + "";
    }



    [ServerRpc]
    public void GetFluidosServerRpc(int num)
    {
        fluidos.Value += num;
    }
}
