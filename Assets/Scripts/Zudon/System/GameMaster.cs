using UnityEngine;
using System.Collections;
using System;
using System.IO;


public interface IComponents {
	void _Awake();
	void _Start();
}


public class GameMaster : MonoBehaviour {

	public int clearScore = 1000;
	public bool debugMode = false;		// true:ゲームシーンのみ（ゲーム開始前・終了後演出なし）
	
	//--------------------------------------------------------------
	
	// Layer of Objects (used for collision detection)
	public enum Layer {
		UI = 8,
		Bullet,
		Target,
		Ground,
		Wall,
	}

	// Game Scene Control	
	public enum GameState {
		None = -1,
		Title = 0,
		Game,
	};

	public GameState currentGameState = GameState.None;
	// Cache of Objects and Components
	private GameObject titleScene;
	private GameObject gameScene;
	private TitleManager titleManager;
	private GameManager gameManager;
	// <-- Add Scene Components
	
	public GameState NextGameState {
		set {
			currentGameState = value;
			
			switch (currentGameState) {
			case GameState.None:
				titleScene.SetActive(false);
				gameScene.SetActive(false);
				break;
			case GameState.Title:
				gameScene.SetActive(false);
				titleScene.SetActive(true);
				break;
			case GameState.Game:
				titleScene.SetActive(false);
				gameScene.SetActive(true);
				break;
			default:
				Debug.LogError("Invalid State is set to NextGameState");
				return;
			}
		}
	}
	
	
	// High Score displayed in Title scene
	private int highScore = -1;
	public int HighScore {
		get {
			return highScore;
		}
		
		set {
			if (highScore < value) {
				highScore = (value > 9999 ? 9999 : value);
				SaveData(highScore);
			}
		}
	}
	
	private string saveFileName = "sv.ign";

	
	private void Awake () {
		Application.targetFrameRate = 60;
	
		HighScore = LoadData();
		
		titleScene = GameObject.Find("TitleScene");
		if (titleScene == null) {
			Debug.LogError("TitleScene is not found.");
		}
		
		gameScene = GameObject.Find("GameScene");
		if (gameScene == null) {
			Debug.LogError("GameScene is not found.");
		}
		
		titleScene.SetActive(false);
		gameScene.SetActive(false);
		
		NextGameState = GameState.Title;
	}
	
	
	// Save&Load Data
	private void SaveData (int score) {
		FileStream fs = new FileStream(saveFileName,
													  FileMode.Create,
													  FileAccess.ReadWrite);
		BinaryWriter  writer = new BinaryWriter(fs);
		
		writer.Write(score);
		fs.Close();
	}
	
	private int LoadData () {
		FileStream fs = new FileStream(saveFileName,
													  FileMode.OpenOrCreate,
													  FileAccess.ReadWrite);
		BinaryReader  reader = new BinaryReader(fs);
		int score;
		
		try {
			score = reader.ReadInt32();
		} catch(EndOfStreamException) {
			score = 0;
		}
		
		fs.Close ();
		return score;
	}
}
