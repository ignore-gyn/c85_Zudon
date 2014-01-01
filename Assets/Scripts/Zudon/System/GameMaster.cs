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

	//--------------------------------------------------------------
	
	public enum Layer {
		UI = 8,
		Bullet,
		Target,
		Ground,
		Wall,
	}
	
	public enum GameState {
		None = -1,
		Title = 0,
		Game,
	};

	public GameState NextGameState { get; set; }
	private GameState currentGameState = GameState.Title;
	
	private int highScore = -1;
	public int HighScore {
		get {
			return highScore;
		}
		
		set {
			if (highScore < value) {
				highScore = value;
				SaveData(highScore);
			}
		}
	}
	
	private string saveFileName = "sv.ign";

	// Cache of Components
	private GameObject titleScene;
	private GameObject gameScene;
	// <-- Add Scene Objects.
	
	private void Start () {
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
		
		HighScore = LoadData();
		currentGameState = GameState.None;
		NextGameState = GameState.Title;
	}
	
	private void Update () {
		// シーン遷移条件判定
		switch(currentGameState) {
		case GameState.Title:
			break;
		case GameState.Game:
			break;
		}		
		
		// シーンの切り替え
		while(NextGameState != GameState.None) {
			currentGameState = NextGameState;
			NextGameState = GameState.None;
			
			switch(currentGameState) {
			case GameState.Title:
				titleScene.SetActive(true);
				gameScene.SetActive(false);
				break;
			case GameState.Game:
				titleScene.SetActive(false);
				gameScene.SetActive(true);
				break;
			}
		}
	}
	
	// Save&Load Data
	private void SaveData (int score) {
		FileStream fs = new FileStream(saveFileName, FileMode.Create, FileAccess.ReadWrite);
		BinaryWriter  writer = new BinaryWriter(fs);
		
		writer.Write(score);
		fs.Close();
	}
	
	private int LoadData () {
		FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
