using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	public enum State {
		Alive,
		Hit,
		Dead,
		Attacked,
	}
	
	public GameObject[] hitPrefabs = new GameObject[5];
	
	// 初期パラメータ
	public Player player;
	public GameManager gameManager;
	
	public int bulletID;
	[HideInInspector] public State state;						// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collision collisionInfo;		// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collider colliderInfo;		// also changed by Target Instance only on hit with bullet
	public int power;
	public int hitCount;
	
	private float bulletInitialVelocity = 16.0f;	
	
	
	public void _Awake (int bulletId) {
		gameManager = player.gameManager;
		
		bulletID = bulletId;
		state = State.Alive;
		collisionInfo = null;
		colliderInfo = null;
		power = Mathf.FloorToInt(gameManager.Charge * 4) + 1;	// 最大5枚割れる
		hitCount = 0;
		
		// charaが矢を握っている座標（矢の原点）と照準の座標から速度ベクトルを算出
		// chargeによって弾の強さ,Axisによって弾の射角(Y方向のみ)をコントロール
		Vector3 moveDirection = player.sight.position - player.launch.position;
		moveDirection.z = 0;
		rigidbody.velocity = moveDirection.normalized * bulletInitialVelocity * (gameManager.Charge + 0.1f);
	}
	
	public bool _Update () {
		switch (state) {
		case State.Attacked:
		case State.Alive:
			transform.LookAt(transform.position + rigidbody.velocity);
			break;
			
		case State.Hit:
			gameManager.Combo++;
			hitCount++;
			
			// 矢のpowerによって打ち抜ける枚数が変わる
			// また一度でも的に当たった矢にフラグを立てる
			state = State.Attacked;
			if (power <= hitCount && !gameManager.isFeverTime) {
				state = State.Dead;
			}
			
			// ヒット表示
			int hitNum = hitCount;
			if (hitCount >= 5) hitNum = 5;
			
			GameObject hit = Instantiate(hitPrefabs[hitNum -1]) as GameObject;
			hit.transform.parent = transform.parent;
			Vector3 hitpos = transform.position;
			hitpos.x += 0.8f;
			hit.transform.position = hitpos;
			
			// スコア
			int[] score = gameManager.scoreArray;
			if (hitNum < score.Length && hitNum > 0) {
				gameManager.Score += score[hitNum-1];
			} else {
				gameManager.Score += score[score.Length-1];
			}
			
			// SE
			gameManager.sound.PlaySE(gameManager.sound.hitSE); 
			
			return true;
			
		case State.Dead:
			Destroy(gameObject);
			return false;
		}
		
		if (!CheckIsVisible()) {
			if (state != State.Attacked) gameManager.Combo = 0;
			state = State.Dead;
		}
		return true;
	}
	
	private bool CheckIsVisible () {
		Vector3 viewArea = Camera.main.WorldToViewportPoint(transform.position);
		
		if (viewArea.x < -0.2f || viewArea.x > 1.2f ||
		    viewArea.y < -0.2f || viewArea.y > 1.2f ) {
			return false;
		} else {
			return true;
		}
	}
	
	// 壁・床への衝突を判定
	/*private void OnTriggerEnter (Collider other) {
		if (!isAlive) return;
	
		if (other.gameObject.layer == (int)GameMaster.Layer.Wall ||
			other.gameObject.layer == (int)GameMaster.Layer.Ground) {			
			//Debug.Log("Broken");
			isAlive = false;
		}
	}*/
	
	// ぶつかったオブジェクトがTargetであれば、通知・破壊処理を行う
	// Collider, Trigger両対応
	private void JudgeHit (Collision other) {
		if (state != State.Alive &&  state != State.Attacked) return;
		if (other.gameObject.layer != (int)GameMaster.Layer.Target) return;
		
		other.gameObject.SendMessage ("Hit");
		
		state = State.Hit;
		collisionInfo = other;
	}
	
	private void JudgeHit (Collider other) {
		if (state != State.Alive &&  state != State.Attacked) return;
		if (other.gameObject.layer != (int)GameMaster.Layer.Target) return;
		
		other.gameObject.SendMessage ("Hit");
		
		state = State.Hit;
		colliderInfo = other;
	}
	
	private void OnCollisionEnter (Collision other) {
		JudgeHit(other);
	}
	/*private void OnCollisionStay (Collision other) {
		JudgeHit(other);
	}*/
	private void OnCollisionExit (Collision other) {
		JudgeHit(other);
	}
	
	private void OnTriggerEnter (Collider other) {
		JudgeHit(other);
	}
	/*private void OnTriggertay (Collider other) {
		JudgeHit(other);
	}*/
	private void OnTriggerExit (Collider other) {
		JudgeHit(other);
	}
}
