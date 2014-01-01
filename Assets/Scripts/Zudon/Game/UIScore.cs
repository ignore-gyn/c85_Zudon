using UnityEngine;
using System.Collections;

public class UIScore : MonoBehaviour, IComponents {
	
	private int ScoreDigit = 4;
	private Transform[,] NumberObjects;
	private Transform[] currentDisplayNumberObjects;
	
	// Cache of Components
	private UIController uiCtrl;
	
	
	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		
		currentDisplayNumberObjects = new Transform[ScoreDigit];
		CacheScoreNumberObjects();
	}
	
	public void _Start () {
		for (int i = 0; i < ScoreDigit; i++) {
			currentDisplayNumberObjects[i] = null;
			for (int j = 0; j < 10; j++) {
				NumberObjects[i, j].renderer.enabled = false;
			}
		}
	}
	
	/// <summary>
	/// スコアの表示(表示中のオブジェクトを無効にし,新しく表示するオブジェクトを有効にする)
	/// </summary>
	/// <param name="score">表示スコア</param>
	public void DisplayScore (int score) {
		int[] number = new int[ScoreDigit];
		
		foreach (Transform numberObject in currentDisplayNumberObjects) {
			if (numberObject != null) numberObject.renderer.enabled = false;
		}
		
		int validDegit = uiCtrl.DecomposeNumber(score, ref number);
		if (score == 0) validDegit = 0;
		
		for (int i = 0; i < ScoreDigit; i++) {
			currentDisplayNumberObjects[i] = NumberObjects[i, number[i]];
			currentDisplayNumberObjects[i].renderer.enabled = true;
		}
		
	}
	
	private void CacheScoreNumberObjects () {
		NumberObjects = new Transform[ScoreDigit, 10];
		for (int i = 0; i < 10; i++) {
			for  (int j = 0; j < ScoreDigit; j++) {
				NumberObjects[j, i] = transform.Find("Score" + (j+1) + "degit/Number_" + i);
			}
		}
	}
}
