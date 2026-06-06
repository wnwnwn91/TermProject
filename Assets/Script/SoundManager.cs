using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 어디서나 SoundManager.instance로 접근 가능한 전역 스위치
    public static SoundManager instance;

    private AudioSource myAudioSource; // 소리를 출력하는 기본 스피커 컴포넌트

    [Header("Item Audio Clips")]
    public AudioClip umbrellaSound;   // 1라운드: 우산 획득음
    public AudioClip iceSound;        // 2라운드: 얼음 획득음
    public AudioClip hotPackSound;    // 3라운드: 핫팩 획득음

    [Header("UI Audio Clips")]
    public AudioClip buttonSound;     // 메뉴 화면: 버튼 클릭음

    [Header("NPC Audio Clips")]
    public AudioClip rescueSound;     // NPC 구출 성공음

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 다음 라운드로 넘어가도 파괴되지 않고 음악 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성된 사운드 매니저 발견 시 즉시 제거
            return;
        }

        // 오디오 소스 컴포넌트 자동 탐색 및 안전 장치
        myAudioSource = GetComponent<AudioSource>();
        if (myAudioSource == null)
        {
            myAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 오디오 재생 알고리즘: PlayOneShot
    public void PlayRescue() { if (rescueSound != null) myAudioSource.PlayOneShot(rescueSound); } // NPC 구출 소리
    public void PlayButton() { if (buttonSound != null) myAudioSource.PlayOneShot(buttonSound); } // 버튼 클릭 소리
    public void PlayUmbrella() { if (umbrellaSound != null) myAudioSource.PlayOneShot(umbrellaSound); } // 우산 아이템 소리
    public void PlayIce() { if (iceSound != null) myAudioSource.PlayOneShot(iceSound); } // 얼음 아이템 소리
    public void PlayHotPack() { if (hotPackSound != null) myAudioSource.PlayOneShot(hotPackSound); } // 핫팩 아이템 소리
}