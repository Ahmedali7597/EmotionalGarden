using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 찍어낼 슬라임의 원본(붕어빵 틀)을 넣을 빈칸입니다.
    public GameObject slimePrefab; 
    
    // 몇 초마다 슬라임을 만들어낼지 결정합니다.
    public float spawnRate = 2.0f; 
    
    // 시간을 잴 타이머입니다.
    private float timer = 0f;

    void Update()
    {
        // 1. 매 프레임마다 타이머에 시간을 더해줍니다.
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        // 2. 타이머가 설정한 시간(spawnRate)을 넘어서면!
        else
        {
            SpawnSlime(); // 슬라임 생성 함수 실행
            timer = 0f;   // 타이머를 다시 0으로 초기화해서 잴 준비를 합니다.
        }
    }

    // 슬라임을 실제로 화면에 만들어내는 함수입니다.
    void SpawnSlime()
    {
        // Instantiate(만들 원본, 만들어질 위치, 회전값)
        // transform.position은 이 스포너가 있는 현재 위치를 뜻합니다.
        // Quaternion.identity는 "회전하지 않고 원본 그대로"라는 뜻입니다.
        Instantiate(slimePrefab, transform.position, Quaternion.identity);
    }
}