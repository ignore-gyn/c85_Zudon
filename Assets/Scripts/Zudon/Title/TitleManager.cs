using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour {
	
	private int ScoreDigit = 4;
	private Transform[,] NumberObjects;
	private Transform[] currentDisplayNumberObjects;
	private Transform ScoreStringObject;
	private Transform clearScoreStringObject;
	
	// Cache of Components
	private GameMaster gameMaster;
	private Sound sound;
	
	private void Awake () {
		// Parent Component
		gameMaster = transform.parent.GetComponent<GameMaster>();
		sound = transform.parent.GetComponent<Sound>();
		
		currentDisplayNumberObjects = new Transform[ScoreDigit];
		CacheScoreNumberObjects();
	}
	
	private void OnEnable () {
		ScoreStringObject.renderer.enabled = false;
		clearScoreStringObject.renderer.enabled = false;
		for (int i = 0; i < ScoreDigit; i++) {
			currentDisplayNumberObjects[i] = null;
			for (int j = 0; j < 10; j++) {
				NumberObjects[i, j].renderer.enabled = false;
			}
		}
		
		DisplayScore(gameMaster.HighScore);
	}
	
	private void Update () {
		// ゲーム開始
		if (Input.GetButtonDown("Zudon")) {
			sound.PlaySE(sound.startSE); 
			gameMaster.NextGameState = GameMaster.GameState.Game;
		}
	}
	
	/// <summary>
	/// ハイスコアの表示(表示中のオブジェクトを無効にし,新しく表示するオブジェクトを有効にする)
	/// </summary>
	/// <param name="score">表示スコア</param>
	public void DisplayScore (int score) {
		int[] number = new int[ScoreDigit];
		
		if (score == 0) {
			ScoreStringObject.renderer.enabled = false;
			clearScoreStringObject.renderer.enabled = false;
			return;
		} else if (score >= gameMaster.clearScore) {
			ScoreStringObject.renderer.enabled = false;
			clearScoreStringObject.renderer.enabled = true;
		} else {
			ScoreStringObject.renderer.enabled = true;
			clearScoreStringObject.renderer.enabled = false;
		}
				
		foreach (Transform numberObject in currentDisplayNumberObjects) {
			if (numberObject != null) numberObject.renderer.enabled = false;
		}
		
		int validDegit = DecomposeNumber(score, ref number);
		
		for (int i = 0; i < ScoreDigit; i++) {
			currentDisplayNumberObjects[i] = NumberObjects[i, number[i]];
			currentDisplayNumberObjects[i].renderer.enabled = true;
		}
	}
	
	private void CacheScoreNumberObjects () {
		ScoreStringObject = transform.Find("HighScore");
		clearScoreStringObject = transform.Find("HighScore/Score_Clear");
		NumberObjects = new Transform[ScoreDigit, 10];
		for (int i = 0; i < 10; i++) {
			for  (int j = 0; j < ScoreDigit; j++) {
				NumberObjects[j, i] = transform.Find("HighScore/Score/Score" + (j+1) + "degit/Number_" + i);
			}
		}
	}
	
	private int DecomposeNumber (int number, ref int[] result) {
		int validDegit = 0;
		
		for (int i = 0; i < result.Length; i++) {
			result[i] = number % 10;
			if (number >= 10) {
				number = (number - result[i]) / 10;
			} else {
				if (validDegit == 0) validDegit = i+1;
				number = 0;
			}
		}
		return validDegit;
	}
}
