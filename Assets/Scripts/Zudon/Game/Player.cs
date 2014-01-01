using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, IComponents {
	//public KeyCode shotButton = KeyCode.Z; 
	//public KeyCode bombButton = KeyCode.X; 
	
	public GameObject bulletPrefab;
	public GameObject[] hitPrefabs = new GameObject[5];
	
	// # 仕様（制約）
	// * ショット中はボム発動しない
	// * コンボは、的に当たったか、画面外に出たか、で判定する
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
	private float chargeCoefficient;	// 経過時間に対するチャージ値算出係数
	
	// 照準（Axisで操作する）
	private int sightYSpeed = 100;		// 照準スピード(	最大照準角度になるまでのFrame数)
	private float sightYMax = 0.8f;
	//private float sightHorizontalSpeed = 1.0f;
	private Transform sight;
	private Vector3 initiSight;
	
	// 矢
	private float bulletInitialVelocity = 16.0f;	
	private int shotTime;		// 矢をうったFrame
	private int bulletCount;	// うった矢の数（BulletID（通し番号）に使用）
	private int prevHitBulletID;
	private List<Bullet> bullets;
	
	// ボム
	private int bombTime;		// ボムをうったFrame
	
	// キャラの状態
	private delegate void SubState();
	private SubState subState;
	
	// For Animation Name
	public Transform chara;
	private string animIdle = "wait";
	private string animBend = "Bend";
	private string animCharge = "Charge";
	private string animRelease = "Release";
	

	// Cache of Components
	private GameManager gameManager;
	
	
	public void _Awake () {
		// Parent Component
		gameManager = transform.parent.GetComponent<GameManager>();
		
		// Child Components
		chara = transform.Find("CharaA");
		sight = transform.Find("Sight");
		initiSight = sight.position;
		
		// Set constant parameters
		chargeCoefficient = (float)1 / (chargeSpeed * chargeSpeed);
	}
	
	public void _Start () {
		if (bullets != null) Cleanup();
		bullets = new List<Bullet>();
		
		inputAccum = 0;
		bulletCount = 0;
		
		subState = OnIdle;
		//chara.animation[animIdle].speed = 0.4f;
		chara.animation.Play(animIdle);
	}
	
	public void _Update () {
		if (gameManager.state != GameManager.State.Game) {
			chara.animation.CrossFade(animIdle, 1.0f);
			return;
		}
		
		// ショットボタンの入力を調べる
		if (Input.GetButton("Zudon")) {
			inputAccum++;
		} else {
			inputAccum = 0;
		}
		
		// ボムボタンの入力を調べる
		/*if (Input.GetButton("SuperZudon")) {
			isBombInput = true;
		} else {
			isBombInput = false;
		}*/
		
		subState();
		if (bullets != null) bullets.RemoveAll(delegate(Bullet b){ return !UpdateBullet(b); });
	}
	
	// 待機中
	private void OnIdle () {
		if (inputAccum == 1) {
			chara.animation.Play(animBend);
			chara.animation.CrossFadeQueued(animCharge, 0.0f, QueueMode.CompleteOthers);
			
			gameManager.sound.PlaySE(gameManager.sound.setSE); 
			
			sight.position = initiSight;
			subState = OnBend;
			
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
	private void OnBend () {
		// 照準のコントロール
		//float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		sight.position += new Vector3(0, vertical * 0.01f, 0);
		
		if (inputAccum == 0) {
			shotTime = gameManager.GameFrame;
			chara.animation.Play(animRelease);
			chara.animation.CrossFadeQueued(animIdle, 0.0f, QueueMode.CompleteOthers);
			subState = OnRelease;
			
			gameManager.sound.PlaySE(gameManager.sound.shootSE); 
			
			// 矢の生成
			MakeBullet();
		}
		
		// チャージ値の算出
		prevInputAccum = inputAccum;
		if (prevInputAccum == 0) {
			gameManager.Charge = 0;
		} else if (prevInputAccum > chargeSpeed) {
			gameManager.Charge = 1;
		} else {
			//gameManager.Charge = (float)prevInputAccum / chargeSpeed;
			//gameManager.Charge = (float)1 - chargeCoefficient * (prevInputAccum-chargeSpeed) * (prevInputAccum-chargeSpeed);
			gameManager.Charge = Mathf.Log(prevInputAccum-1, chargeSpeed-1);
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
		GameObject bulletObject = Instantiate(bulletPrefab,
		                                      					new Vector3(chara.position.x, chara.position.y+0.8f, chara.position.z),
		                                      					chara.rotation) as GameObject;
		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullets.Add(bullet);
		
		bullet.transform.parent = transform;
		
		bullet.player = this;
		bullet.bulletID = bulletCount++;
		bullet.state = Bullet.State.Alive;
		bullet.collision = null;
		bullet.collider = null;
		bullet.power = Mathf.FloorToInt(gameManager.Charge * 4) + 1;	// 最大5枚割れる
		bullet.currentPower = bullet.power;
		bullet.hitCount = 0;
		
		MakeBullet_type2(bullet);
	}
/*	
	// (1)chargeによって,弾の速さ,弾の射角をコントロール
	private void MakeBullet_type1 (Bullet bulletScript) {
		bulletScript.BulletPower = charge;
		
		// 正攻法
		velocity = initialVelocity + BulletPower * 1.5f;
		angle = initialAngle - BulletPower * 32.0f;
		//Debug.Log("velocity = " + velocity + ", angle = " + angle + ", power = " + BulletPower);
		
		x0 = rigidbody.position.x;
		y0 = rigidbody.position.y;
		gravity = gravityA * 0.5f;
	}
	
	private void UpdateBullet_type1 (Bullet bulletScript) {
		// 正攻法
		float x = x0 + velocity * elapsed * Mathf.Cos(angle * Mathf.Deg2Rad);
		float y = y0 + (velocity * elapsed * Mathf.Sin(angle * Mathf.Deg2Rad) - gravity * elapsed * elapsed);
		transform.LookAt(new Vector3(x, y, 0));
		rigidbody.MovePosition(new Vector3(x, y, 0));
		elapsed++;
		//Debug.Log("x = " + x + ", y = " + y);
	}
*/	
	// (2)chargeによって弾の強さ,Axisによって弾の射角(Y方向のみ)をコントロール
	private void MakeBullet_type2 (Bullet bullet) {		
		Vector3 moveDirection = (sight.position - bullet.transform.position).normalized;
		bullet.rigidbody.velocity = moveDirection * bulletInitialVelocity * gameManager.Charge;
	}
	
	
	private bool UpdateBullet (Bullet bullet) {
		switch (bullet.state) {
		case Bullet.State.Attacked:
		case Bullet.State.Alive:
			bullet.transform.LookAt (bullet.transform.position + bullet.rigidbody.velocity);
			break;
			
		case Bullet.State.Hit:
			gameManager.Combo++;
			bullet.hitCount++;
			
			// 矢のpowerによって打ち抜ける枚数が変わる, また一度でも的に当たった矢にフラグを立てる
			bullet.state = Bullet.State.Attacked;
			//if (bullet.currentPower > 0) bullet.currentPower--;
			if (bullet.power <= bullet.hitCount) {
				// フィーバー中は矢は死なない
				if (gameManager.GameFrame < gameManager.GameLength - gameManager.feverTime) {
					bullet.state = Bullet.State.Dead;
				}
			}
			
			// ヒット表示
			int hitNum = bullet.hitCount;
			if (bullet.hitCount >= 5) hitNum = 5;
			
			//Debug.Log ( bullet.hitCount);
			GameObject hit = Instantiate(hitPrefabs[hitNum -1]) as GameObject;
			hit.transform.parent = transform;
			Vector3 hitpos = bullet.transform.position;
			hitpos.x += 0.8f;
			hit.transform.position = hitpos;
			
			// スコア
			switch (hitNum -1) {
			case 0:
				gameManager.Score += 5;
				break;
			case 1:
				gameManager.Score += 15;
				break;
			case 2:
				gameManager.Score += 25;
				break;
			case 3:
				gameManager.Score += 50;
				break;
			case 4:
				gameManager.Score += 100;
				break;
			default:
				gameManager.Score += 100;
				break;
			}
			
			// SE
			gameManager.sound.PlaySE(gameManager.sound.hitSE); 
			
			return true;
			
		case Bullet.State.Dead:
			Destroy(bullet.gameObject);
			return false;
		}
		
		if (!CheckIsVisible(bullet)) {
			if (bullet.state != Bullet.State.Attacked) gameManager.Combo = 0;
			bullet.state = Bullet.State.Dead;
		}
		return true;
	}
	
	private bool CheckIsVisible (Bullet bullet) {
		Vector3 viewArea = Camera.main.WorldToViewportPoint(bullet.transform.position);
		
		if (viewArea.x < -0.2f || viewArea.x > 1.2f ||
		    viewArea.y < -0.2f || viewArea.y > 1.2f ) {
			return false;
		} else {
			return true;
		}
	}
	
	private void Cleanup () {
		foreach (Bullet bullet in bullets) {
			Destroy(bullet.gameObject);
		}
	}
}
