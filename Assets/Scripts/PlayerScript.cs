using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] NetworkVariable<float> speed = new NetworkVariable<float>(1);

    [SerializeField] NetworkVariable<float> rotacion = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] NetworkVariable<float> jumpForce = new NetworkVariable<float>(1f);
    [SerializeField] NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(true);
    [SerializeField] NetworkVariable<int> fluidos = new NetworkVariable<int>(0);


    Rigidbody2D m_RigidBody;
    Vector2 movement = Vector2.zero;
    public GameObject playerCamera;
    public Transform orientation;
    public bool readyToJump = true;



    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        rotacion.OnValueChanged += PlayerRotationCallback;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;

        fluidos.OnValueChanged += FluidosCallback;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        fluidos.OnValueChanged -= FluidosCallback;
    }



    private void FluidosCallback(int oldValue, int newValue)
    {
        GameManager.Instance.textFluidos.text = fluidos.Value + "";
        Debug.Log(string.Format("He obtenido 1 fluido!"));
    }

    private void PlayerRotationCallback(float oldValue, float newValue)
    {
        orientation.transform.rotation = new Quaternion(0, rotacion.Value, 0, 0);
    }



    void Update()
    {


        if (!IsOwner)
            return;


        movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movement += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            movement -= Vector2.up;
        if (Input.GetKey(KeyCode.A))
            movement -= Vector2.right;
        if (Input.GetKey(KeyCode.D))
            movement += Vector2.right;


        if (movement != Vector2.zero)
        {
            PlayerMovementServerRpc(movement.normalized * speed.Value);
            if (movement.x > 0)
                rotacion.Value = 0;
            else if (movement.x < 0)
                rotacion.Value = 180;
        }


        else
            StopMovementServerRpc();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded.Value)
        {
            ChangeIsGroundedServerRpc(false);
            readyToJump = false;
            StartCoroutine(JumpCooldown());
            PlayerJumpServerRpc(new ServerRpcParams());
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 1f, 7);
        if (readyToJump && hit.transform?.tag == "Ground")
        {
            ChangeIsGroundedServerRpc(true);
        }



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;
        if (collision.transform.tag == "Fluidos")
        {
            collision.gameObject.GetComponent<FluidosDeSlime>().OnCollectServerRpc();
            fluidos.Value++;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (collision.transform.tag == "Trader")
        {
            collision.gameObject.GetComponent<Trader>().GetFluidosServerRpc(fluidos.Value);
            fluidos.Value = 0;
        }
    }


    // MOVEMENT
    [ServerRpc]
    private void PlayerMovementServerRpc(Vector2 velocity, ServerRpcParams serverRpcParams = default)
    {
        m_RigidBody.velocity = new Vector2(velocity.x, m_RigidBody.velocity.y);
    }
    // STOP MOVEMENT
    [ServerRpc]
    private void StopMovementServerRpc(ServerRpcParams serverRpcParams = default)
    {
        m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
    }



    //JUMP
    [ServerRpc]
    private void PlayerJumpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        m_RigidBody.AddForce(transform.up * jumpForce.Value);
    }

    //CheckGrounded
    [ServerRpc(RequireOwnership = false)]
    private void ChangeIsGroundedServerRpc(bool is_grounded, ServerRpcParams serverRpcParams = default)
    {
        {
            isGrounded.Value = is_grounded;
        }
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        readyToJump = true;
    }



}

