using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 5f;      // 이동 속도
    public float jumpForce = 10f;     // 점프 힘

    private Rigidbody rb;             // Rigidbody 컴포넌트
    private bool isGrounded;          // 땅에 있는지 확인
    private string characterRole;     // 플레이어 역할 (Fireboy 또는 Watergirl)

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // P1과 P2의 역할을 CustomProperties에서 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character"))
        {
            string p1Role = (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"];
            characterRole = (p1Role == "Fireboy") ? "Fireboy" : "Watergirl";
        }
        else if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
        {
            string p2Role = (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];
            characterRole = (p2Role == "Fireboy") ? "Fireboy" : "Watergirl";
        }

        Debug.Log("Player Role: " + characterRole);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;  // 다른 플레이어는 처리하지 않음

        // 역할에 따른 입력 처리
        if (characterRole == "Fireboy")
        {
            MovePlayer();
            Jump();
        }
        else if (characterRole == "Watergirl")
        {
            MovePlayer();
            Jump();
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A, D 키 또는 좌우 화살표
        float vertical = Input.GetAxis("Vertical");     // W, S 키 또는 위, 아래 화살표

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // 땅에 있을 때만 점프 가능
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
