using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;
using UnityEngine.UI;

public sealed class Chessboard : MonoBehaviour 
{
	//He Singleton
	private static Chessboard _instance;
	public static Chessboard instance {
		get {
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<Chessboard>();
			return _instance;
		}
	}

	// Attributs
	private List<Piece> pieces;
	private Dictionary<string, GameObject> positions;

	// Use this for initialization
	void Start () {
		pieces = new List<Piece> ();
		positions = new Dictionary<string, GameObject>();
		pieces.Add (new Piece ("pawn1", "A7"));
		pieces.Add (new Piece("pawn2", "B7"));
		pieces.Add (new Piece("pawn3", "C7"));
		pieces.Add (new Piece("pawn4", "D7"));
		pieces.Add (new Piece("pawn5", "E7"));
		pieces.Add (new Piece("pawn6", "F7"));
		pieces.Add (new Piece("pawn7", "G7"));
		pieces.Add (new Piece("pawn8", "H7"));
		pieces.Add (new Piece("rook1", "A8"));
		pieces.Add (new Piece("knight1", "B8"));
		pieces.Add (new Piece("bishop1","C8"));
		pieces.Add (new Piece("queen", "D8"));
		pieces.Add (new Piece("king", "E8"));
		pieces.Add (new Piece("bishop2", "F8"));
		pieces.Add (new Piece("knight2", "G8"));
		pieces.Add (new Piece("rook2", "H8"));

		for (int i = 1; i <= 8; i++) {
			for (char c = 'A'; c <= 'H'; c++)
			{
				string name = c+""+i;
				positions.Add(name, GameObject.Find(name));
			} 
		}

		Debug.Log("[FRED] Hello! My name is : " + instance.name);
		//ProcessMessage ("D2D4");
		InitBtPlugin ();
	}

#if UNITY_ANDROID
	private void InitBtPlugin()
	{
		AndroidJNI.AttachCurrentThread ();

		/*
		// DEBUG
		Debug.Log ("[FRED] Creating AndroidJavaClass ...");
		AndroidJavaClass debug_btPlugin = new AndroidJavaClass("com.artchess.bluetoothplugin.BluetoothPluginActivity");
		Debug.Log ("[FRED] Getting message ...");
		string message = debug_btPlugin.CallStatic<string> ("GetMessage");
		Debug.Log ("[FRED] Obtained message = " + message);
		*/
		
		Debug.Log ("[FRED] Getting BluetoothManager instance ...");
		AndroidJavaClass btPlugin = new AndroidJavaClass("com.artchess.bluetoothplugin.BluetoothManager");
		
		Debug.Log ("[FRED] Init Bluetooth connexion ...");
		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				btPlugin.CallStatic("init", currentActivity);
			}
		}

	}
