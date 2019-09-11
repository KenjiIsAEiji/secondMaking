using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [プログラム概要]
/// ・キャラクターが持つ銃の向きを制御
/// ・キャラクターの持つ手の位置をIKで制御
/// </summary>
public class IKController : MonoBehaviour
{
    // IK制御のためのAnimatorを格納
    Animator animator;

    // カメラが狙う位置を取得
    [SerializeField] ShooterController shooterController;

    // 銃の位置
    [SerializeField] Transform GunTransform;
    // 右手と左手の持つ位置を指定
    [SerializeField] Transform rightHand, leftHand;

    // キャラクターが銃を向けるスピード
    [SerializeField] float TargettingSpeed;

    // 銃が向けるポジション
    Vector3 target;

    // Startメソッド -----------------------------------------------------------------------------------------
    void Start()
    {
        animator = GetComponent<Animator>();        // Animatorコンポーネント取得
        target = shooterController.TargettingPosition;　// targetの初期値はカメラが狙う位置
    }

    // Updateメソッド ----------------------------------------------------------------------------------------
    private void Update()
    {
        // ターゲットをカメラの狙う位置にスムーズに移行
        target = Vector3.Lerp(target, shooterController.TargettingPosition, TargettingSpeed * Time.deltaTime);
    }

    private void OnAnimatorIK()
    {
        if (animator.GetBool("Sprint") == false)
        {
            // スプリント時でないときには、銃と頭をカメラの狙う位置に向ける
            GunTransform.LookAt(target);

            SetHandIK();

            animator.SetLookAtWeight(1.0f, 0.0f, 1.0f, 1.0f, 0f);
            animator.SetLookAtPosition(target);
        }
        else
        {
            // それ以外は手のIKのみ指定
            SetHandIK();
        }
    }

    /// <summary>
    /// 左右の手を銃の指定した位置にIKで固定
    /// </summary>
    private void SetHandIK()
    {
        // 左右の手のIKのウェイトを設定
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

        // 左右の指定した位置と向きに移動・回転
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
    }
}
