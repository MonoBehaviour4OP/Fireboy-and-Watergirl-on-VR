using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Multirobby : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;  // 방 생성 시 입력 필드
    public TMP_InputField joinRoomNameInput;  // 방 조인 시 입력 필드
    public Button joinButton;  // Join 버튼
    public Button createRoomButton;  // 방 생성 버튼
    public GameObject createRoomPanel;  // 방 생성 패널
    public TMP_Text nicknameText;  // 닉네임 표시 텍스트
    public Button cancelButton;  // Cancel 버튼
    public Button confirmCreateButton; // Create 버튼
    public Button backButton; // Back(메뉴로 돌아가기) 버튼

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();  // Photon 서버 연결
        }

        createRoomPanel.SetActive(false);  // 방 생성 패널 비활성화
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);  // Cancel 버튼 리스너
        confirmCreateButton.onClick.AddListener(OnConfirmCreateRoom); // Create 버튼 리스너

        // 닉네임 표시
        nicknameText.text = "Nickname : " + PhotonNetwork.NickName;

        PhotonNetwork.JoinLobby(); // 로비에 입장하여 방 목록을 받기 시작

        // Join 버튼 클릭 리스너 등록
        joinButton.onClick.AddListener(OnJoinRoomButtonClicked);

        backButton.onClick.AddListener(OnBackButtonClicked);  // Cancel 버튼 리스너
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버에 연결됨");
    }

    // Join 버튼 클릭 시 실행될 메서드
    void OnJoinRoomButtonClicked()
    {
        string roomName = joinRoomNameInput.text;  // 방 이름 입력값 가져오기
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름을 입력하세요!");
            return;
        }

        // 입력된 방 이름으로 방에 입장 시도
        PhotonNetwork.JoinRoom(roomName);
    }

    void OnCreateRoomButtonClicked()
    {
        createRoomPanel.SetActive(true);  // 방 생성 패널 활성화
    }

    void OnCancelButtonClicked()
    {
        Debug.Log("Cancel 버튼 클릭됨!");
        createRoomPanel.SetActive(false);  // 방 생성 패널 비활성화
    }

    void OnConfirmCreateRoom()
    {
        string roomName = roomNameInput.text;  // 방 이름 입력값 가져오기
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름을 입력하세요!");
            return;
        }

        RoomOptions options = new RoomOptions { MaxPlayers = 2 }; // 방 최대 인원 설정
        Debug.Log($"방 이름: {roomName}, 최대 인원: {options.MaxPlayers}");

        // 방 생성 요청
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공!");
        PhotonNetwork.LoadLevel("MultiroomScene");  // MultiroomScene으로 이동
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공!");
        PhotonNetwork.LoadLevel("MultiroomScene");  // MultiroomScene으로 이동
    }

    void OnBackButtonClicked()
    {
        Debug.Log("뒤로가기 버튼 클릭!");
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("서버 연결이 해제되었습니다: " + cause);
        PhotonNetwork.LoadLevel("MainmenuScene"); // MainmenuScene으로 이동
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 입장 실패: {message}");
    }
}
