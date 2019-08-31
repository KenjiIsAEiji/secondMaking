using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <プログラム概要>
/// プレイヤーの状態を地上状態(State.Ground)と浮遊状態(State.Hover)として、ステート管理
/// ステートに応じて移動やアニメーションを変更
/// 
/// 
/// </summary>

public class PlayerController : MonoBehaviour
{
    // 基本的なキャラクターの移動として、CharacterControllerを使用
    CharacterController character;

    // 重力とジャンプ・浮遊力の設定
    [SerializeField] float Gravity = 20.0f;
    [SerializeField] float JumpPower = 15.0f;
    [SerializeField] float JetPower = 10;

    // キャラクターの向きの取得と向きの変更スピード、浮遊スプリント時に上向きになる角度
    [SerializeField] Transform BodyTransform;
    [SerializeField] float turnSpeed = 10;
    [SerializeField] float lazeAngle = 10.0f;

    // カメラの向きの取得用
    [SerializeField] Transform CamTransform;

    // 移動速度の設定（通常、地上・空中でのスプリント時の速度の倍率）
    [SerializeField] float NomalSpeed = 10.0f;
    [SerializeField] float SprintScaleFactor = 2.0f;
    [SerializeField] float AirSprintScaleFactor = 3.0f;

    // 空中になってからHover状態に推移できる時間
    [SerializeField] float HoverSwitchTime = 0.5f;
    float flytime;  // 浮遊時間を格納する用

    // 地面に接地判定をするRayの長さ
    [SerializeField] float GroundedHight;

    // Hover状態でのスプリント時に当たり判定の半径を変更する値
    [SerializeField] float AirSprintRadius = 1.77f;
    float defaltRadius;　// デフォルトの半径格納用

    // アニメション制御用
    [SerializeField] Animator animator;

    // CharacterControllerに渡す移動量
    Vector3 Moving = Vector3.zero;

    // キーボードからの入力格納用
    float Move_x;
    float Move_z;

    //プレイヤーの状態を定義
    enum State
    {
        Ground,
        Hover
    }

    // ステート定義
    State PlayerState;

    /// startメソッド --------------------------------------------------------------------
    void Start()
    {
        PlayerState = State.Ground;
        character = GetComponent<CharacterController>();
        defaltRadius = character.radius;
    }

    // Updateメソッド---------------------------------------------------------------------
    void Update()
    {
        // キーボード入力を格納
        Move_x = Input.GetAxis("Horizontal");
        Move_z = Input.GetAxis("Vertical");

        // アニメーションのblendTree制御のためキーボード入力を渡す
        animator.SetFloat("moving_x", Move_x);
        animator.SetFloat("moving_z", Move_z);

        // 各ステートの処理の分岐---------------------------------------
        switch (PlayerState)
        {
            case State.Ground:
                Debug.Log("now Ground");
                animator.SetBool("Hovering", false);    // アニメーションでHover状態の解除
                PlayerIsGround();
                break;

            case State.Hover:
                Debug.Log("now Hover");
                animator.SetBool("Hovering", true);     // アニメーションでHover状態に
                PlayerIsHover();
                break;
        }

        // カメラの向きを取得し、プレイヤーの向きと同期
        transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                CamTransform.eulerAngles.y,
                transform.eulerAngles.z
            );

        // 最終的な移動ベクトルをワールドプレイヤーからのワールド座標に変換し、キャラクターを移動
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

    private void BodyTurnUp()
    {
        BodyTransform.localRotation = Quaternion.Slerp(
            BodyTransform.localRotation,
            Quaternion.LookRotation(new Vector3(Move_x, lazeAngle, Move_z)),
            turnSpeed * Time.deltaTime
        );
    }


    private void BodyAirTurn()
    {
        BodyTransform.localRotation = Quaternion.Slerp(
            BodyTransform.localRotation,
            Quaternion.Euler(Move_z * 15, 0, Move_x * -15),
            turnSpeed * Time.deltaTime
        );
    }

    private void PlayerIsGround()
    {
        character.radius = defaltRadius;
        Moving.y -= Gravity * Time.deltaTime;

        if (Physics.Raycast(transform.position, -transform.up, GroundedHight)){
            Debug.Log("IsGrounded");
            animator.SetBool("IsGround", true);

            // Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Sprint", true);
                Moving = new Vector3(
                    Move_x * NomalSpeed * SprintScaleFactor,
                    Moving.y,
                    Move_z * NomalSpeed * SprintScaleFactor
                );

                BodyTurn();
            }
            else
            {
                animator.SetBool("Sprint", false);
                Moving = new Vector3(Move_x * NomalSpeed, Moving.y, Move_z * NomalSpeed);
                BodyNomal();
            }
            

            flytime = 0.0f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Moving.y = JumpPower;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
            animator.SetBool("Sprint", false);

            flytime += Time.deltaTime;

            if (flytime >= HoverSwitchTime && Input.GetKeyDown(KeyCode.Space))
            {
                PlayerState = State.Hover;
            }
        }
    }

    // Hover状態
    private void PlayerIsHover()
    {
        Moving = new Vector3(Move_x, 0, Move_z) * NomalSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("Sprint", true);
            Moving = new Vector3(
                Move_x * NomalSpeed * AirSprintScaleFactor,
                0,
                Move_z * NomalSpeed * AirSprintScaleFactor
            );

            if (Input.GetKey(KeyCode.Space))
            {
                BodyTurnUp();
            }

            BodyTurn();

            character.radius = AirSprintRadius;
        }
        else
        {
            animator.SetBool("Sprint", false);
            Moving = new Vector3(Move_x * NomalSpeed, 0, Move_z * NomalSpeed);
            BodyAirTurn();
            character.radius = defaltRadius;
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
