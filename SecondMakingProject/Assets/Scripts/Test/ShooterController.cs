using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;      // Cinemacine名前空間を使用

/// <summary>
/// [プログラム概要]
/// ・のぞき込みや、スプリント時にカメラの画角(FOV値)を変更
/// ・カメラからRayCastして、カメラが狙う位置を設定(IKControllerに渡して、銃の向きを変更するため)
/// </summary>
public class ShooterController : MonoBehaviour
{
    // カメラ制御にはCinemacineの名前空間内のVirtualCameraを使用
    CinemachineVirtualCamera virtualCamera;

    // 通常時のFOV値
    [Range(1f, 179f)] public float NomalFOV = 40;
    // のぞき込み時のFOV値
    [Range(1f, 179f)] [SerializeField] float AdsFOV = 25;

    // FOV値の変化のスピード
    [SerializeField] float ChengeSpeed = 10f;

    // プレイヤー
    [SerializeField] PlayerController playerController;

    // Sprint状態把握のため取得
    [SerializeField] Animator animator;

    // カメラが狙っている位置を格納
    public Vector3 TargettingPosition;

    // カメラから発するRayの長さ
    [SerializeField] float TargetDistance = 100.0f;
    RaycastHit raycastHit;      // Rayのヒット情報

    // Startメソッド -----------------------------------------------------------------------------
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();   // VirtualCameraコンポーネント取得
        virtualCamera.m_Lens.FieldOfView = NomalFOV;    // スタート時は通常のFOV値

        // カーソルの非表示とロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Updateメソッド ----------------------------------------------------------------------------
    void Update()
    {
        // Rayの原点は、カメラの位置から少し前方の位置からスタート
        Vector3 rayOrigin = transform.position + transform.forward * TargetDistance * 0.05f;

        // 設定した位置からRayを発射
        if (Physics.Raycast(rayOrigin,transform.forward,out raycastHit, TargetDistance))
        {
            // ヒットしていたら、当たった場所を狙っていると判断
            TargettingPosition = raycastHit.point;
        }
        else
        {
            // そうでない場合は、Rayの長さの先を狙っていると判断
            TargettingPosition = transform.position + transform.forward * TargetDistance;
        }

        if (Input.GetMouseButton(1))    // 右クリックでのぞき込み
        {
            ChengeFOV(AdsFOV);          // 画角を狭める
            playerController.IsAds = true;      // ADSしている状態に変更
        }
        else if(animator.GetBool("Sprint"))
        { 
            ChengeFOV(NomalFOV * 1.5f);     // スプリント時は通常時の1.5倍に広げる
        }
        else
        {
            playerController.IsAds = false;     // 上記以外は通常のFOV値で、ADS状態を解除
            ChengeFOV(NomalFOV);
        }
    }

    /// <summary>
    /// FOV値のスムーズな変更をするメソッド
    /// </summary>
    /// <param name="TargetFOV">変更したいFOV値</param>
    void ChengeFOV(float TargetFOV)
    {
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            TargetFOV,
            ChengeSpeed * Time.deltaTime
        );
    }
}
