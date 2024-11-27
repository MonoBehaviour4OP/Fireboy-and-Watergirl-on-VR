using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Multirobby : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;  // �� ���� �� �Է� �ʵ�
    public TMP_InputField joinRoomNameInput;  // �� ���� �� �Է� �ʵ�
    public Button joinButton;  // Join ��ư
    public Button createRoomButton;  // �� ���� ��ư
    public GameObject createRoomPanel;  // �� ���� �г�
    public TMP_Text nicknameText;  // �г��� ǥ�� �ؽ�Ʈ
    public Button cancelButton;  // Cancel ��ư
    public Button confirmCreateButton; // Create ��ư
    public Button backButton; // Back(�޴��� ���ư���) ��ư

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();  // Photon ���� ����
        }

        createRoomPanel.SetActive(false);  // �� ���� �г� ��Ȱ��ȭ
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);  // Cancel ��ư ������
        confirmCreateButton.onClick.AddListener(OnConfirmCreateRoom); // Create ��ư ������

        // �г��� ǥ��
        nicknameText.text = "Nickname : " + PhotonNetwork.NickName;

        PhotonNetwork.JoinLobby(); // �κ� �����Ͽ� �� ����� �ޱ� ����

        // Join ��ư Ŭ�� ������ ���
        joinButton.onClick.AddListener(OnJoinRoomButtonClicked);

        backButton.onClick.AddListener(OnBackButtonClicked);  // Cancel ��ư ������
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("������ �����");
    }

    // Join ��ư Ŭ�� �� ����� �޼���
    void OnJoinRoomButtonClicked()
    {
        string roomName = joinRoomNameInput.text;  // �� �̸� �Է°� ��������
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("�� �̸��� �Է��ϼ���!");
            return;
        }

        // �Էµ� �� �̸����� �濡 ���� �õ�
        PhotonNetwork.JoinRoom(roomName);
    }

    void OnCreateRoomButtonClicked()
    {
        createRoomPanel.SetActive(true);  // �� ���� �г� Ȱ��ȭ
    }

    void OnCancelButtonClicked()
    {
        Debug.Log("Cancel ��ư Ŭ����!");
        createRoomPanel.SetActive(false);  // �� ���� �г� ��Ȱ��ȭ
    }

    void OnConfirmCreateRoom()
    {
        string roomName = roomNameInput.text;  // �� �̸� �Է°� ��������
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("�� �̸��� �Է��ϼ���!");
            return;
        }

        RoomOptions options = new RoomOptions { MaxPlayers = 2 }; // �� �ִ� �ο� ����
        Debug.Log($"�� �̸�: {roomName}, �ִ� �ο�: {options.MaxPlayers}");

        // �� ���� ��û
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����!");
        PhotonNetwork.LoadLevel("MultiroomScene");  // MultiroomScene���� �̵�
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"�� ���� ����: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� ����!");
        PhotonNetwork.LoadLevel("MultiroomScene");  // MultiroomScene���� �̵�
    }

    void OnBackButtonClicked()
    {
        Debug.Log("�ڷΰ��� ��ư Ŭ��!");
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("���� ������ �����Ǿ����ϴ�: " + cause);
        PhotonNetwork.LoadLevel("MainmenuScene"); // MainmenuScene���� �̵�
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"�� ���� ����: {message}");
    }
}
