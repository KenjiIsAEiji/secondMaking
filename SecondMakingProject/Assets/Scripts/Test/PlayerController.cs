using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を地上状態(State.Ground)と浮遊状態(State.Hover)として、ステート管理
/// 
/// </summary>

public class PlayerController : MonoBehaviour
{
    CharacterController character;

    [SerializeField] float Gravity = 20.0f;
    [SerializeField] float JumpPower = 15.0f;
    [SerializeField] float JetPower = 10;

    [SerializeField] float NomalSpeed = 10.0f;

    [SerializeField] float HoverSwitchTime = 0.5f;

    Vector3 Moving = Vector3.zero;

    [SerializeField] float GroundedHight;

    float Move_x;
    float Move_z;
    float flytime;

    //プレイヤーの状態を定義
    enum State
    {
        Ground,
        Hover
    }

    State PlayerState;

    // Start is called before the first frame update
    void Start()
    {
        PlayerState = State.Ground;
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move_x = Input.GetAxis("Horizontal");
        Move_z = Input.GetAxis("Vertical");

        switch (PlayerState)
        {
            case State.Ground:
                Debug.Log("now Ground");
                PlayerIsGround();
                break;

            case State.Hover:
                Debug.Log("now Hover");
                PlayerIsHover();
                break;        
        }

        character.Move(transform.TransformDirection(Moving * Time.deltaTime));

    }


    private void PlayerIsGround()
    {
        Moving.y -= Gravity * Time.deltaTime;

        if (Physics.Raycast(transform.position, -transform.up, GroundedHight)){
            Debug.Log("IsGrounded");

            flytime = 0.0f;

            Moving = new Vector3(Move_x * NomalSpeed, Moving.y, Move_z * NomalSpeed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Moving.y = JumpPower;
            }
        }
        else
        {
            flytime += Time.deltaTime;

            if (flytime >= HoverSwitchTime && Input.GetKeyDown(KeyCode.Space))
            {
                PlayerState = State.Hover;
            }
        }
    }

    private void PlayerIsHover()
    {
        Moving = new Vector3(Move_x, 0, Move_z) * NomalSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            Moving.y = JetPower;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PlayerState = State.Ground;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, -transform.up * GroundedHight);
    }
}
