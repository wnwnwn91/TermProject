using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Speed Settings")]
    public Transform target;          // 추적할 플레이어 
    public float followSpeed = 5f;    // 카메라 부드러움 속도 
    public Vector3 offset = new Vector3(0f, 0f, -10f); // 기본 거리 간격

    [Header("Camera Constraints")]
    public float fixedXPosition = 0f; // 좌우 고정할 X 좌표 값

    void LateUpdate()
    {
        if (target != null)
        {
            // X축은 적어둔 숫자로 칼고정, Y와 Z축만 플레이어의 실시간 위치 따라가기
            Vector3 targetPosition = new Vector3(
                fixedXPosition + offset.x,      // X축 고정 알고리즘
                target.position.y + offset.y,   // Y축 실시간 추적
                target.position.z + offset.z    // Z축 깊이 유지
            );

            // 선형보간
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}