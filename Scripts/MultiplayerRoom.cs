using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

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
        UpdatePlayerNicknames();
        InvokeRepeating(nameof(UpdatePing), 1f, 5f); // �� ������Ʈ
        InvokeRepeating(nameof(UpdatePlayerNicknames), 1f, 2f); // �г��� ������Ʈ

        SetInitialCharacter();
        UpdateCharacterImages();

        changeRoleButton.interactable = false;

        changeRoleButton.onClick.AddListener(OnChangeRoleButtonClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
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

    private void SetInitialCharacter()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length > 0 && players[0] == PhotonNetwork.LocalPlayer)
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character"))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Character", "Fireboy" } });
            }
        }

        if (players.Length > 1 && players[1] == PhotonNetwork.LocalPlayer)
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

        if (players.Length > 1 && players[1].CustomProperties.ContainsKey("P2Character"))
        {
            string character2 = (string)players[1].CustomProperties["P2Character"];
            player2CharacterImage.sprite = character2 == "Fireboy" ? fireboySprite : watergirlSprite;
            player2CharacterImage.gameObject.SetActive(true);
        }
        else
        {
            player2CharacterImage.gameObject.SetActive(false);
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
        // Start ��ư�� ���� �÷��̾ �̺�Ʈ�� Ʈ����
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.RaiseEvent(
                1, // �̺�Ʈ �ڵ�
                null, // �����ʹ� �ʿ� ����
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, // ��� �÷��̾�� ����
                SendOptions.SendReliable // ���������� ����
            );
            Debug.Log("Start button clicked. Triggering ReadyScene transition for all players.");
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
        if (photonEvent.Code == 1) // �̺�Ʈ �ڵ尡 1�� ��� ó��
        {
            PhotonNetwork.LoadLevel("ReadyScene");
            Debug.Log("Moving all players to ReadyScene");
        }
    }
}
