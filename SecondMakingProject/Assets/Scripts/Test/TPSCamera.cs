using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float RotateSpeed = 10.0f;

    float yaw, pitch;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);

        yaw += Input.GetAxis("Mouse X") * RotateSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * RotateSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -80, 60);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
