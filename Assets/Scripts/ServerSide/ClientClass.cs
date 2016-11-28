using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientClass : MonoBehaviour {

    double x;
    double y;
    double z;

    PlayerType currentType;

    NetworkConnection clientConnection;

    enum PlayerType
    {
        Human, Zombie
    };


    void Start() {
        Input.location.Start();
    }

	// Update is called once per frame
	void Update () {

    }

    void OnStartClient()
    {
        Debug.Log("The client is started.");
    }

    void OnClientConnect()
    {
        Debug.Log("The client has connected to a server.");
    }

    void OnClientSceneChanged()
    {
        Debug.Log("The network manager has changed scenes.");
    }
}
