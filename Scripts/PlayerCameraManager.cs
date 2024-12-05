using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerCameraManager : MonoBehaviourPunCallbacks
{
    public static PlayerCameraManager Instance { get; private set; }

    public GameObject player1CameraObject;  // P1�� ī�޶� ������Ʈ
    public GameObject player2CameraObject;  // P2�� ī�޶� ������Ʈ

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // �� �ε� �� ȣ��Ǵ� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // PhotonNetwork.LocalPlayer�� CustomProperties�� P1Character, P2Character ���� ���� Ȯ��
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character") || PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
        {
            string role = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character")
                          ? (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"]
                          : (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];
            SetPlayerCamera(role);
        }
        else
        {
            Debug.LogError("LocalPlayer�� ���� ������ �������� �ʾҽ��ϴ�.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene")  // MainGameScene�� �ε�� ��
        {
            string role = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character")
                          ? (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"]
                          : (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];
            SetPlayerCamera(role);
        }
    }

    private void SetPlayerCamera(string role)
    {
        // ��� ī�޶� ��Ȱ��ȭ
        player1CameraObject.SetActive(false);
        player2CameraObject.SetActive(false);

        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length >= 2)
        {
            string player1Role = players[0].CustomProperties.ContainsKey("P1Character") ? (string)players[0].CustomProperties["P1Character"] : "";
            string player2Role = players[1].CustomProperties.ContainsKey("P2Character") ? (string)players[1].CustomProperties["P2Character"] : "";

            // P1�� P2�� ���ҿ� ���� ī�޶� ����
            if (player1Role == "Fireboy" && player2Role == "Watergirl")
            {
                // ���� �÷��̾ Fireboy�� ��� P1 ī�޶� Ȱ��ȭ
                if (PhotonNetwork.LocalPlayer == players[0])  // ���� �÷��̾ P1 (Fireboy)
                {
                    player1CameraObject.SetActive(true);  // Fireboy�� P1 ī�޶�
                    Debug.Log("P1 (Fireboy) camera activated.");
                }

                // ���� �÷��̾ Watergirl�� ��� P2 ī�޶� Ȱ��ȭ
                if (PhotonNetwork.LocalPlayer == players[1])  // ���� �÷��̾ P2 (Watergirl)
                {
                    player2CameraObject.SetActive(true);  // Watergirl�� P2 ī�޶�
                    Debug.Log("P2 (Watergirl) camera activated.");
                }
            }
            else if (player1Role == "Watergirl" && player2Role == "Fireboy")
            {
                // ���� �÷��̾ Watergirl�� ��� P2 ī�޶� Ȱ��ȭ
                if (PhotonNetwork.LocalPlayer == players[0])  // ���� �÷��̾ P1 (Watergirl)
                {
                    player2CameraObject.SetActive(true);  // Watergirl�� P2 ī�޶�
                    Debug.Log("P1 (Watergirl) camera activated.");
                }

                // ���� �÷��̾ Fireboy�� ��� P1 ī�޶� Ȱ��ȭ
                if (PhotonNetwork.LocalPlayer == players[1])  // ���� �÷��̾ P2 (Fireboy)
                {
                    player1CameraObject.SetActive(true);  // Fireboy�� P1 ī�޶�
                    Debug.Log("P2 (Fireboy) camera activated.");
                }
            }
            else
            {
                // �������� ���� ���� ������ ���� ��� ó��
                Debug.LogError("Unexpected role combination for players.");
            }
        }
        else
        {
            Debug.LogWarning("Not enough players in the room to assign cameras.");
        }
    }



    void OnDestroy()
    {
        // �� �ε� �̺�Ʈ�� ��� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Photon���� �÷��̾��� ������ ������Ʈ�� ������ ī�޶� �����ϵ��� �ϴ� �ݹ� �Լ�
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // P1Character �Ǵ� P2Character�� ����Ǿ��� �� ī�޶� ����
        if (changedProps.ContainsKey("P1Character") || changedProps.ContainsKey("P2Character"))
        {
            string role = "";
            if (changedProps.ContainsKey("P1Character"))
                role = (string)targetPlayer.CustomProperties["P1Character"];
            else if (changedProps.ContainsKey("P2Character"))
                role = (string)targetPlayer.CustomProperties["P2Character"];

            SetPlayerCamera(role);
        }
    }

    // ������ ������Ʈ�Ǿ��� �� �ǽð����� ī�޶� ������Ʈ
    private void SetPlayerCameraForUpdatedRole()
    {
        string role = "";
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character"))
            role = (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"];
        else if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
            role = (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];

        SetPlayerCamera(role);
    }
}