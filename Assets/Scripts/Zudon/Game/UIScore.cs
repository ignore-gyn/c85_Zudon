using UnityEngine;
using System.Collections;

public class UIScore : MonoBehaviour, IComponents {
	
	private int ScoreDigit = 4;
	
	// Cache of Components
	private UIController uiCtrl;
	
	private SpriteRenderer[] spriteRenderer;
	private UnityEngine.Sprite[] spriteNumberArray;
	
	
	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		spriteNumberArray = uiCtrl.spriteCollection.number45;
		
		spriteRenderer = new SpriteRenderer[ScoreDigit];
		for (int i = 0; i < ScoreDigit; i++) {
			spriteRenderer[i] = transform.Find("Score" + (i+1) + "degit").GetComponent<SpriteRenderer>();
		}
	}
	
	public void _Start () {
		for (int i = 0; i < ScoreDigit; i++) {
			spriteRenderer[i].sprite = spriteNumberArray[0];
		}
	}
	
	/// <summary>
	/// スコアの表示
	/// </summary>
	/// <param name="score">表示スコア</param>
	public void DisplayScore (int score) {
		int[] number = new int[ScoreDigit];
		
		uiCtrl.DecomposeNumber(score, ref number);
		for (int i = 0; i < ScoreDigit; i++) {
			spriteRenderer[i].sprite = spriteNumberArray[number[i]];
		}
	}
	
}
