using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCamera : NetworkBehaviour
{
    public GameObject playerCamera;

    public override void OnNetworkSpawn()
    {
        playerCamera.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }
    private void Update()
    {
        playerCamera.transform.position = transform.position;
    }
}