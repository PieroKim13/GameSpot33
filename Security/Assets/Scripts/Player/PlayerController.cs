using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputAction InputAction;
    public PlayerInputAction playerInputAction => InputAction;
    CharacterController controller;
    public CharacterController Controller => controller;
    CinemachineVirtualCamera cinemachine;
    public CinemachineVirtualCamera Cinemachine => cinemachine;

    /// <summary>
    /// 카메라 위치
    /// </summary>
    public Transform cameraRoot;

    /// <summary> 
    /// 위치변수
    /// </summary>
    Vector3 moveDir = Vector3.zero;

    /// <summary>
    /// 현재 이동속도
    /// </summary>
    float currentSpeed = 0.0f;

    /// <summary>
    /// 걷기 속도
    /// </summary>
    float walkingSpeed = 3.0f;

    /// <summary>
    /// 달리기 속도
    /// </summary>
    float sprintingSpeed = 4.7f;

    /// <summary>
    /// 달리기 체크
    /// </summary>
    bool sprintChecking = false;

    /// <summary>
    /// 점프 높이
    /// </summary>
    float jumpHeight = 4.0f;

    /// <summary>
    /// 점프 체크
    /// </summary>
    bool jumpChecking = false;

    /// <summary>
    /// 점프 높이 확인
    /// </summary>
    float jumpCheckHeight = 0.0f;

    /// <summary>
    /// 중력 크기
    /// </summary>
    float gravity = 9.8f;

    /// <summary>
    /// 점프 카운트(1번으로 제한)
    /// </summary>
    int jumpCount = 0;

    /// <summary>
    /// 웅크리기 감소량
    /// </summary>
    float crouchDecrease = 1.0f;

    /// <summary>
    /// 웅크리기 체크
    /// </summary>
    bool crouchChecking = false;

    /// <summary>
    /// x방향 전환 민감도
    /// </summary>
    float rotateSensitiveX = 7.5f;

    /// <summary>
    /// y방향 전환 민감도
    /// </summary>
    float rotateSensitiveY = 10.0f;

    /// <summary>
    /// 현재 이동한 y방향 전환
    /// </summary>
    float curRotateY = 0.0f;

    float Temp = 0.0f;
    Vector3 boxsize = new Vector3(0.25f, 0.125f, 0.25f);
    Vector3 groundCheckPostion;

    public Action onSprinting;
    public Action offSprinting;

    public bool isStamina = false;

    private void Start()
    {
        currentSpeed = walkingSpeed;
        Temp = currentSpeed;
    }

    private void Awake()
    {
        InputAction = new PlayerInputAction();

        controller = GetComponent<CharacterController>();
        cameraRoot = transform.GetChild(0);
        cinemachine = GetComponentInChildren<CinemachineVirtualCamera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputAction.Player.Enable();
        InputAction.Player.Move.performed += OnMove;
        InputAction.Player.Move.canceled += OnMove;
        InputAction.Player.Sprint.performed += OnSprint;
        InputAction.Player.Sprint.canceled += OnSprint;
        InputAction.Player.Jump.performed += OnJump;
        InputAction.Player.Crouch.performed += OnCrouch;
        InputAction.Player.Crouch.canceled += OnCrouch;
        InputAction.Mouse.Enable();
        InputAction.Mouse.MouseVector2.performed += OnMouseDelta;
    }

    private void OnDisable()
    {
        InputAction.Mouse.MouseVector2.performed -= OnMouseDelta;
        InputAction.Mouse.Disable();
        InputAction.Player.Crouch.canceled -= OnCrouch;
        InputAction.Player.Crouch.performed -= OnCrouch;
        InputAction.Player.Jump.performed -= OnJump;
        InputAction.Player.Sprint.canceled -= OnSprint;
        InputAction.Player.Sprint.performed -= OnSprint;
        InputAction.Player.Move.canceled -= OnMove;
        InputAction.Player.Move.performed -= OnMove;
        InputAction.Player.Disable();
    }

    private void Update()
    {
        if (!IsGrounded())
        {
            moveDir.y -= gravity * Time.deltaTime;
        }
        // 플레이어 x, z좌표 이동
        controller.Move(Time.deltaTime * currentSpeed * transform.TransformDirection(new Vector3(moveDir.x, 0.0f, moveDir.z)));
        // 플레이어 y좌표 이동
        controller.Move(Time.deltaTime * new Vector3(0.0f, moveDir.y, 0.0f));
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        // 입력받은 W, A, S, D(Vector2)좌표를 x, z좌표로 지정
        moveDir.x = dir.x; moveDir.z = dir.y;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        // 웅크리기 상태가 아닐 때
        if (!crouchChecking)
        {
            if (context.performed)
            {
                // 현재 이동속도 = 달리기 속도
                currentSpeed = sprintingSpeed * crouchDecrease;
                onSprinting?.Invoke();
                isStamina = false;
                sprintChecking = true;
            }
            else
            {
                // 끝내기 코루틴 실행
                OffSprinting();
            }
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // 웅크리기 상태가 아닐 때
        if (!crouchChecking && IsGrounded())
        {
            if (jumpCount < 1)
            {
                // y이동 값을 점프 높이로 할당
                moveDir.y = jumpHeight;

                if (jumpCount == 0)
                {
                    // 목표 지점의 점프 높이
                    jumpCheckHeight = transform.position.y + controller.radius * 0.3f;
                }
                // 점프 상태 = true
                jumpChecking = true;
                jumpCount++;
            }
        }
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        if (!sprintChecking && IsGrounded())
        {
            if (context.performed)
            {
                // 감소량 0.5배
                crouchDecrease = 0.5f;
                // 현재 속도 = 임시변수(현재 속도) * 감소량(0.5)
                currentSpeed = Temp * crouchDecrease;
                crouchChecking = true;

                // 시점 낮추기 #웅크리기 O
                cameraRoot.transform.position += new Vector3(0f, -0.5f, 0f);
            }
            else
            {
                // 감소량 X
                crouchDecrease = 1.0f;
                // 현재 속도 = 걷기 속도(현재 속도) * 감소량X
                currentSpeed = Temp * crouchDecrease;
                crouchChecking = false;

                // 시점 올리기 #웅크리기 X
                cameraRoot.transform.position += new Vector3(0f, 0.5f, 0f);
            }
        }
    }

    private void OnMouseDelta(InputAction.CallbackContext context)
    {
        // 입력받은 마우스 좌표를 저장
        Vector2 temp = context.ReadValue<Vector2>();
        // 입력받은 좌표를 x방향 전환 민감도 만큼 천천히 이동
        float rotateX = temp.x * rotateSensitiveX * Time.fixedDeltaTime;
        // transform에 적용
        transform.Rotate(Vector3.up, rotateX);

        // 입력받은 좌표를 y방향 전환 민감도 만큼 천천히 이동
        float rotateY = temp.y * rotateSensitiveY * Time.fixedDeltaTime;
        // 계산된 y방향 전환 이동량을 현재 이동한 y방향 전환에 저장
        curRotateY -= rotateY;
        // y방향 전환의 최소 및 최대량을 지정
        curRotateY = Mathf.Clamp(curRotateY, -60.0f, 60.0f);
        // 이동한 방향만큼 카메라를 이동
        cameraRoot.rotation = Quaternion.Euler(curRotateY, cameraRoot.eulerAngles.y, cameraRoot.eulerAngles.z);
    }

    private bool IsGrounded()
    {
        // 점프 상태가 아니고, 현재 y높이가 목표 y보다 높을 때
        if (jumpChecking && transform.position.y > jumpCheckHeight)
        {
            jumpChecking = false;
        }

        // 캐릭터 밑으로 바닥을 체크하는 직사각형을 생성
        groundCheckPostion = new Vector3(transform.position.x, transform.position.y + controller.radius * -3.0f, transform.position.z);

        // 직사각형이 레이어 "Ground"에 닿을 경우
        if (Physics.CheckBox(groundCheckPostion, boxsize, Quaternion.identity, LayerMask.GetMask("Ground")))
        {
            if (!jumpChecking)
            {
                //현재 높이가 점프 최대 높이보다 작을 때
                if (moveDir.y < jumpHeight)
                {
                    //현재 높이를 -0.01f 줄임
                    moveDir.y = -0.01f;
                }
                jumpChecking = false;
                jumpCount = 0;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 달리기 종료 코루틴
    /// </summary>
    public void OffSprinting()
    {
        // 현재 속도 = 걷기 속도
        currentSpeed = walkingSpeed * crouchDecrease;
        offSprinting?.Invoke();
        sprintChecking = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawCube(groundCheckPostion, boxsize);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(groundCheckPostion, boxsize);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(cameraRoot.transform.position, 0.25f);
    }

#endif


}