#endif
	//string[] from = {"D7", "B8", "C7", "C8", "E7", "D8"};
	//string[] to = {"D5", "A6", "C6", "F5", "E5", "H4"};

	// Update is called once per frame
	void Update () {
		foreach (var piece in pieces) {
			if (piece.inMovement) {
				piece.obj.transform.Translate((piece.newPosition-piece.obj.transform.position) * Time.deltaTime, Space.World);
				if (piece.obj.transform.position == piece.newPosition) {
					piece.inMovement = false;
				}
			}
		}
	}


	// Function that will be called by the Java Plugin
	// Handle incoming message
	public void ProcessMessage(string message) {
		Debug.Log ("[FRED] Received message = " + message);
		ParseMove.Parse (message);
	}


	// Prend deux positions (sous forme de strings) en paramètre.
	// Cherche s'il y a une pièce à la position "from" :
	// Si oui, bouge cette pièce en position "to" et renvoie vrai,
	// Sinon, renvoie faux.
	public Boolean Move(string from, string to) {
		Debug.Log ("[FRED] Move from " + from + " to " + to);
		Piece piece = pieces.Find (x => x.position == from);
		if (piece == null) {
			Debug.Log ("[FRED] Piece is null");
			return false;
		}
		GameObject newPosition = new GameObject();
		if (! positions.TryGetValue (to, out newPosition)) {
			Debug.Log ("[FRED] NOT TryGetValue");
			return false;
		}
		Debug.Log (newPosition.transform.position);
		//piece.obj.transform.position = newPosition.transform.position;
		piece.newPosition = newPosition.transform.position;
		piece.inMovement = true;
		piece.position = to;
		return true;
	}
	
	// Prend une position (sous forme de string) en paramètre.
	// Cherche s'il y a une pièce à la position "pos" :
	// Si oui, supprime cette pièce du plateau et renvoie vrai,
	// Sinon, renvoie faux.
	public Boolean Kill(string pos) {
		Piece piece = pieces.Find (x => x.position == pos);
		if (piece == null) {
			return false;
		}
		/*Debug.Log("test kill 1");
		FractureObject f = (FractureObject) piece.obj.GetComponent<FractureObject>();
		Debug.Log("test kill 2");
		f.FractureAtPoint(piece.obj.GetComponent<MeshFilter>().mesh.bounds.center, new Vector3(100F,100F,100F));
		Debug.Log("test kill 3");*/
		piece.obj.SetActive (false);
		return true;
	}
	
	
	public Boolean LittleCastling ()
	{
		// récupérer le roi
		Piece king = pieces.Find (x => x.name == "king");
		if (king == null) {
			return false;
		}
		// récupérer la tour
		Piece rook = pieces.Find (x => x.name == "rook2");
		if (rook == null) {
			return false;
		}
		// bouger le roi
		GameObject kingNewPosition = new GameObject();
		if (! positions.TryGetValue ("G8", out kingNewPosition)) {
			return false;
		}
		king.obj.transform.position = kingNewPosition.transform.position;
		king.position = "G8";
		// bouger la tour
		GameObject rookNewPosition = new GameObject();
		if (! positions.TryGetValue ("F8", out rookNewPosition)) {
			return false;
		}
		rook.obj.transform.position = rookNewPosition.transform.position;
		rook.position = "F8";
		
		return true;
	}


	public Boolean BigCastling ()
	{
		// récupérer le roi
		Piece king = pieces.Find (x => x.name == "king");
		if (king == null) {
			return false;
		}
		// récupérer la tour
		Piece rook = pieces.Find (x => x.name == "rook1");
		if (rook == null) {
			return false;
		}
		// bouger le roi
		GameObject kingNewPosition = new GameObject ();
		if (! positions.TryGetValue ("C8", out kingNewPosition)) {
			return false;
		}
		king.obj.transform.position = kingNewPosition.transform.position;
		king.position = "C8";
		// bouger la tour
		GameObject rookNewPosition = new GameObject ();
		if (! positions.TryGetValue ("D8", out rookNewPosition)) {
			return false;
		}
		rook.obj.transform.position = rookNewPosition.transform.position;
		rook.position = "D8";
		
		return true;
	}
	
	
	public void Reset () {
		ResetOne ("rook1",   "A8");
		ResetOne ("knight1", "B8");
		ResetOne ("bishop1", "C8");
		ResetOne ("queen",  "D8");
		ResetOne ("king",   "E8");
		ResetOne ("bishop2", "F8");
		ResetOne ("knight2", "G8");
		ResetOne ("rook2",   "H8");
		ResetOne ("pawn1",   "A7");
		ResetOne ("pawn2",   "B7");
		ResetOne ("pawn3",   "C7");
		ResetOne ("pawn4",   "D7");
		ResetOne ("pawn5",   "E7");
		ResetOne ("pawn6",   "F7");
		ResetOne ("pawn7",   "G7");
		ResetOne ("pawn8",   "H7");
	}
	
	
	public void ResetOne (string name, string pos) {
		Piece pc = pieces.Find (x => x.name == name);
		pc.obj.SetActive (true);

		GameObject newPosition = new GameObject ();
		positions.TryGetValue (pos, out newPosition);
		pc.obj.transform.position = newPosition.transform.position;
		pc.position = pos;
	}
}
