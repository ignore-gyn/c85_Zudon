using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public int gameLengthFrame;
	public int feverTime = 1000;

	public int maxBombStock = 3;			// ボムストック最大数 # UIStockも直す
	public int bombExtendNum = 10;		// ボムストックが増えるコンボ数
	
	//--------------------------------------------------------------
	
	public enum Layer {
		player1 = 8,
		player2 = 9,
		player1Bullet = 10,
		player2Bullet = 11
	}
	
	public enum State {
		Ready,
		Start,
		Game,
		Timeup,
		Over,
	}
	public State state;
	
	public int GameLength { get; private set; }
	
	private bool isFinish = false;
	
	
	private int gameReadyDisplayTime = 220;
	private int gameStartDisplayTime = 60;
	private int gameTimeupDisplayTime = 300;
	private int fadeoutBGMTime = 500;
	
	// Cache of Components
	public GameMaster gameMaster;
	public Sound sound;
	
	public Player player;
	public TargetController targets;
	public UIController ui;
	// <-- Add Child Components
	private List<IComponents> childComponents = new List<IComponents>();
	
	private int gameFrame;
	public int GameFrame {
		get {	return gameFrame;	}
		
		private set {
			gameFrame = value;
			if (gameFrame <= GameLength && state == State.Game) {
				ui.uiTime.DisplayTime(GameLength - gameFrame);
			}
		}
	}
	
	private float charge;	// チャージ値（0から1）
	public float Charge {
		get {	return charge; }
		
		set {
			charge = value;
			ui.uiChargeGauge.DisplayChargeGauge(charge);
		}
	}
	
	private int score;
	public int Score {
		get {	return score; }
		
		set {
			if (value > 9999) score = 9999;
			score = value;
			ui.uiScore.DisplayScore(score);
		}
	}
	
	private int combo;
	public int Combo {
		get {	return combo; }
		
		set {
			combo = value;
			ui.uiCombo.DisplayCombo(combo);
						
			if (combo > 0 && combo % bombExtendNum == 0) {
				BombStock++;
			}
			
			if (combo > 0 && combo % 10 == 0) {
				Score += 100;
			}
		}
	}

	private int bombStock;
	public int BombStock {
		get {	return bombStock; }
		
		set {
			if (bombStock < maxBombStock) {
				bombStock = value;
				//ui.uiStock.DisplayStock(bombStock);
			}
		}
	}
	
	
	private void Awake () {
		// Parent Component
		gameMaster = transform.parent.GetComponent<GameMaster>();
		sound = transform.parent.GetComponent<Sound>();
		
		// Child Components	
		player = GameObject.Find("Player").GetComponent<Player>();
		childComponents.Add (player);
		
		targets = GameObject.Find("Target").GetComponent<TargetController>();
		childComponents.Add (targets);
		
		ui = transform.Find("UI").GetComponent<UIController>();
		childComponents.Add (ui);
		
		foreach (IComponents child in childComponents) {
			child._Awake();
		}
		
		// Set constant parameters
		GameLength = gameLengthFrame;
	}

	private void OnEnable () {
		isFinish = false;
		Input.GetKey(KeyCode.Z);
		Input.GetKey(KeyCode.X);
			
		foreach (IComponents child in childComponents) {
			child._Start();
		}
		
		// Initialize parameters
		state = State.Ready;
		GameFrame = -(gameReadyDisplayTime+gameStartDisplayTime);
		
		Charge = 0;
		Score = 0;
		Combo = 0;
		BombStock = 0;
		
		ui.uiTime.DisplayTime(GameLength);
		sound.PlayBGM(sound.GameBGM_Intro, sound.GameBGM_Loop);
	}

	private void FixedUpdate () {
		GameFrame++;
		
		switch (state) {
		case State.Ready:
			if (GameFrame >= -gameStartDisplayTime) {
				state = State.Start;
			}
			break;
		
		case State.Start:
			if (GameFrame >= 0) {
				state = State.Game;
				//sound.PlayBGM(sound.GameBGM_Intro, sound.GameBGM_Loop);
			}
			break;
		
		case State.Game:
			if (GameFrame >= GameLength) {
				state = State.Timeup;
			}
			break;
			
		case State.Timeup:
			
			// クリアスコア以上でガッツポーズ
			if (GameFrame >= GameLength +100 && Score >= gameMaster.clearScore) {
				player.chara.animation.Play("guts");
			}
			
			// BGMフェードアウト
			sound.FadeOutBGM(fadeoutBGMTime);
			if (GameFrame >= GameLength + gameTimeupDisplayTime) {	
				sound.StopBGM();
				
				gameMaster.NextGameState = GameMaster.GameState.Title;
				gameMaster.HighScore = Score;
				state = State.Over;
				return;
			}
			break;
			
		case State.Over:
			return;
		}
	
		player._Update();
		targets._Update ();
	}
}