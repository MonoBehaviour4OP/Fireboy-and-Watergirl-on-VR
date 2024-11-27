using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReadySceneManager : MonoBehaviourPunCallbacks
{
    public static ReadySceneManager Instance { get; private set; }

    private static int readyPlayers = 0; // ReadyScene에 도달한 플레이어 수
    public float delayBeforeStart = 3f;  // 씬 전환 전 대기 시간 (3초)

    private void Awake()
    {
        // 싱글톤 패턴 설정
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
            readyPlayers = 0; // 초기화
        }

        SavePlayerInfo(); // 플레이어 정보 저장

        // 준비 상태 전송
        photonView.RPC("PlayerReady", RpcTarget.AllBuffered);
    }

    void SavePlayerInfo()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // 닉네임 기본값 설정
        string nickname = localPlayer.NickName ?? "Unknown";

        // 역할 정보 기본값 설정
        string role = localPlayer.CustomProperties.ContainsKey("P1Character")
                      ? localPlayer.CustomProperties["P1Character"].ToString()
                      : (localPlayer.CustomProperties.ContainsKey("P2Character")
                         ? localPlayer.CustomProperties["P2Character"].ToString()
                         : "Unknown");

        // 플레이어 정보 저장
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
            // 모든 플레이어가 준비된 경우 씬 전환
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("모든 플레이어 준비 완료. 3초 후 MainGameScene으로 이동.");
                StartCoroutine(LoadMainGameSceneCoroutine());
            }
        }
    }

    public void OnPlayerReady(string role)
    {
        // 역할을 CustomProperties에 설정
        if (role == "P1Character")
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "P1Character", role } });
        }
        else if (role == "P2Character")
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "P2Character", role } });
        }

        // 준비 완료 후, 게임 씬으로 전환하는 코드
        StartCoroutine(LoadMainGameSceneCoroutine());  // LoadMainGameSceneCoroutine으로 수정
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
