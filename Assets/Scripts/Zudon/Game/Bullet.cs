using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	public enum State {
		Alive,
		Hit,
		Dead,
		Attacked,
	}
	
	// 生成時にCharaが設定するパラメータ
	public Player player;
	public int bulletID;
	[HideInInspector] public State state;				// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collision collision;	// also changed by Target Instance only on hit with bullet
	[HideInInspector] public Collider collider;		// also changed by Target Instance only on hit with bullet
	public int power;
	public int currentPower;
	public int hitCount;
	
	// 衝突判定と通知しかしない
	
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
		collision = other;
	}
	
	private void JudgeHit (Collider other) {
		if (state != State.Alive &&  state != State.Attacked) return;
		if (other.gameObject.layer != (int)GameMaster.Layer.Target) return;
		
		other.gameObject.SendMessage ("Hit");
		
		state = State.Hit;
		collider = other;
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
