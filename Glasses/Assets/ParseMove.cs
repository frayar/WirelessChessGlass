using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ParseMove {

	// Attributes
	public const string MOVE_PATTERN = "^[A-Ha-h][1-8][A-Ha-h][1-8]$";
	public const string KILL_PATTERN = "^KILL\\_([A-Ha-h][1-8])$";
	public const string LITTLE_CASTLING_PATTERN = "^LITTLE\\_CASTLING$";
	public const string BIG_CASTLING_PATTERN = "^BIG\\_CASTLING$";
	public const string RESET_PATTERN = "^RESET$";

	/// <summary>
	/// Parse the specified inputText.
	/// </summary>
	/// <param name="inputText">Input text.</param>
	static public void Parse(string inputText) {

		inputText = inputText.ToUpper ();

		// Classic Move
		Match match = Regex.Match (inputText, MOVE_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			ExecuteMove (inputText);
			return;
		}

		// Kill order
		match = Regex.Match (inputText, KILL_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			ExecuteKill (match.Groups[1].Value);
			return;
		}

		// Little castling order
		match = Regex.Match (inputText, LITTLE_CASTLING_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			ExecuteLittleCastling();
			return;
		}

		// Big castling order
		match = Regex.Match (inputText, BIG_CASTLING_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			ExecuteBigCastling();
			return;
		}

		// Reset order
		match = Regex.Match (inputText, RESET_PATTERN, RegexOptions.IgnorePatternWhitespace);
		if (match.Success) {
			ExecuteReset();
			return;
		}
	}

	/// <summary>
	/// Executes the move.
	/// </summary>
	/// <param name="inputText">Input text.</param>
	static public void ExecuteMove(string inputText) {
		Debug.Log ("Execute Move : " + inputText);
		Chessboard.instance.Move (inputText.Substring(0,2), inputText.Substring(2,2));
	}

	/// <summary>
	/// Executes the kill.
	/// </summary>
	/// <param name="inputText">Input text.</param>
	static public void ExecuteKill(string inputText) {
		Debug.Log ("Execute Kill : " + inputText);
		Chessboard.instance.Kill (inputText);
	}

	/// <summary>
	/// Executes the little castling.
	/// </summary>
	static public void ExecuteLittleCastling() {
		Debug.Log ("Execute Little Castling");
		Chessboard.instance.LittleCastling ();
	}

	/// <summary>
	/// Executes the big castling.
	/// </summary>
	static public void ExecuteBigCastling() {
		Debug.Log ("Execute Big Castling");
		Chessboard.instance.BigCastling ();
	}

	/// <summary>
	/// Executes the reset.
	/// </summary>
	static public void ExecuteReset() {
		Debug.Log ("RESET");
		Chessboard.instance.Reset ();
	}
}
