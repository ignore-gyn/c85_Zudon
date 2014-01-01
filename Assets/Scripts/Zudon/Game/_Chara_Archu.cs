using UnityEngine;
using System.Collections;

public class _Chara_Archu : MonoBehaviour, IComponents {
	public KeyCode shotButton = KeyCode.Z; 
	public KeyCode bombButton = KeyCode.X; 
	
	// # 仕様（制約）
	// # ショット中はボム発動しない
	// # コンボは当たった矢のIDが連番になっているか否かで判定（後から打った矢が先に当たるとコンボが途切れる）
	
	public int shotCoolTime = 20;	// 矢を放った後の硬直Frame数
	public int chargeSpeed = 200;	// チャージがMAXになるまでのFrame数
	
	public int bombCoolTime = 150;	// ボム発動中の硬直Frame数
	public int maxBombStock = 3;	// ボムストック最大数 # UIStockも直す
	public int bombExtendNum = 10;	// ボムストックが増えるコンボ数
	
	//--------------------------------------------------------------
	
	private int inputAccum = 0;
	private int prevInputAccum;
	private bool isBombInput = false;
	
	public float charge;		// チャージ値（0から1）
	private float chargeCoefficient;	// 経過時間に対するチャージ値算出係数
	
	private int bombTime;		// ボムをうったFrame
	private int shotTime;		// 矢を放ったFrame
	
	private delegate void SubState();
	private SubState subState;
	
	private int bulletCount;	// うった矢の数（BulletID（通し番号）に使用）
	private int prevHitBulletID;
	public int combo;
	
	public int bombStock;
	
	// Cache of Components
	private GameManager gameManager;
	
	public void _Awake () {
		// Parent Component
		gameManager = transform.parent.GetComponent<GameManager>();
		
		// Set constant parameters
		chargeCoefficient = (float)1 / (chargeSpeed * chargeSpeed);
	}
	
	public void _Start () {
		combo = 0;
		inputAccum = 0;
		bombStock = 0;
		bulletCount = 0;
		
		subState = OnIdle;
		animation.Play("Idle");
	}
	
	public void _Update () {
		// ショットボタンの入力を調べる
		if (Input.GetKey(shotButton)) {
			inputAccum++;
		} else {
			inputAccum = 0;
		}
		
		// ボムボタンの入力を調べる
		if (Input.GetKeyUp(bombButton)) {
			isBombInput = true;
		} else {
			isBombInput = false;
		}
		
		subState();
	}
	
	// 待機中
	private void OnIdle () {
		if (inputAccum == 1) {
			animation.Play("Magic");
			subState = OnBend;
			
		} else if (isBombInput && bombStock > 0) {
			bombTime = gameManager.GameFrame;
			bombStock--;
			animation.Play("Attack4");
			animation.CrossFadeQueued("Idle", 0.1f, QueueMode.CompleteOthers);
			subState = OnBomb;
		}
	}
	
	// ボム発動中
	private void OnBomb () {
		if (gameManager.GameFrame - bombTime >= bombCoolTime) {
			subState = OnIdle;
		}
	}
	
	// 弓を引いている間
	private void OnBend () {
		if (inputAccum == 0) {
			shotTime = gameManager.GameFrame;
			animation.Play("Attack1");
			animation.CrossFadeQueued("Idle", 0.1f, QueueMode.CompleteOthers);
			subState = OnRelease;
			
			// 矢の生成
			MakeBullet();
		}
		prevInputAccum = inputAccum;
		if (prevInputAccum > chargeSpeed) {
			charge = 1;
		} else if (prevInputAccum == 0) {
			charge = 0;
		} else {
			//charge = (float)prevInputAccum / chargeSpeed;
			charge = (float)1 - chargeCoefficient * (prevInputAccum-chargeSpeed) * (prevInputAccum-chargeSpeed);
		}
	}
	
	// 矢の発射と硬直中
	private void OnRelease () {
		if (gameManager.GameFrame - shotTime >= shotCoolTime) {
			subState = OnIdle;
		}
	}
	
	// 矢の生成
	private void MakeBullet () {
		GameObject bullet = Instantiate(Resources.Load("Prefabs/Bullet/BulletA"),
		            new Vector3(transform.position.x, 0, transform.position.z),
		            transform.rotation) as GameObject;
		bullet.transform.parent = GameObject.FindWithTag("game").transform;
		
		Bullet bulletScript = bullet.GetComponent<Bullet>();
		//bulletScript.BulletPower = charge;
		//bulletScript.chara = this;
		bulletScript.bulletID = bulletCount++;
	}
	
	// コンボのカウント
	public int HitBulletID {
		set {
			if (value != prevHitBulletID+1) {
				combo = 0;
			} else {
				combo++;
				if (bombStock < maxBombStock && combo % bombExtendNum == 0) {
					bombStock++;
				}
			}
			prevHitBulletID = value;
		}
	}
}
