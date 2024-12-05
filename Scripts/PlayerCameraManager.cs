using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerCameraManager : MonoBehaviourPunCallbacks
{
    public static PlayerCameraManager Instance { get; private set; }

    public GameObject player1CameraObject;  // P1의 카메라 오브젝트
    public GameObject player2CameraObject;  // P2의 카메라 오브젝트

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

        // 씬 로딩 후 호출되는 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // PhotonNetwork.LocalPlayer의 CustomProperties에 P1Character, P2Character 설정 여부 확인
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character") || PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P2Character"))
        {
            string role = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character")
                          ? (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"]
                          : (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];
            SetPlayerCamera(role);
        }
        else
        {
            Debug.LogError("LocalPlayer의 역할 정보가 설정되지 않았습니다.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene")  // MainGameScene이 로드될 때
        {
            string role = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("P1Character")
                          ? (string)PhotonNetwork.LocalPlayer.CustomProperties["P1Character"]
                          : (string)PhotonNetwork.LocalPlayer.CustomProperties["P2Character"];
            SetPlayerCamera(role);
        }
    }

    private void SetPlayerCamera(string role)
    {
        // 모든 카메라를 비활성화
        player1CameraObject.SetActive(false);
        player2CameraObject.SetActive(false);

        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length >= 2)
        {
            string player1Role = players[0].CustomProperties.ContainsKey("P1Character") ? (string)players[0].CustomProperties["P1Character"] : "";
            string player2Role = players[1].CustomProperties.ContainsKey("P2Character") ? (string)players[1].CustomProperties["P2Character"] : "";

            // P1과 P2의 역할에 따른 카메라 조정
            if (player1Role == "Fireboy" && player2Role == "Watergirl")
            {
                // 로컬 플레이어가 Fireboy일 경우 P1 카메라 활성화
                if (PhotonNetwork.LocalPlayer == players[0])  // 로컬 플레이어가 P1 (Fireboy)
                {
                    player1CameraObject.SetActive(true);  // Fireboy는 P1 카메라
                    Debug.Log("P1 (Fireboy) camera activated.");
                }

                // 로컬 플레이어가 Watergirl일 경우 P2 카메라 활성화
                if (PhotonNetwork.LocalPlayer == players[1])  // 로컬 플레이어가 P2 (Watergirl)
                {
                    player2CameraObject.SetActive(true);  // Watergirl은 P2 카메라
                    Debug.Log("P2 (Watergirl) camera activated.");
                }
            }
            else if (player1Role == "Watergirl" && player2Role == "Fireboy")
            {
                // 로컬 플레이어가 Watergirl일 경우 P2 카메라 활성화
                if (PhotonNetwork.LocalPlayer == players[0])  // 로컬 플레이어가 P1 (Watergirl)
                {
                    player2CameraObject.SetActive(true);  // Watergirl은 P2 카메라
                    Debug.Log("P1 (Watergirl) camera activated.");
                }

                // 로컬 플레이어가 Fireboy일 경우 P1 카메라 활성화
                if (PhotonNetwork.LocalPlayer == players[1])  // 로컬 플레이어가 P2 (Fireboy)
                {
                    player1CameraObject.SetActive(true);  // Fireboy는 P1 카메라
                    Debug.Log("P2 (Fireboy) camera activated.");
                }
            }
            else
            {
                // 예상하지 못한 역할 조합이 있을 경우 처리
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
        // 씬 로딩 이벤트를 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Photon에서 플레이어의 역할이 업데이트될 때마다 카메라를 갱신하도록 하는 콜백 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // P1Character 또는 P2Character가 변경되었을 때 카메라를 갱신
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

    // 역할이 업데이트되었을 때 실시간으로 카메라를 업데이트
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