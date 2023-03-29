using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{   

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

       
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Application.version;
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        PhotonNetwork.CreateRoom("NewName");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
}
