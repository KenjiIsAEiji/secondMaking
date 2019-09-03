using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField] Camera ControlCamera;
    [Range(1f, 179f)] public float NomalFOV = 40f;
    [Range(1f, 179f)] [SerializeField] float AdsFOV = 25f;
    [SerializeField] float ChengeSpeed = 10f;

    [SerializeField] PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
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

    void ChengeFOV(float fov)
    {
        ControlCamera.fieldOfView = Mathf.Lerp(
            ControlCamera.fieldOfView,
            fov,
            ChengeSpeed * Time.deltaTime
        );
    }

}
