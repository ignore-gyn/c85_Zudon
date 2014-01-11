using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour {
	
	private int ScoreDigit = 4;
	
	//private Transform[,] NumberObjects;
	//private Transform[] currentDisplayNumberObjects;
	//private Transform ScoreStringObject;
	//private Transform clearScoreStringObject;
	
	// Cache of Components
	private GameMaster gameMaster;
	private Sound sound;
	private SpriteCollection spriteCollection;
	
	private SpriteRenderer rendererScoreBase;
	private UnityEngine.Sprite[] spriteScoreBaseArray;
	private SpriteRenderer[] rendererNumber;
	private UnityEngine.Sprite[] spriteNumberArray;
	
	
	private void Awake () {
		// Parent Component
		gameMaster = transform.parent.GetComponent<GameMaster>();
		sound = transform.parent.GetComponent<Sound>();
		spriteCollection = transform.parent.GetComponent<SpriteCollection>();
		
		spriteScoreBaseArray = spriteCollection.titleScoreBase;
		spriteNumberArray = spriteCollection.number45;
		
		rendererScoreBase =  transform.Find("HighScore").GetComponent<SpriteRenderer>();
		rendererNumber = new SpriteRenderer[ScoreDigit];
		for (int i = 0; i < ScoreDigit; i++) {
			rendererNumber[i] = transform.Find("HighScore/Score" + (i+1) + "degit").GetComponent<SpriteRenderer>();
		}
	}
	
	private void OnEnable () {
		if (gameMaster.currentGameState != GameMaster.GameState.Title) return;
		
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
	/// ハイスコアの表示
	/// </summary>
	/// <param name="score">表示スコア</param>
	private void DisplayScore (int score) {
		int[] number = new int[ScoreDigit];
		
		if (score <= 0) {
			rendererScoreBase.sprite = null;
			return;
		} else if (score < gameMaster.clearScore) {
			rendererScoreBase.sprite = spriteScoreBaseArray[0];
		} else {
			rendererScoreBase.sprite = spriteScoreBaseArray[1];
		}
		
		int validDegit = DecomposeNumber(score, ref number);
		for (int i = 0; i < ScoreDigit; i++) {
			if (i < validDegit) { 
				rendererNumber[i].sprite = spriteNumberArray[number[i]];
			} else {
				rendererNumber[i].sprite = null;
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
