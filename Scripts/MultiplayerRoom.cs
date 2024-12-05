using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class MultiplayerRoom : MonoBehaviourPunCallbacks
{
    // �̱��� �ν��Ͻ�
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
        // �̱��� ����
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

        InvokeRepeating(nameof(UpdatePing), 1f, 5f); // �� ������Ʈ
        InvokeRepeating(nameof(UpdatePlayerNicknames), 1f, 2f); // �г��� ������Ʈ

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

        // ���� ���� �÷��̾�(P1)���� Fireboy ������ ������ ����
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", "Fireboy" } });

        // ���� �̹��� ������Ʈ
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

        // P1 ���� Ȯ�� (Fireboy)
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

        // P2 ���� Ȯ�� (Watergirl)
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
        // �� �÷��̾ ��� �濡 ���� ���� Start ��ư Ȱ��ȭ
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
            Debug.LogWarning("�÷��̾ 2���� �ƴմϴ�.");
            return;
        }

        SwapRoles();
    }

    void SwapRoles()
    {
        Debug.Log("������ ��ȯ�մϴ�.");

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            string currentCharacter = i == 0 ? (string)players[i].CustomProperties["P1Character"] : (string)players[i].CustomProperties["P2Character"];
            string newCharacter = (currentCharacter == "Fireboy") ? "Watergirl" : "Fireboy";

            // ������ ��ȯ
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

        if (PhotonNetwork.IsMasterClient && newPlayer != PhotonNetwork.LocalPlayer) // ���常 ������ ����
        {
            Debug.Log("P2 joined. Assigning role to P2.");

        // P1�� ������ Ȯ���ϰ� P2�� ������ ���մϴ�.
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            Player p1 = PhotonNetwork.PlayerList[0]; // P1�� �׻� ù ��° �÷��̾�� ������
            Player p2 = PhotonNetwork.PlayerList[1]; // P2�� �� ��° �÷��̾�� ������

            if (p1.CustomProperties.ContainsKey("P1Character"))
            {
                string p1Character = (string)p1.CustomProperties["P1Character"];

                // P1�� ĳ���Ͱ� "Fireboy"�� P2�� "Watergirl", �ݴ�� P1�� "Watergirl"�̸� P2�� "Fireboy"
                string p2Character = (p1Character == "Fireboy") ? "Watergirl" : "Fireboy";

                // P2���� ���� ����
                newPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Character", p2Character } });
            }
        }

        // ���� �̹��� ������Ʈ
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
        // �� �÷��̾ ��� ���� ���� ���� ����
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            // ���� ���� �̺�Ʈ�� ��� �÷��̾�� ����
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.RaiseEvent(
                    1, // �̺�Ʈ �ڵ� (1: ReadyScene �̵�)
                    null, // �����ʹ� �ʿ� ����
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, // ��� �÷��̾�� ����
                    SendOptions.SendReliable // ���������� ����
                );

                // ReadyScene���� �̵�
                PhotonNetwork.LoadLevel("ReadyScene");
            }
        }
        else
        {
            Debug.LogWarning("�÷��̾ 2���� �ƴϹǷ� ������ ������ �� �����ϴ�.");
        }
    }

    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient) // ������ ���� ������ ���
        {
            // ���� ��Ȱ��ȭ
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // �� ���� �̺�Ʈ�� �ٸ� �÷��̾�� ����
            PhotonNetwork.RaiseEvent(
                2, // �̺�Ʈ �ڵ� 2: �� ����
                null, // �߰� ������ ����
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, // ��� �÷��̾�� ����
                SendOptions.SendReliable // ���������� ����
            );

            // ������ ���� ����
            PhotonNetwork.LeaveRoom();
            Debug.Log("Player1 left. Room will be destroyed.");
        }
        else
        {
            // �����ڰ� ���� ����
            PhotonNetwork.LeaveRoom();
            Debug.Log("Player2 left. Leaving the room.");
        }

        // ���� ���� �ʱ�ȭ
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
    {
        { "P1Character", null },
        { "P2Character", null }
    });

        // �κ� ������ �̵�
        PhotonNetwork.LoadLevel("MultilobbyScene");
    }




    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (PhotonNetwork.IsMasterClient && otherPlayer != PhotonNetwork.LocalPlayer)
        {
            // Player2�� ���� ��� UI ������Ʈ
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
        if (photonEvent.Code == 1) // �̺�Ʈ �ڵ� 1: ReadyScene���� �̵�
        {
            Debug.Log("��� �÷��̾ �غ� �Ϸ�. ReadyScene���� �̵��մϴ�.");

            // ��� �÷��̾ ReadyScene���� �̵�
            PhotonNetwork.LoadLevel("ReadyScene");
        }
        else if (photonEvent.Code == 2) // �̺�Ʈ �ڵ� 2: �� ����
        {
            Debug.Log("���� ����˴ϴ�. ���� �����ϴ�.");
            PhotonNetwork.LeaveRoom(); // �� ������

            // ���� ���� �ʱ�ȭ
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "P1Character", null },
            { "P2Character", null }
        });

            // �κ� ������ �̵�
            PhotonNetwork.LoadLevel("MultilobbyScene");
        }
    }




    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        Debug.Log($"MasterClient switched to: {newMasterClient.NickName}");
        if (newMasterClient != PhotonNetwork.LocalPlayer)
        {
            // ���ο� ���忡�� �ʿ��� ������ ������Ʈ
            // (���� ������ ������ ������� �߰� �۾��� �ʿ��� ��� ���⿡ �ۼ�)
        }
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // ���� ���� �� ���� �޴��� �̵�
        PhotonNetwork.LoadLevel("MainmenuScene");
        Debug.Log("Left the room. Returning to main menu.");
    }



}