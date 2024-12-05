using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    void Start()
    {
        if (!GetComponent<PhotonView>())
        {
            // PhotonView 동적으로 추가
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ObservedComponents = new System.Collections.Generic.List<Component>();

            // PhotonTransformView 동적으로 추가 및 설정
            PhotonTransformView transformView = gameObject.AddComponent<PhotonTransformView>();
            photonView.ObservedComponents.Add(transformView);

            transformView.m_SynchronizePosition = true;
            transformView.m_SynchronizeRotation = true;
            transformView.m_SynchronizeScale = false; // 필요하면 true로 설정
        }
    }
}
