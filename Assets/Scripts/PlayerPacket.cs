using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerPacket : MessageBase {

	public int _id;
	public double _x;
	public double _y;
	public double _z;
	public char _playerType;

}
