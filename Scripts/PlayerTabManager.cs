using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTabManager : MonoBehaviour
{
    public GameObject tabCanvas;                // 플레이어 정보 UI Canvas
    public TextMeshProUGUI player1InfoText;     // P1 정보 출력할 TMP 텍스트
    public TextMeshProUGUI player2InfoText;     // P2 정보 출력할 TMP 텍스트

    private void Start()
    {
        // Tab 키를 누를 때 UI 표시
        tabCanvas.SetActive(false);  // 시작 시 캔버스 비활성화
    }

    private void Update()
    {
        // Tab 키를 눌렀을 때 UI 토글
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePlayerInfoPanel();
        }
    }

    private void TogglePlayerInfoPanel()
    {
        // UI 활성화/비활성화
        bool isActive = tabCanvas.activeSelf;
        tabCanvas.SetActive(!isActive);

        if (!isActive)
        {
            UpdatePlayerInfo();  // UI 갱신
        }
    }

    private void UpdatePlayerInfo()
    {
        // Photon 네트워크에서 모든 플레이어 정보를 가져옵니다
        Player[] players = PhotonNetwork.PlayerList;

        // P1 정보 업데이트
        if (players.Length > 0)
        {
            string player1Nickname = players[0].NickName;
            string player1Role = (players[0].CustomProperties.ContainsKey("P1Character"))
                ? (string)players[0].CustomProperties["P1Character"]
                : "Unknown Role";

            player1InfoText.text = $"P1: {player1Nickname}, {player1Role}";
        }

        // P2 정보 업데이트
        if (players.Length > 1)
        {
            string player2Nickname = players[1].NickName;
            string player2Role = (players[1].CustomProperties.ContainsKey("P2Character"))
                ? (string)players[1].CustomProperties["P2Character"]
                : "Unknown Role";

            player2InfoText.text = $"P2: {player2Nickname}, {player2Role}";
        }
    }
}
