using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Slime : NetworkBehaviour
{

    [SerializeField] Slider healthBar;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] float maxHealth;
    [SerializeField] NetworkVariable<float> currentHealth = new NetworkVariable<float>(100);
    private float direction;

    [SerializeField] private GameObject drop;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;

        currentHealth.OnValueChanged += HealthBarCallbackModificacioClientRpc;

        rigidBody.velocity = new Vector3(1, 0, 0);
        StartPatrolServerRpc();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        currentHealth.OnValueChanged -= HealthBarCallbackModificacioClientRpc;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner)
            return;
        if (collision.transform.tag == "Proyectil")
        {
            ReceiveDmgServerRpc(collision.GetComponent<Proyectil>().ProyectilDamage);
        }
    }


    [ServerRpc]
    void ReceiveDmgServerRpc(float dmg)
    {
        currentHealth.Value -= dmg;
        if (currentHealth.Value <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(DieCoroutine());


        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        MoveSlimeServerRpc();
    }

    [ServerRpc]
    private void MoveSlimeServerRpc()
    {
        rigidBody.velocity = new Vector2(direction, 0);
    }

    [ClientRpc]
    private void HealthBarCallbackModificacioClientRpc(float oldValue, float newValue)
    {
        healthBar.value = newValue;
    }




    [ServerRpc]
    private void StartPatrolServerRpc()
    {
        StartCoroutine(SlimePatrol());
    }
    [ServerRpc]
    private void SlimeDespawnServerRpc()
    {
        GameObject newDdrop = Instantiate(drop, GetComponentInParent<Transform>().position, Quaternion.identity);
        newDdrop.GetComponent<NetworkObject>().Spawn();
        newDdrop.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2, 3), Random.Range(-2, -5));
        GetComponent<NetworkObject>().Despawn();
    }

    IEnumerator SlimePatrol()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (Random.Range(0, 2) == 0)
            {
                direction = 1;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                direction = -1;
                GetComponent<SpriteRenderer>().flipX = false;
            }



        }

    }
    IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(0f);
        SlimeDespawnServerRpc();
    }

}