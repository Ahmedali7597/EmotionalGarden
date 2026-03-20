using UnityEngine;
using UnityEngine.InputSystem; // 새로운 입력 시스템 사용!

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 5.0f; 
    private Rigidbody2D rb; 
    private bool isGrounded = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        
        // 게임이 시작될 때 시간이 정상적으로 흐르도록 설정 (재시작 시 멈춤 방지)
        Time.timeScale = 1f; 
    }

    void Update()
    {
        bool isJumpPressed = false;

        // 1. 마우스 왼쪽 버튼 클릭 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isJumpPressed = true;
        }
        
        // 2. 모바일 화면 터치 감지 (앱 게임용)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isJumpPressed = true;
        }

        // 버튼이 눌렸고, 바닥에 닿아있다면 점프!
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; 
        }
    }

    // 충돌 감지 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 바닥에 닿았을 때
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }
        // 2. 장애물(슬라임)에 닿았을 때
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameOver(); 
        }
    }

    // 게임 오버 처리 함수
    private void GameOver()
    {
        Debug.Log("Game Over!"); // 콘솔창에 메시지 띄우기

        // 게임의 시간을 0으로 만들어서 모든 것을 멈춥니다!
        Time.timeScale = 0f; 
    }
}