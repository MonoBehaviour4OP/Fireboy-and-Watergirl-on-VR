using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 5f;      // �̵� �ӵ�
    public float jumpForce = 10f;     // ���� ��

    private Rigidbody rb;             // Rigidbody ������Ʈ
    private bool isGrounded;          // ���� �ִ��� Ȯ��
    private string characterRole;     // �÷��̾� ���� (Fireboy �Ǵ� Watergirl)

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // P1�� P2�� ������ CustomProperties���� ��������
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
        if (!photonView.IsMine) return;  // �ٸ� �÷��̾�� ó������ ����

        // ���ҿ� ���� �Է� ó��
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
        float horizontal = Input.GetAxis("Horizontal"); // A, D Ű �Ǵ� �¿� ȭ��ǥ
        float vertical = Input.GetAxis("Vertical");     // W, S Ű �Ǵ� ��, �Ʒ� ȭ��ǥ

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // ���� ���� ���� ���� ����
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
