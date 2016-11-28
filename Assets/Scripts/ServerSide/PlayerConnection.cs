using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerConnection: MonoBehaviour {


	public bool _isAtStartup = true;
	public bool _gameStarted = true;
	public Text _lazy_game_text;

	public Button gameButton;
	public Toggle serverSwitch;



	NetworkClient _client;


	void Start(){
		gameButton.onClick.AddListener(stateButton);

	}

	// Update is called once per frame
	void Update () {



	}



	void SetupClient(){

		Debug.Log (_lazy_game_text.GetComponentInChildren<Text> ().text);
		string[] network_address= _lazy_game_text.GetComponentInChildren<Text>().text.Split(':');


		_client = new NetworkClient();
		_client.RegisterHandler(MsgType.Connect, OnConnected); 
		Debug.Log ("Ports: "+network_address[1]);
		_client.Connect(network_address[0],int.Parse(network_address[1]));

	}


	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log("Connected to server");
		_client.RegisterHandler(MsgType.AddPlayer, readySession);     
	}

	public void readySession(NetworkMessage p_msg){

		PlayerPacket newPacket = p_msg.ReadMessage<PlayerPacket>();

		//Register for game data
		_client.RegisterHandler(666, gameSession);     
		_client.Send (MsgType.Ready,newPacket);
		Debug.Log("Sent Ready Confirmation: "+newPacket._id);
	}

	public void gameSession(NetworkMessage p_msg){
		PlayerPacket newPacket = p_msg.ReadMessage<PlayerPacket>();


	}

	public void stateButton(){

		if (!serverSwitch.isOn) {
			SetupClient ();

		}
	}

}
