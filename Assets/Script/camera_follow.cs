using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    // 인스펙터창에서 카메라를 처음에 고정시켜둘 X 좌표를 직접 지정할 수도 있습니다.
    public float fixedXPosition = 0f;

    void LateUpdate()
    {
        if (target != null)
        {
            //핵심 수정 부분
            // X축은 캐릭터의 target.position.x 대신, 내가 고정하고 싶은 fixedXPosition을 넣습니다.
            // Y축과 Z축만 캐릭터를 부드럽게 추적하도록 셋팅합니다.
            Vector3 targetPosition = new Vector3(fixedXPosition + offset.x, target.position.y + offset.y, target.position.z + offset.z);

            // 지정된 목표 위치로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}