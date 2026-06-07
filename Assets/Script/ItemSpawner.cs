using System.Collections.Generic;
using UnityEngine;


// 아이템 무작위 배치, 5-8개

public class ItemSpawner : MonoBehaviour
{
    [Header("아이템 프리펩")]
    public GameObject itemPrefab;

    [Header("아이템 스폰 후보)")]
    public List<Transform> allPlatformPoints = new List<Transform>();

    [Header("예외, NPC가 위치한 발판")]
    public Transform npcPlatformPoint;

    [Header("예외, 시작 발판")]
    public Transform playerStartPlatformPoint;

    void Start()
    {
        // 게임 시작 전 아이템 배치
        SpawnItemsRandomly();
    }

    // 중복 없이 아이템 생성
    void SpawnItemsRandomly()
    {
        // 데이터 무결성 검사
        if (allPlatformPoints.Count == 0 || itemPrefab == null) return;

        // NPC 발판에 아이템이 존재할 경우, 제거
        if (npcPlatformPoint != null && allPlatformPoints.Contains(npcPlatformPoint))
        {
            allPlatformPoints.Remove(npcPlatformPoint);
        }

        // 플레이어 시작 발판에 아이템이 존재할 경우, 제거
        if (playerStartPlatformPoint != null && allPlatformPoints.Contains(playerStartPlatformPoint))
        {
            allPlatformPoints.Remove(playerStartPlatformPoint);
        }

        // 조건에 부합하는 5개 이상 8개 이하의 정수형 난수 추출
        int spawnCount = Random.Range(5, 9);

        // 남은 최종 발판 수가 요구 스폰 수보다 적을 경우, 최대값 강제 동기화
        if (spawnCount > allPlatformPoints.Count)
            spawnCount = allPlatformPoints.Count;

        // 선착순 배정 시 한 공간당 최대 1개 스폰이 물리적으로 보장
        for (int i = 0; i < allPlatformPoints.Count; i++)
        {
            // 현재 인덱스부터 리스트 끝자리의 인덱스 사이에서 무작위 타겟 난수 결정
            int randomIndex = Random.Range(i, allPlatformPoints.Count);

            // 임시 변수를 활용한 두 객체의 메모리 위치 스왑 수행
            Transform temp = allPlatformPoints[i];
            allPlatformPoints[i] = allPlatformPoints[randomIndex];
            allPlatformPoints[randomIndex] = temp;
        }

        // point 중 앞에서부터 최종 결정된 개수만큼 순차 배정
        for (int i = 0; i < spawnCount; i++)
        {
            // 좌표 위치 결정
            Vector3 spawnPosition = allPlatformPoints[i].position;

            // 프리펩 공중 생성 금지
            spawnPosition.y += 0.7f;

            // 프리펨 생성
            // 각도 고정
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }

        Debug.Log($"{spawnCount}개 아이템 배치");
    }
}