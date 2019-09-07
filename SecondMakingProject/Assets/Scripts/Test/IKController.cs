using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [動きの概要]
/// 
/// </summary>
public class IKController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    Animator animator;

    [SerializeField] ShooterController shooterController;

    [SerializeField] Transform GunTransform;
    [SerializeField] Transform rightHand, leftHand;

    [SerializeField] float TargettingSpeed;

    Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        target = shooterController.TargettingPosition;
    }

    private void Update()
    {
        target = Vector3.Lerp(target, shooterController.TargettingPosition, TargettingSpeed * Time.deltaTime);
    }

    private void OnAnimatorIK()
    {
        if(animator.GetBool("Sprint") == false)
        {
            GunTransform.LookAt(target);

            SetHandIK();

            animator.SetLookAtWeight(1.0f, 0.0f, 1.0f, 1.0f, 0f);
            animator.SetLookAtPosition(target);
        }
        else
        {
            SetHandIK();
        }
    }

    private void SetHandIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
    }
}
