using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    Camera ControlCamera;
    [Range(1f, 179f)] public int NomalFOV = 40;
    [Range(1f, 179f)] [SerializeField] int AdsFOV = 25;
    [Range(1f, 179f)] [SerializeField] int SprintFOV = 80;

    [SerializeField] float ChengeSpeed = 10f;

    [SerializeField] PlayerController playerController;

    [SerializeField] Animator animator;
    private Vector3 DefaultSplainAngle;

    // Start is called before the first frame update
    void Start()
    {
        ControlCamera = GetComponent<Camera>();
        DefaultSplainAngle = animator.GetBoneTransform(HumanBodyBones.UpperChest).localEulerAngles;
        ControlCamera.fieldOfView = NomalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Sprint"))
        {
            ChengeFOV(SprintFOV);
        }
        else
        {
            ChengeFOV(NomalFOV);
        }

        if (Input.GetMouseButton(1))
        {
            ChengeFOV(AdsFOV);
            playerController.IsAds = true;
        }
        else
        {
            ChengeFOV(NomalFOV);
            playerController.IsAds = false;
        }
    }

    void ChengeFOV(int TargetFOV)
    {
        ControlCamera.fieldOfView = Mathf.Lerp(
            ControlCamera.fieldOfView,
            TargetFOV,
            ChengeSpeed * Time.deltaTime
        );
    }
}
