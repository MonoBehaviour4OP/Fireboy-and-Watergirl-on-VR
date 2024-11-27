using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReadySceneManager : MonoBehaviourPunCallbacks
{
    public static ReadySceneManager Instance { get; private set; }

    private static int readyPlayers = 0; // ReadyScene�� ������ �÷��̾� ��
    public float delayBeforeStart = 3f;  // �� ��ȯ �� ��� �ð� (3��)

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
        {
            readyPlayers = 0; // �ʱ�ȭ
        }

        SavePlayerInfo(); // �÷��̾� ���� ����

        // �غ� ���� ����
        photonView.RPC("PlayerReady", RpcTarget.AllBuffered);
    }

    void SavePlayerInfo()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // �г��� �⺻�� ����
        string nickname = localPlayer.NickName ?? "Unknown";

        // ���� ���� �⺻�� ����
        string role = localPlayer.CustomProperties.ContainsKey("P1Character")
                      ? localPlayer.CustomProperties["P1Character"].ToString()
                      : (localPlayer.CustomProperties.ContainsKey("P2Character")
                         ? localPlayer.CustomProperties["P2Character"].ToString()
                         : "Unknown");

        // �÷��̾� ���� ����
        localPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "nickname", nickname },
            { "role", role }
        });

        Debug.Log($"Player Info Saved - Nickname: {nickname}, Role: {role}");
    }

    [PunRPC]
    void PlayerReady()
    {
        readyPlayers++;

        Debug.Log($"Ready Players: {readyPlayers}/{PhotonNetwork.PlayerList.Length}");

        if (readyPlayers == PhotonNetwork.PlayerList.Length)
        {
            // ��� �÷��̾ �غ�� ��� �� ��ȯ
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("��� �÷��̾� �غ� �Ϸ�. 3�� �� MainGameScene���� �̵�.");
                StartCoroutine(LoadMainGameSceneCoroutine());
            }
        }
    }

    public void OnPlayerReady(string role)
    {
        // ������ CustomProperties�� ����
        if (role == "P1Character")
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "P1Character", role } });
        }
        else if (role == "P2Character")
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "P2Character", role } });
        }

        // �غ� �Ϸ� ��, ���� ������ ��ȯ�ϴ� �ڵ�
        StartCoroutine(LoadMainGameSceneCoroutine());  // LoadMainGameSceneCoroutine���� ����
    }

    IEnumerator LoadMainGameSceneCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        PhotonNetwork.LoadLevel("MainGameScene");
    }

    void LoadMainGameScene()
    {
        PhotonNetwork.LoadLevel("MainGameScene");
    }
}
