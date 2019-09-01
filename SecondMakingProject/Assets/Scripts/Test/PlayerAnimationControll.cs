using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControll : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("moving_x", characterController.velocity.x);
        animator.SetFloat("moving_z", characterController.velocity.z);
    }
}
