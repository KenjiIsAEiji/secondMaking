using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    Camera ControlCamera;
    [Range(1f, 179f)] public float NomalFOV = 40f;
    [Range(1f, 179f)] [SerializeField] float AdsFOV = 25f;
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
    }

    // Update is called once per frame
    void Update()
    {
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

    void ChengeFOV(float TargetFOV)
    {
        ControlCamera.fieldOfView = Mathf.Lerp(
            ControlCamera.fieldOfView,
            TargetFOV,
            ChengeSpeed * Time.deltaTime
        );
    }
}
