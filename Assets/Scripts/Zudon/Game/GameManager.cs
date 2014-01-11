using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public int gameLength = 9000;		// ゲームメイン部の時間
	public int feverTimeLength = 1000;	// フィーバー時間
	
	public int maxBombStock = 3;			// ボムストック最大数 # UIStockも直す
	public int bombExtendNum = 10;		// ボムストックが増えるコンボ数
	
	//--------------------------------------------------------------
	
	public enum State {
		Ready,
		Go,
		Main,
		Timeup,
		Over
	}
	public State state;
	public bool isFeverTime = false;
	
	private int readyTimeLength = 220;		// Ready画像が表示される時間
	private int goTimeLength = 60;			// Go画像が表示される時間
	private int timeupTimeLength = 300;	// Timeup画像が表示される時間
	private int fadeoutBGMTimeLength = 500;
	
	
	// Cache of Components
	public GameMaster gameMaster;
	public Sound sound;
	public SpriteCollection spriteCollection;
	
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
			// ゲームメイン中のみ,残り時間表示を更新
			if (state == State.Main &&
			    gameFrame >= 0 && gameFrame <= gameLength) {
				ui.uiTime.DisplayTime(gameLength - gameFrame);
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
	
	private int baseTargetScore = 5;		// 的素点
	private int[] coefficientTargetScore = new int[] {1, 3, 5, 10, 20};	// 連続命中時の点数倍率
	public int[] scoreArray;			// 的点数（素点ｘ倍率）
	
	private int score;
	public int Score {
		get {	return score; }
		
		set {
			score = (value > 9999 ? 9999 : value);
			ui.uiScore.DisplayScore(score);
		}
	}
	
	private int combo;
	public int Combo {
		get {	return combo; }
		
		set {
			combo = (value > 999 ? 999 : value);
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
		spriteCollection = transform.parent.GetComponent<SpriteCollection>();
		
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
		scoreArray = new int[coefficientTargetScore.Length];
		for (int i = 0; i < coefficientTargetScore.Length; i++) {
			scoreArray[i] = baseTargetScore * coefficientTargetScore[i];
		}
	}
	
	private void OnEnable () {
		if (gameMaster.currentGameState != GameMaster.GameState.Game) return;
		
		Input.GetKey(KeyCode.Z);
		Input.GetKey(KeyCode.X);
		
		foreach (IComponents child in childComponents) {
			child._Start();
		}
		
		// Initialize parameters
		state = State.Ready;
		GameFrame = - readyTimeLength - goTimeLength;
		isFeverTime = false;
		
		Charge = 0;
		Score = 0;
		Combo = 0;
		BombStock = 0;
		
		ui.uiTime.DisplayTime(gameLength);
		
		sound.PlayBGM(sound.GameBGM_Intro, sound.GameBGM_Loop);
	}
	
	private void Update () {
		GameFrame++;
		
		switch (state) {
		case State.Ready:
			if (GameFrame >= -goTimeLength) {
				state = State.Go;
			}
			break;
			
		case State.Go:
			if (GameFrame >= 0) {
				state = State.Main;
			}
			break;
			
		case State.Main:
			if (GameFrame >= gameLength) {
				state = State.Timeup;
				isFeverTime = false;
			} else if (GameFrame >= gameLength - feverTimeLength) {
				isFeverTime = true;
			}
			break;
			
		case State.Timeup:
			
			// クリアスコア以上でガッツポーズ
			/////////////// Playerで処理する
			if (GameFrame >= gameLength +100 && Score >= gameMaster.clearScore) {
				player.chara.animation.Play("guts");
			}
			//////////
			
			// BGMフェードアウト
			sound.FadeOutBGM(fadeoutBGMTimeLength);
			if (GameFrame >= gameLength + timeupTimeLength) {	
				sound.StopBGM();
				
				gameMaster.HighScore = Score;
				state = State.Over;
				gameMaster.NextGameState = GameMaster.GameState.Title;
				return;
			}
			break;
			
		case State.Over:
			return;
		}
		
		player._Update();
		targets._Update();
	}
}
