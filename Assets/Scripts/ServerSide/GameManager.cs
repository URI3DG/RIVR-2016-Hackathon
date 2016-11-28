using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	/// <summary>
	/// The state.
	/// 0: prestart
	/// 1: listenting for connections
	/// 2: sent out ready
	/// 3: game running
	/// </summary>
	public int _state = 0;
	private int _lastFrame = 0;

	public bool _isAtStartup = true;
	public Text _lazy_game_text;
	public Button gameButton;

	public Toggle serverSwitch;

	Dictionary<int,NetworkConnection> _active_players;
	Dictionary<int,NetworkConnection> _healthy_players;
	Dictionary<int,PlayerPacket> _recentData;
	// Use this for initialization
	void Start () {

		_active_players = new Dictionary<int, NetworkConnection> ();
		_healthy_players = new Dictionary<int, NetworkConnection> ();
		_recentData = new Dictionary<int, PlayerPacket> ();
		gameButton.onClick.AddListener(stateButton);
	}
	
	// Update is called once per frame
	void Update () {

		if (_state == 1) {
			_lazy_game_text.text = "Connected: "+NetworkServer.connections.Count;
			Debug.Log(NetworkServer.connections.Count);
		} else if (_state == 2) {
			_lazy_game_text.text = "Ready Players: "+_healthy_players.Count;

		//Normal game operations
		} else if (_state == 3) {
			_lazy_game_text.text = "Game things happening";
			if (_lastFrame < (int)Time.time) {

				//Send latest data
				sendAllUpdates();
			}

		} 


	}


	//Send everyone the closest disimilar data packet
	private void sendAllUpdates(){

		foreach (KeyValuePair<int,NetworkConnection> connection in _active_players) {

			//lock this part
			PlayerPacket closestPacket = null;
			//double min_dist = 
			foreach (KeyValuePair<int,PlayerPacket> datum in _recentData){
					
				if (datum.Key != connection.Key) {
					PlayerPacket temp = datum.Value;
					if (closestPacket == null) {
						closestPacket = temp;
						 
					} else if ( distance(_recentData[connection.Key],datum.Value) <
						distance(_recentData[connection.Key],closestPacket) ) {

						closestPacket = temp;
					}

				}
			}

			//Send out closest different type
			if (closestPacket != null) {
				NetworkServer.SendToClient(connection.Key,666,closestPacket);
			}
		}
	}

	private double distance(PlayerPacket p_first, PlayerPacket p_second){

		double dist =  System.Math.Sqrt ((p_first._x * p_first._x - p_second._x * p_second._x) + (p_first._y * p_first._y - p_second._y * p_second._y));
		return dist;
	}

	public void playerUpdate(NetworkMessage p_msg){

		PlayerPacket newPacket = p_msg.ReadMessage<PlayerPacket>();

		if (_recentData.ContainsKey(newPacket._id)){
			_recentData.Add (newPacket._id, newPacket);
		}

		//assume instant response
		//PlayerPacket outMsg;
		//outMsg.

	}




	public void stateButton(){

		if (serverSwitch.isOn) {
			if (_state == 0) {

				listenOnPort ();
				gameButton.GetComponentInChildren<Text> ().text = "Ready Players?";
			} else if (_state == 1) {
				readyRequest ();
				gameButton.GetComponentInChildren<Text> ().text = "Begin Game?";
			} else if (_state == 2) {
				finalizePlayers ();
				gameButton.GetComponentInChildren<Text> ().text = "Stop Game";
			} 

			_state++;
		}
	}

	public void listenOnPort(){

		string[] network_address= _lazy_game_text.GetComponentInChildren<Text>().text.Split(':');
		Debug.Log (network_address[1]);

		NetworkServer.Listen (int.Parse(network_address[1]));
		_isAtStartup = false;
	}
		

	public void readyRequest(){

		//Send ready signals
		NetworkServer.RegisterHandler (MsgType.Ready, registerPlayer);		

		//register them
		foreach (NetworkConnection connection in NetworkServer.connections) {

			if (connection != null) {
				_active_players.Add (connection.hostId, connection);
				Debug.Log ("we added something");
			} 
		}

		//Send Ready
		foreach (KeyValuePair<int,NetworkConnection> connection in _active_players) {
			PlayerPacket outMsg = formatPacket (connection.Key, (char)0, 0, 0, 0);
			Debug.Log ("The ready I sent: " + outMsg._id);
			Debug.Log ("The ready I sent 2: " + connection.Value.hostId);
			NetworkServer.SendToClient (connection.Value.hostId, MsgType.AddPlayer, outMsg);
		}
			
	}

	//Listens for player confirmation
	public void registerPlayer(NetworkMessage p_msg){

		PlayerPacket newPacket = p_msg.ReadMessage<PlayerPacket>();
		Debug.Log ("Received this: "+newPacket._id);
		//should lock this for safety
		if (!_healthy_players.ContainsKey (newPacket._id) && _active_players.ContainsKey (newPacket._id)) {
			_healthy_players.Add (newPacket._id, _active_players [newPacket._id]);
			_recentData.Add (newPacket._id, newPacket);
		}
		Debug.Log ("Registered a participant");
	}

	public void finalizePlayers(){
		NetworkServer.UnregisterHandler (MsgType.Ready);

		_active_players = _healthy_players;
		NetworkServer.RegisterHandler (666 , playerUpdate);

	}

	PlayerPacket formatPacket(int p_id,char p_type,double p_x,double p_y, double p_z){

		PlayerPacket outMsg= new PlayerPacket();

		outMsg._id = p_id;
		outMsg._playerType  = p_type;
		outMsg._x = p_x;
		outMsg._y = p_y;
		outMsg._z = p_z;
		return outMsg;
	}
}
