using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ParseInput : MonoBehaviour {

	InputField inputField;
	public const string MOVE_PATTERN = "^[A-Ha-h][1-8][A-Ha-h][1-8]$";
	public const string KILL_PATTERN = "^KILL\\_([A-Ha-h][1-8])$";
	public const string LITTLE_CASTLING_PATTERN = "^LITTLE\\_CASTLING$";
	public const string BIG_CASTLING_PATTERN = "^BIG\\_CASTLING$";
	public const string RESET_PATTERN = "^RESET$";

	// Use this for initialization
	void Start () {
		inputField = GameObject.Find ("InputField").GetComponent<InputField>();
		inputField.onEndEdit.AddListener (Parse);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Parse(string inputText) {
		inputText = inputText.ToUpper ();
		Match match = Regex.Match (inputText, MOVE_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			inputField.text = "";
			ExecuteMove (inputText);
			return;
		}
		match = Regex.Match (inputText, KILL_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			inputField.text = "";
			ExecuteKill (match.Groups[1].Value);
			return;
		}
		match = Regex.Match (inputText, LITTLE_CASTLING_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			inputField.text = "";
			ExecuteLittleCastling();
			return;
		}
		match = Regex.Match (inputText, BIG_CASTLING_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			inputField.text = "";
			ExecuteBigCastling();
			return;
		}
		match = Regex.Match (inputText, RESET_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			inputField.text = "";
			ExecuteReset();
			return;
		}
	}
	
	public void ExecuteMove(string inputText) {
		Debug.Log ("Execute Move : " + inputText);
		Chessboard.instance.Move (inputText.Substring(0,2), inputText.Substring(2,2));
	}
	
	public void ExecuteKill(string inputText) {
		Debug.Log ("Execute Kill : " + inputText);
		Chessboard.instance.Kill (inputText);
	}
	
	public void ExecuteLittleCastling() {
		Debug.Log ("Execute Little Castling");
		Chessboard.instance.LittleCastling ();
	}
	
	public void ExecuteBigCastling() {
		Debug.Log ("Execute Big Castling");
		Chessboard.instance.BigCastling ();
	}

	public void ExecuteReset() {
		Debug.Log ("RESET");
		Chessboard.instance.Reset ();
	}
}
