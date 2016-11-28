using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleMaps : MonoBehaviour {


	string url = "";
	/// <summary>
	/// Langitude/latitude of area. default Karachi is set
	/// </summary>
	public float playerLat =  24.917828f;

	public float playerLong = 67.097096f;

	public float targetLat =  24.917828f;
	public float targetLong = 67.097096f;

	public GameObject player;
	public GameObject target;
	public Plane gameMap;

	LocationInfo li;
	/// <summary>
	/// Maps on Google Maps have an integer 'zoom level' which defines the resolution of the current view.
	/// Zoom levels between 0 (the lowest zoom level, in which the entire world can be seen on one map) and 
	/// 21+ (down to streets and individual buildings) are possible within the default roadmap view. 
	/// </summary>
	public int zoom = 14;
	/// <summary>
	/// not more then 640 * 640 
	/// </summary>
	public int mapWidth = 640;
	public int mapHeigh = 640;

	public enum mapType { roadmap, satellite, hybrid, terrain };
	public mapType mapSelected;

	/// <summary>
	/// scale can be 1,2 for free plan and can also be 4 for paid
	/// </summary>
	public int scale;
	bool loadingMap;

	IEnumerator GetGoogleMap()
	{
		url = "https://maps.googleapis.com/maps/api/staticmap?center=" + playerLat + "," + playerLong +
			"&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeigh + "&scale=" + scale 
			+"&maptype=" + mapSelected +
			"&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyCY4mkF9uTLWQzbh0PYm4nwPQB7i-Qz9Nc";
		loadingMap = true;
		WWW www = new WWW(url); 
		yield return www;
		loadingMap = false;
		//Assign downloaded map texture to any gameobject e.g., plane
		gameObject.GetComponent<Renderer>().material.mainTexture = www.texture;

	}


	IEnumerator getLatLong(string p_address){

		Debug.Log ("Entered function");
		string pbUrl = "https://api.pitneybowes.com/location-intelligence/geocode-service/v1/transient/basic/geocode?mainAddress\n=SANTA ANA&country=Mex&areaName1=DISTRITOFEDERAL&postalCode=44910HTTP/1.1%";


		//WWWForm webForm = new WWWForm ();
		//webForm.AddField ("Authorization", "Bearer " + "E9XWK2bFqCeNdnCswpjITHjJ92Lg");
		//Debug.Log ("Entered function: "+webForm.headers["Authorization"]);


		Dictionary<string,string> headerDict;
		headerDict = new Dictionary<string, string> ();


		headerDict.Add ("Authorization", "Bearer " + "E9XWK2bFqCeNdnCswpjITHjJ92Lg");
		WWW getWWW=new WWW(pbUrl,null, headerDict);
		yield return getWWW;
		Debug.Log ("Processed request");
		//getWWW.
		Debug.Log (getWWW.bytesDownloaded);
 	}


	// Use this for initialization
	void Start () {
		StartCoroutine (GetGoogleMap ());
		StartCoroutine( getLatLong (""));
	}
	
	// Update is called once per frame
	void Update () {
	
		//gameMap.

	}
}
