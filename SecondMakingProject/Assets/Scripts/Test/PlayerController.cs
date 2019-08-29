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

    [SerializeField] Transform BodyTransform;
    [SerializeField] Transform CamTransform;
    [SerializeField] float turnSpeed = 10;

    [SerializeField] float NomalSpeed = 10.0f;
    [SerializeField] float SprintScaleFactor = 2.0f;

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

        transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                CamTransform.eulerAngles.y,
                transform.eulerAngles.z
            );

        character.Move(transform.TransformDirection(Moving * Time.deltaTime));
    }

    private void BodyTurn()
    {
        BodyTransform.localRotation = Quaternion.Slerp(
            BodyTransform.localRotation,
            Quaternion.LookRotation(new Vector3(Move_x, 0, Move_z)),
            turnSpeed * Time.deltaTime
        );
    }

    private void BodyNomal()
    {
        BodyTransform.localRotation = Quaternion.Slerp(
            BodyTransform.localRotation,
            Quaternion.LookRotation(new Vector3(0, 0, 0)),
            turnSpeed * Time.deltaTime
        );
    }

    private void PlayerIsGround()
    {
        Moving.y -= Gravity * Time.deltaTime;

        if (Physics.Raycast(transform.position, -transform.up, GroundedHight)){
            Debug.Log("IsGrounded");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Moving = new Vector3(
                    Move_x * NomalSpeed * SprintScaleFactor,
                    Moving.y,
                    Move_z * NomalSpeed * SprintScaleFactor
                );

                BodyTurn();
            }
            else
            {
                Moving = new Vector3(Move_x * NomalSpeed, Moving.y, Move_z * NomalSpeed);
                BodyNomal();
            }

            flytime = 0.0f;
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Moving = new Vector3(
                Move_x * NomalSpeed * SprintScaleFactor,
                0,
                Move_z * NomalSpeed * SprintScaleFactor
            );

            BodyTurn();
        }
        else
        {
            Moving = new Vector3(Move_x * NomalSpeed, 0, Move_z * NomalSpeed);
            BodyNomal();
        }

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
