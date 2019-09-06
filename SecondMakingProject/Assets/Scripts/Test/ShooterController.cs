using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    Camera ControlCamera;
    [Range(1f, 179f)] public float NomalFOV = 40;
    [Range(1f, 179f)] [SerializeField] float AdsFOV = 25;

    [SerializeField] float ChengeSpeed = 10f;

    [SerializeField] PlayerController playerController;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        ControlCamera = GetComponent<Camera>();
        ControlCamera.fieldOfView = NomalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
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
        ControlCamera.fieldOfView = Mathf.Lerp(
            ControlCamera.fieldOfView,
            TargetFOV,
            ChengeSpeed * Time.deltaTime
        );
    }
}
