using UnityEngine;

public class SlimeObstacle : MonoBehaviour
{
    public float speed = 5.0f; // 슬라임이 다가오는 속도
    public float deadZone = -10.0f; // 슬라임이 삭제될 X 좌표 (화면 왼쪽 밖)

    void Update()
    {
        // 1. 왼쪽으로 계속 이동합니다.
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. 만약 슬라임의 X 좌표가 deadZone보다 더 왼쪽으로 넘어갔다면
        if (transform.position.x < deadZone)
        {
            // 이 슬라임 오브젝트를 게임에서 파괴(삭제)하여 메모리를 아낍니다.
            Destroy(gameObject); 
        }
    }
}