using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, IComponents {
	//public KeyCode shotButton = KeyCode.Z; 
	//public KeyCode bombButton = KeyCode.X; 
	
	public GameObject bulletPrefab;
	//public GameObject[] hitPrefabs = new GameObject[5];
	
	// # 仕様（制約）
	// * ショット中はボム発動しない
	// * コンボは,的に当たったかor画面外に出たか,で判定する
	// 		"的に当たった"あるいは"画面外に出た"ことを検出した時点で,
	//		コンボがつながるか否かを判定する（射出順は考慮しない）
	//		"地面に当たった"ことを条件に加える場合は, 地面のColliderを有効にする
	//		矢はID管理しているため, 射出順に判定することも可能である
	
	public int shotCoolTime = 20;	// 矢を放った後の硬直Frame数
	public int chargeSpeed = 200;	// チャージがMAXになるまでのFrame数
	
	public int bombCoolTime = 150;	// ボム発動中の硬直Frame数
	
	//--------------------------------------------------------------
	
	// 入力
	private int inputAccum = 0;
	private int prevInputAccum;
	private bool isBombInput = false;

	// チャージ
	private float chargeCoefficient;				// 経過時間に対するチャージ値算出係数
	
	// 照準（Axisで操作する）
	public Transform sight;						// 照準
	private Transform sightLockon;			// 会有り表示
	public Transform launch;						// 矢の原点
	private Vector3 initiSight;					// 照準の初期位置
	private float sightYSpeed = 0.01f;		// 照準スピード(毎Frameの移動量)
	private float highChargeMaxAngle = 57.0f;	// 最大射出角度( = HighChargeモーションの矢の角度)
	private int bendMinLength = 55;			// Bendアニメーションのフレーム数は72(60FPS換算)
	private float currentSightAngle;			// 現在の射出角度
	private int sightFadeoutLength = 8;		// 照準が消えるまでのフレーム数
	
	// 矢
	private int shotTime;							// 矢をうったFrame
	private int bulletCount;						// うった矢の数（BulletID（通し番号）に使用）
	private int prevHitBulletID;
	private List<Bullet> bullets;
	
	// ボム
	private int bombTime;							// ボムをうったFrame
	
	// キャラの状態
	private delegate void SubState();
	private SubState subState;
	
	// For Animation Name
	public Transform chara;
	private string animIdle = "wait";
	private string animBend = "Bend";
	private string animCharge = "Charge";
	private string animHighCharge = "HighCharge";
	private string animRelease = "Release";
	
	// Cache of Components
	public GameManager gameManager;
	
	
	public void _Awake () {
		// Parent Component
		gameManager = transform.parent.GetComponent<GameManager>();
		
		// Child Components
		chara = transform.Find("CharaA");
		sight = transform.Find("Sight");
		launch = transform.Find("Launch");
		initiSight = sight.position;
		sightLockon = sight.Find("SightLockOn");
		
		// Set constant parameters
		chargeCoefficient = (float)1 / (chargeSpeed * chargeSpeed);		
	}
	
	public void _Start () {
		if (bullets != null) Cleanup();
		bullets = new List<Bullet>();
		
		inputAccum = 0;
		bulletCount = 0;
		sight.position = initiSight;
		sight.renderer.enabled = false;
		sightLockon.renderer.enabled = false;
		
		subState = OnIdle;
		//chara.animation[animIdle].speed = 0.4f;
		chara.animation.Play(animIdle);
	}
	
	public void _Update () {
		if (gameManager.state != GameManager.State.Main) {
			chara.animation.CrossFade(animIdle, 1.0f);
			return;
		}
		
		// ショットボタンの入力を調べる
		if (Input.GetButton("Zudon")) {
			inputAccum++;
		} else if (Input.GetButtonUp("Zudon")) {
			inputAccum = 0;
		}
		
		// ボムボタンの入力を調べる
		/*if (Input.GetButton("SuperZudon")) {
			isBombInput = true;
		} else {
			isBombInput = false;
		}*/
		
		subState();
		
		if (bullets != null) bullets.RemoveAll(bullet => !bullet._Update());
	}
	
	// 待機中
	private void OnIdle () {
		if (inputAccum == 1) {
			chara.animation.Play(animBend);
			//chara.animation.CrossFadeQueued(animCharge, 0.0f, QueueMode.CompleteOthers);
			
			gameManager.sound.PlaySE(gameManager.sound.setSE); 
			
			sight.renderer.enabled = true;
			SetRendererAlpha(sight.renderer, 0);
			currentSightAngle = 0;
			subState = OnCharge;
			
		} else if (isBombInput && gameManager.BombStock > 0) {
			bombTime = gameManager.GameFrame;
			gameManager.BombStock--;
			//chara.animation.Play("Attack4");
			chara.animation.CrossFadeQueued(animIdle, 0.0f, QueueMode.CompleteOthers);
			subState = OnBomb;
		}
	}
	
	// ボム発動中
	private void OnBomb () {
		//if (gameManager.GameFrame - bombTime >= bombCoolTime) {
			subState = OnIdle;
		//}
	}
	
	// 弓を引いている間
	private void OnCharge () {
		// 矢の生成
		if (inputAccum == 0) {
			shotTime = gameManager.GameFrame;
			chara.animation.Play(animRelease);
			chara.animation.CrossFadeQueued(animIdle, 0.0f, QueueMode.CompleteOthers);
			subState = OnRelease;
			
			if (gameManager.Charge > 0) sightLockon.renderer.enabled = true;
			MakeBullet();

		// 射出窓の表示フェードイン
		} else if (inputAccum <= bendMinLength) {
			float alpha = Mathf.Sin((float)inputAccum / bendMinLength * 0.5f * Mathf.PI);
			SetRendererAlpha(sight.renderer, alpha);
		
		// 照準(射角)のコントロール
		} else {
			float vertical = Input.GetAxis("Vertical");
			
			if (currentSightAngle <= highChargeMaxAngle) {
				Vector3 pos = sight.position;
				pos.y += vertical * sightYSpeed;
				if (pos.y < initiSight.y) pos.y = initiSight.y;
				float angle = Vector3.Angle(initiSight - launch.position, pos - launch.position);
				if (angle <= highChargeMaxAngle) {
					sight.position = pos;
					currentSightAngle = angle;
				}
			}
			
			chara.animation.Stop(animBend);
			float blendRatio = (float)currentSightAngle/highChargeMaxAngle;
			chara.animation.Blend(animHighCharge, blendRatio, 0.0f);
			chara.animation.Blend(animCharge, 1-blendRatio, 0.0f);
		}
		
		// チャージ値の算出
		prevInputAccum = inputAccum - bendMinLength;
		if (prevInputAccum <= 1) {
			gameManager.Charge = 0;
		} else if (prevInputAccum > chargeSpeed) {
			gameManager.Charge = 1;
		} else {
			gameManager.Charge = Mathf.Log(prevInputAccum-1, chargeSpeed-1);
		}
	}
	
	// 矢の発射と硬直中
	private void OnRelease () {
		float alpha = 1 - (float)(gameManager.GameFrame - (shotTime + shotCoolTime - sightFadeoutLength)) / sightFadeoutLength;
		SetRendererAlpha(sight.renderer, alpha);
		//SetRendererAlpha(sightLockon.renderer, alpha);
		
		if (gameManager.GameFrame - shotTime >= shotCoolTime) {
			sight.renderer.enabled = false;
			sight.position = initiSight;
			sightLockon.renderer.enabled = false;
			
			subState = OnIdle;
		}
	}
	
	
	// 矢の生成
	private void MakeBullet () {
		GameObject bulletObject = Instantiate(bulletPrefab,
																new Vector3(sight.position.x, sight.position.y, sight.position.z),
		                                      					chara.rotation) as GameObject;
		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullets.Add(bullet);
		
		bullet.transform.parent = transform;
		bullet.player = this;
		bullet._Awake(bulletCount++);
		
		gameManager.sound.PlaySE(gameManager.sound.shootSE); 
	}
	
	private void Cleanup () {
		foreach (Bullet bullet in bullets) {
			Destroy(bullet.gameObject);
		}
	}
	
	private void SetRendererAlpha (Renderer r, float alpha) {
		Color color = r.material.color;
		if (alpha < 0) {
			alpha = 0;
		} else if (alpha > 1) {
			alpha = 1;
		}
		color.a = alpha;
		r.material.color = color;
	}
}
