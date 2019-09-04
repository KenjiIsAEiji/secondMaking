using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsLookCamera : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    Animator animator;
    [SerializeField] Transform ControlCamera;

    private Vector3 DefaultSplainAngle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        DefaultSplainAngle = animator.GetBoneTransform(HumanBodyBones.UpperChest).localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK()
    {
        if (playerController.IsAds)
        {
            animator.SetLookAtWeight(1.0f, 0.8f, 1.0f, 0.0f, 0f);
            animator.SetLookAtPosition(ControlCamera.position);
        }
        else
        {
            if (animator.GetBool("Sprint") == false)
            {
                animator.SetLookAtWeight(1.0f, 0.0f, 1.0f, 0.0f, 0f);
                animator.SetLookAtPosition(ControlCamera.position);
            }
        }
    }
}
