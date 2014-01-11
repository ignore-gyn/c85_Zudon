using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetController : MonoBehaviour, IComponents {

	public GameObject targetPrefabs;
	private int targetNum = 5;
	private int feverTargetNum = 10;
	public GameObject explosionPrefab;
	
	private Target[] targets;
	//private List<Target> targets;
	
	// Target behavior parameters
	public float rangeMoveY = 0.9f;		// 0.3, 0.5, 0.2 
	//public float rangeMoveZ;
	public float moveSpeedY = 0.005f;	// 0.08, 0.08, 0.08
	//public float moveSpeedZ;
	
	private float[] targetSpeedOptions = new float[] {4, 6, 9, 12, 18};
	private int targetSpeedOptionsNum = 5;
	//private int targetRespawnCycle = 36;			// Respawn時に無敵タイムが必要かなー
	
	private int targetAllDestroyed;						// すべての的が破壊された時間
	private int targetRespawnInterval = 36;		// すべての的が破壊されてからRespawnまでの時間
	
	// Cache of Components
	public GameManager gameManager;
	
	public void _Awake () {
		// Parent Component
		gameManager = transform.parent.GetComponent<GameManager>();
	}
	
	public void _Start () {
		// Random.seed = 
		if (targets != null) Cleanup();
		targets = new Target[feverTargetNum];
		//targets = new List<Target>();
		
		for (int i = 0; i < feverTargetNum; i++) {
			GameObject targetObject = Instantiate(targetPrefabs) as GameObject;
			Target target = targetObject.AddComponent("Target") as Target;
			//target = targetObject.GetComponent<Target>();
			targets[i] = target;
			//targets.Add(target);
			
			targetObject.transform.position = target.positionMatrix[i];
			target .transform.parent = transform;
			
			target.targetCtrl = this;
			target.id = i;
			target.initialPosition = target.transform.position;		// 本来固定だが, Sceneビュー上で動かしても反映されるように
			InitTarget(target);
			
			
			if (i >= targetNum) {
				target.state = Target.State.Dead;
				target.gameObject.SetActive(false);
			}
		}
	}
	
	
	public void _Update () {
		bool isRespawn = true;		// Deadじゃない的があったらフラグを倒してRespawnしない
	
		for (int i = 0; i < feverTargetNum; i++) {
			Target target = targets[i];
			
			switch (target.state) {
			case Target.State.Alive:
				/*int time = gameManager.GameFrame - target.spawnTime;
				if (time < 20) {
					target.transform.localScale = new Vector3(1 * Mathf.Sin(time / 20 *  Mathf.PI) * 1.1f, 1 * Mathf.Sin(time / 20 *  Mathf.PI) * 1.1f, 1);
				} else if (time < 30) {
					target.transform.localScale = new Vector3(1 * (1-Mathf.Sin(time / 10 *  Mathf.PI) * 0.05f), 1 * (1-Mathf.Sin(time / 20 *  Mathf.PI) * 0.05f), 1);
				}*/
			
			
				Vector3 targetPos = target.initialPosition;
				targetPos.y += Mathf.Sin(gameManager.GameFrame * moveSpeedY * target.speed) * rangeMoveY;
				//targetPos.z += Mathf.Sin(gameManager.GameFrame * moveSpeedZ) * rangeMoveZ;
				target.rigidbody.MovePosition(targetPos);
				
				isRespawn  = false;
				break;
				
			case Target.State.Hit:
				/*
				if (target.collisionInfo == null && target.colliderInfo == null ) {
					Debug.Log("Not set collision information.");
					break;
				}
				if (target.collisionInfo != null) {
					ContactPoint contact = target.collisionInfo.contacts[0];
					Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
					Vector3 pos = contact.point;
					//Instantiate(explosionPrefab, pos, rot);
				}
				target.collisionInfo = null;
				target.colliderInfo = null;
				*/
				
				//Instantiate(explosionPrefab, target.transform.position, target.transform.rotation);
							
				target.state = Target.State.Dead;
				target.deadTime = gameManager.GameFrame;
				target.gameObject.SetActive(false);
				
				isRespawn  = false;
				break;
				
			case Target.State.Dead:
				break;
			}
		}
		
		// すべての的が破壊されたらRespawn
		if (isRespawn) {
			if (targetAllDestroyed == 0) targetAllDestroyed = gameManager.GameFrame;
			
			if (gameManager.GameFrame - targetAllDestroyed >= targetRespawnInterval) {
				for (int i = 0; i < feverTargetNum; i++) {
					
					if (!gameManager.isFeverTime) {
						if (i >= targetNum) break;
					}
					
					targets[i].gameObject.SetActive(true);
					InitTarget(targets[i]);
				}
				targetAllDestroyed = 0;		// タイマーリセット
			}
		}
	}
	
	private void InitTarget (Target target) {
		target.transform.position = target.initialPosition;
		target.speed = targetSpeedOptions[Random.Range (0, targetSpeedOptionsNum)];
		target.state = Target.State.Alive;
		target.collisionInfo = null;
		target.colliderInfo = null;
		target.deadTime = 0;
		target.spawnTime = gameManager.GameFrame;
	}
	
	
	public void Cleanup () {
		for (int i = 0; i < feverTargetNum; i++) {
			Destroy(targets[i].gameObject);
		}
	}
}
