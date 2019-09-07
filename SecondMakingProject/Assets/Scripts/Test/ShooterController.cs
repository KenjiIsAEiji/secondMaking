﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShooterController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    [Range(1f, 179f)] public float NomalFOV = 40;
    [Range(1f, 179f)] [SerializeField] float AdsFOV = 25;

    [SerializeField] float ChengeSpeed = 10f;

    [SerializeField] PlayerController playerController;

    [SerializeField] Animator animator;

    public Vector3 TargettingPosition;

    [SerializeField] float TargetDistance = 100.0f;
    RaycastHit raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.FieldOfView = NomalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = transform.position + transform.forward * TargetDistance * 0.05f;
        if (Physics.Raycast(rayOrigin,transform.forward,out raycastHit, TargetDistance))
        {
            TargettingPosition = raycastHit.point;
        }
        else
        {
            TargettingPosition = transform.position + transform.forward * TargetDistance;
        }

        if (Input.GetMouseButton(1))
        {
            ChengeFOV(AdsFOV);
            playerController.IsAds = true;
        }
        else if(animator.GetBool("Sprint"))
        { 
            ChengeFOV(NomalFOV * 1.5f);
        }
        else
        {
            playerController.IsAds = false;
            ChengeFOV(NomalFOV);
        }
    }

    void ChengeFOV(float TargetFOV)
    {
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            TargetFOV,
            ChengeSpeed * Time.deltaTime
        );
    }
}
