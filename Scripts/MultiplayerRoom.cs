using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class MultiplayerRoom : MonoBehaviourPunCallbacks
{
    // 싱글톤 인스턴스
    public static MultiplayerRoom Instance { get; private set; }

    public TMP_Text player1NicknameText;
    public TMP_Text player2NicknameText;
    public TMP_Text player1PingText;
    public TMP_Text player2PingText;

    public Image player1CharacterImage;
    public Image player2CharacterImage;

    public Sprite fireboySprite;
    public Sprite watergirlSprite;

    public Button changeRoleButton;
    public Button startButton;
    public Button backButton;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character"))
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", "Fireboy" } });
        }

        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Character", "Watergirl" } });
        }

        UpdatePlayerNicknames();
        SetInitialCharacter();
        UpdateCharacterImages();

        InvokeRepeating(nameof(UpdatePing), 1f, 5f); // 핑 업데이트
        InvokeRepeating(nameof(UpdatePlayerNicknames), 1f, 2f); // 닉네임 업데이트

        changeRoleButton.interactable = false;
        changeRoleButton.onClick.AddListener(OnChangeRoleButtonClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        UpdateStartButtonInteractivity();
    }

    void Update()
    {
        if (PhotonNetwork.PlayerList.Length == 2 && !changeRoleButton.interactable)
        {
            changeRoleButton.interactable = true;
        }
        else if (PhotonNetwork.PlayerList.Length < 2 && changeRoleButton.interactable)
        {
            changeRoleButton.interactable = false;
        }
        UpdateStartButtonInteractivity();
    }

    private void UpdatePlayerNicknames()
    {
        Player[] players = PhotonNetwork.PlayerList;
        player1NicknameText.text = players.Length > 0 ? players[0].NickName : "Waiting for Player1...";
        player2NicknameText.text = players.Length > 1 ? players[1].NickName : "Waiting for Player2...";
    }

    private void UpdatePing()
    {
        if (PhotonNetwork.IsConnected)
        {
            Player[] players = PhotonNetwork.PlayerList;
            if (players.Length > 0) player1PingText.text = $"Ping: {PhotonNetwork.GetPing()}ms";
            if (players.Length > 1) player2PingText.text = $"Ping: {PhotonNetwork.GetPing()}ms";
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room created. Assigning Fireboy to P1.");

        // 방을 만든 플레이어(P1)에게 Fireboy 역할을 강제로 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", "Fireboy" } });

        // 역할 이미지 업데이트
        UpdateCharacterImages();
    }



    private void SetInitialCharacter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character"))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", "Fireboy" } });
            }
        }
        else
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Character", "Watergirl" } });
            }
        }
    }

    private void UpdateCharacterImages()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // P1 역할 확인 (Fireboy)
        if (players.Length > 0 && players[0].CustomProperties.ContainsKey("P1Character"))
        {
            string character1 = (string)players[0].CustomProperties["P1Character"];
            player1CharacterImage.sprite = character1 == "Fireboy" ? fireboySprite : watergirlSprite;
            player1CharacterImage.gameObject.SetActive(true);
        }
        else
        {
            player1CharacterImage.gameObject.SetActive(false);
        }

        // P2 역할 확인 (Watergirl)
        if (players.Length > 1 && players[1].CustomProperties.ContainsKey("P2Character"))
        {
            string character2 = (string)players[1].CustomProperties["P2Character"];
            player2CharacterImage.sprite = character2 == "Watergirl" ? watergirlSprite : fireboySprite;
            player2CharacterImage.gameObject.SetActive(true);
        }
        else
        {
            player2CharacterImage.gameObject.SetActive(false);
        }
    }

    private void UpdateStartButtonInteractivity()
    {
        // 두 플레이어가 모두 방에 있을 때만 Start 버튼 활성화
        if (PhotonNetwork.PlayerList.Length == 2 && !startButton.interactable)
        {
            startButton.interactable = true;
        }
        else if (PhotonNetwork.PlayerList.Length < 2 && startButton.interactable)
        {
            startButton.interactable = false;
        }
    }

    public void OnChangeRoleButtonClicked()
    {
        if (PhotonNetwork.PlayerList.Length != 2)
        {
            Debug.LogWarning("플레이어가 2명이 아닙니다.");
            return;
        }

        SwapRoles();
    }

    void SwapRoles()
    {
        Debug.Log("역할을 교환합니다.");

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            string currentCharacter = i == 0 ? (string)players[i].CustomProperties["P1Character"] : (string)players[i].CustomProperties["P2Character"];
            string newCharacter = (currentCharacter == "Fireboy") ? "Watergirl" : "Fireboy";

            // 역할을 교환
            if (i == 0)
                players[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", newCharacter } });
            else
                players[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Character", newCharacter } });
        }

        UpdateCharacterImages();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient && newPlayer != PhotonNetwork.LocalPlayer) // 방장만 역할을 설정
        {
            Debug.Log("P2 joined. Assigning role to P2.");

        // P1의 역할을 확인하고 P2의 역할을 정합니다.
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            Player p1 = PhotonNetwork.PlayerList[0]; // P1은 항상 첫 번째 플레이어로 설정됨
            Player p2 = PhotonNetwork.PlayerList[1]; // P2는 두 번째 플레이어로 설정됨

            if (p1.CustomProperties.ContainsKey("P1Character"))
            {
                string p1Character = (string)p1.CustomProperties["P1Character"];

                // P1의 캐릭터가 "Fireboy"면 P2는 "Watergirl", 반대로 P1이 "Watergirl"이면 P2는 "Fireboy"
                string p2Character = (p1Character == "Fireboy") ? "Watergirl" : "Fireboy";

                // P2에게 역할 설정
                newPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Character", p2Character } });
            }
        }

        // 역할 이미지 업데이트
        UpdateCharacterImages();
        }
    }





    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("P1Character") || changedProps.ContainsKey("P2Character"))
        {
            UpdateCharacterImages();
        }
    }

    private void OnStartButtonClicked()
    {
        // 두 플레이어가 모두 있을 때만 시작 가능
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            // 게임 시작 이벤트를 모든 플레이어에게 전송
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.RaiseEvent(
                    1, // 이벤트 코드 (1: ReadyScene 이동)
                    null, // 데이터는 필요 없음
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, // 모든 플레이어에게 전달
                    SendOptions.SendReliable // 안정적으로 전달
                );

                // ReadyScene으로 이동
                PhotonNetwork.LoadLevel("ReadyScene");
            }
        }
        else
        {
            Debug.LogWarning("플레이어가 2명이 아니므로 게임을 시작할 수 없습니다.");
        }
    }

    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient) // 방장이 방을 떠나는 경우
        {
            // 방을 비활성화
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // 방 종료 이벤트를 다른 플레이어에게 전달
            PhotonNetwork.RaiseEvent(
                2, // 이벤트 코드 2: 방 종료
                null, // 추가 데이터 없음
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, // 모든 플레이어에게 전달
                SendOptions.SendReliable // 안정적으로 전송
            );

            // 방장이 방을 나감
            PhotonNetwork.LeaveRoom();
            Debug.Log("Player1 left. Room will be destroyed.");
        }
        else
        {
            // 참여자가 방을 나감
            PhotonNetwork.LeaveRoom();
            Debug.Log("Player2 left. Leaving the room.");
        }

        // 역할 정보 초기화
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
    {
        { "P1Character", null },
        { "P2Character", null }
    });

        // 로비 씬으로 이동
        PhotonNetwork.LoadLevel("MultilobbyScene");
    }




    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (PhotonNetwork.IsMasterClient && otherPlayer != PhotonNetwork.LocalPlayer)
        {
            // Player2가 나간 경우 UI 업데이트
            player2CharacterImage.gameObject.SetActive(false);
            player2NicknameText.text = "Waiting for Player2...";
            Debug.Log("Player2 has left. Disabling Player2's sprite.");
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    private void OnEventReceived(EventData photonEvent)
    {
        if (photonEvent.Code == 1) // 이벤트 코드 1: ReadyScene으로 이동
        {
            Debug.Log("모든 플레이어가 준비 완료. ReadyScene으로 이동합니다.");

            // 모든 플레이어가 ReadyScene으로 이동
            PhotonNetwork.LoadLevel("ReadyScene");
        }
        else if (photonEvent.Code == 2) // 이벤트 코드 2: 방 종료
        {
            Debug.Log("방이 종료됩니다. 방을 나갑니다.");
            PhotonNetwork.LeaveRoom(); // 방 나가기

            // 역할 정보 초기화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "P1Character", null },
            { "P2Character", null }
        });

            // 로비 씬으로 이동
            PhotonNetwork.LoadLevel("MultilobbyScene");
        }
    }




    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        Debug.Log($"MasterClient switched to: {newMasterClient.NickName}");
        if (newMasterClient != PhotonNetwork.LocalPlayer)
        {
            // 새로운 방장에게 필요한 설정을 업데이트
            // (방을 나가는 로직과 관계없이 추가 작업이 필요할 경우 여기에 작성)
        }
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // 방을 나간 후 메인 메뉴로 이동
        PhotonNetwork.LoadLevel("MainmenuScene");
        Debug.Log("Left the room. Returning to main menu.");
    }



}