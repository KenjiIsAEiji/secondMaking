using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsLookCamera : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    Animator animator;

    [SerializeField] Transform ControlCamera;

    [SerializeField] Transform GunTransform;
    [SerializeField] Transform rightHand, leftHand;

    //[SerializeField] TPSCamera TpsCamera;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    private void OnAnimatorIK()
    {
        if(animator.GetBool("Sprint") == false)
        {
            GunTransform.LookAt(ControlCamera.position);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);

            animator.SetLookAtWeight(1.0f, 0.0f, 1.0f, 1.0f, 0f);
            animator.SetLookAtPosition(ControlCamera.position);
        }
        else
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
}
