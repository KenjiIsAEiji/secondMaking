using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <プログラム概要>
/// プレイヤーの状態を地上状態(State.Ground)と浮遊状態(State.Hover)として、ステート管理
/// ステートに応じて移動やアニメーションを変更
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
        PlayerState = State.Ground;             //初期ステートはGround
        character = GetComponent<CharacterController>();    // キャラコンのコンポーネント取得
        defaltRadius = character.radius;        // デフォルトの当たり判定の半径を格納
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

    /// <summary>
    /// [地上での動きの概要]
    /// ・前後左右の動きとジャンプでの移動
    /// ・重力適用
    /// ・Shiftキーでスプリント
    /// ・通常はカメラに対して同じ向き、スプリント時は移動方向にキャラクターを向かせる
    /// ・Spaceキーでジャンプ、空中に一定時間以上いたときにもう一度SpaceでHover状態に移行
    /// </summary>
    private void PlayerIsGround()
    {
        character.radius = defaltRadius;        // 地上では常にデフォルトの当たり判定
        Moving.y -= Gravity * Time.deltaTime;

        // 接地判定にはRaycastを使用
        if (Physics.Raycast(transform.position, -transform.up, GroundedHight)){
            Debug.Log("IsGrounded");
            animator.SetBool("IsGround", true);

            // Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Move_x != 0.0f || Move_z != 0.0f)
                {
                    animator.SetBool("Sprint", true);
                }
                else
                {
                    animator.SetBool("Sprint", false);
                }

                // スプリント時は移動スピードにスプリント倍率を適用
                Moving = new Vector3(
                    Move_x * NomalSpeed * SprintScaleFactor,
                    Moving.y,
                    Move_z * NomalSpeed * SprintScaleFactor
                );

                // スプリント時のキャラクターは、移動方向を向く
                BodyTurn(Quaternion.LookRotation(new Vector3(Move_x, 0, Move_z)));
            }
            else
            {
                animator.SetBool("Sprint", false);
                Moving = new Vector3(Move_x * NomalSpeed, Moving.y, Move_z * NomalSpeed);
                BodyTurn(Quaternion.LookRotation(new Vector3(0, 0, 0)));        // 通常時のキャラクターは、カメラを向く
            }
            
            flytime = 0.0f;         // 浮遊時間をリセット
            if (Input.GetKeyDown(KeyCode.Space))        // Spaceキーでジャンプ
            {
                Moving.y = JumpPower;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
            animator.SetBool("Sprint", false);

            // 空中にいる時間をカウント
            flytime += Time.deltaTime;

            // 一定時間後にSpaceキーでHover状態に移行
            if (flytime >= HoverSwitchTime && Input.GetKeyDown(KeyCode.Space))
            {
                PlayerState = State.Hover;
            }
        }
    }

    /// <summary>
    /// [空中での動きの概要]
    /// ・前後左右と上昇
    /// ・重力なし
    /// ・Shiftキーでスプリント
    /// ・通常はカメラに対して同じ向き、スプリント時は移動方向にキャラクターを向かせる
    /// ・Spaceキーで上昇
    /// ・CtrlキーでHover状態解除（Ground状態に移行）
    /// </summary>
    private void PlayerIsHover()
    {
        Moving = new Vector3(Move_x, 0, Move_z) * NomalSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Move_x != 0.0f || Move_z != 0.0f)
            {
                animator.SetBool("Sprint", true);
                
                // 空中スプリント時はキャラクターの当たり判定の半径を変更
                character.radius = AirSprintRadius;
            }
            else
            {
                animator.SetBool("Sprint", false);
                character.radius = defaltRadius;
            }

            // スプリント時は移動スピードに、空中時のスプリント倍率を適用
            Moving = new Vector3(
                Move_x * NomalSpeed * AirSprintScaleFactor,
                0,
                Move_z * NomalSpeed * AirSprintScaleFactor
            );

            // スプリント時の上昇では、上向きに角度を
            if (Input.GetKey(KeyCode.Space))
            {
                BodyTurn(Quaternion.LookRotation(new Vector3(Move_x, lazeAngle, Move_z)));
            }

            BodyTurn(Quaternion.LookRotation(new Vector3(Move_x, 0, Move_z)));

            
        }
        else
        {
            animator.SetBool("Sprint", false);
            Moving = new Vector3(Move_x * NomalSpeed, 0, Move_z * NomalSpeed);

            // 空中での通常移動では移動方向に傾く
            BodyTurn(Quaternion.Euler(Move_z * 15, 0, Move_x * -15));

            character.radius = defaltRadius;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Moving.y = JetPower;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))      // CtrlキーでGround状態に移行
        {
            PlayerState = State.Ground;
        }
    }

    ///------------------------------------------------------------------------------------------------
    /// キャラクターの向きを制御するメソッド
    ///------------------------------------------------------------------------------------------------
    private void BodyTurn(Quaternion Target)
    {
        BodyTransform.localRotation = Quaternion.Slerp(
            BodyTransform.localRotation,
            Target,
            turnSpeed * Time.deltaTime
        );
    }


    // 接地判定を視覚的に設定できるように、Gizmosを使用し、シーンビューで視覚化
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, -transform.up * GroundedHight);
    }
}
