using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] public TextMeshProUGUI textFluidos;
    [SerializeField] Vector3 traderPosition;
    [SerializeField] GameObject trader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!IsServer) return;
        SpawnTraderServerRpc();
    }



    [ServerRpc]
    private void SpawnTraderServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject newProjectil = Instantiate(trader, traderPosition, Quaternion.identity);
        newProjectil.GetComponent<NetworkObject>().Spawn();
    }

}
