using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTabManager : MonoBehaviour
{
    public GameObject tabCanvas;                // �÷��̾� ���� UI Canvas
    public TextMeshProUGUI player1InfoText;     // P1 ���� ����� TMP �ؽ�Ʈ
    public TextMeshProUGUI player2InfoText;     // P2 ���� ����� TMP �ؽ�Ʈ

    private void Start()
    {
        // Tab Ű�� ���� �� UI ǥ��
        tabCanvas.SetActive(false);  // ���� �� ĵ���� ��Ȱ��ȭ
    }

    private void Update()
    {
        // Tab Ű�� ������ �� UI ���
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePlayerInfoPanel();
        }
    }

    private void TogglePlayerInfoPanel()
    {
        // UI Ȱ��ȭ/��Ȱ��ȭ
        bool isActive = tabCanvas.activeSelf;
        tabCanvas.SetActive(!isActive);

        if (!isActive)
        {
            UpdatePlayerInfo();  // UI ����
        }
    }

    private void UpdatePlayerInfo()
    {
        // Photon ��Ʈ��ũ���� ��� �÷��̾� ������ �����ɴϴ�
        Player[] players = PhotonNetwork.PlayerList;

        // P1 ���� ������Ʈ
        if (players.Length > 0)
        {
            string player1Nickname = players[0].NickName;
            string player1Role = (players[0].CustomProperties.ContainsKey("P1Character"))
                ? (string)players[0].CustomProperties["P1Character"]
                : "Unknown Role";

            player1InfoText.text = $"P1: {player1Nickname}, {player1Role}";
        }

        // P2 ���� ������Ʈ
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
