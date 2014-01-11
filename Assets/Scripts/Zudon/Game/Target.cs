using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {
	
	public enum State {
		Alive,
		Hit,
		Dead,
	}
	
	// Set by TargetController
	// constant
	[HideInInspector] public TargetController targetCtrl;
	[HideInInspector] public int id;
	[HideInInspector] public Vector3 initialPosition;
	
	// variable
	[HideInInspector] public float speed;
	[HideInInspector] public State state;						// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collision collisionInfo;		// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collider colliderInfo;		// also changed by Target Instance only on hit with bullet
	[HideInInspector] public int deadTime;
	public int spawnTime;
	//[HideInInspector] public int score;
	
	public Vector3[] positionMatrix = new Vector3[] {	
		new Vector3(2.0f, 1.6f, 0),
		new Vector3(2.8f, 1.6f, 0),
		new Vector3(3.6f, 1.6f, 0),
		new Vector3(4.4f, 1.6f, 0),
		new Vector3(5.2f, 1.6f, 0),
		
		new Vector3(2.4f, 1.6f, 0),
		new Vector3(3.2f, 1.6f, 0),
		new Vector3(4.0f, 1.6f, 0),
		new Vector3(4.8f, 1.6f, 0),
		new Vector3(5.6f, 1.6f, 0),
	};
	
	// 衝突判定と通知しかしない
	
/*
	// ぶつかったオブジェクトがBulletであれば、通知・破壊処理を行う
	// Collider, Trigger両対応
	private void JudgeHit (Collision other) {
		if (state != State.Alive) return;
		if (other.gameObject.layer != (int)GameMaster.Layer.Bullet) return;
		
		state = State.Hit;
		collisionInfo = other;
	}
	
	private void JudgeHit (Collider other) {
		if (state != State.Alive) return;
		if (other.gameObject.layer != (int)GameMaster.Layer.Bullet) return;
		
		state = State.Hit;
		colliderInfo = other;
	}
*/
	// Bulletから衝突メッセージを受け取った時, TargetControllerへ通知	
	public void Hit () {
		state = State.Hit;
	}
}
