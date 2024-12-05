using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Mainmenu : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public Button multiplayerButton;

    void Start()
    {
        multiplayerButton.onClick.AddListener(OnMultiplayerButtonClicked);
    }

    void OnMultiplayerButtonClicked()
    {
        string nickname = nicknameInput.text;
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning("Nickname is empty!");
            return;
        }

        PhotonNetwork.NickName = nickname;
        PhotonNetwork.LoadLevel("MultilobbyScene"); // 로비 씬으로 전환
    }
}
