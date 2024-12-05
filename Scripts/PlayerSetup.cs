using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    void Start()
    {
        if (!GetComponent<PhotonView>())
        {
            // PhotonView �������� �߰�
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ObservedComponents = new System.Collections.Generic.List<Component>();

            // PhotonTransformView �������� �߰� �� ����
            PhotonTransformView transformView = gameObject.AddComponent<PhotonTransformView>();
            photonView.ObservedComponents.Add(transformView);

            transformView.m_SynchronizePosition = true;
            transformView.m_SynchronizeRotation = true;
            transformView.m_SynchronizeScale = false; // �ʿ��ϸ� true�� ����
        }
    }
}
