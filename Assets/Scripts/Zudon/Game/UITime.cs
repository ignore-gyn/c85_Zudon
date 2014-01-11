using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITime : MonoBehaviour, IComponents {
	
	private int TimeDigit = 4;
	
	// Cache of Components
	private UIController uiCtrl;
	
	private SpriteRenderer[] spriteRenderer;
	private UnityEngine.Sprite[][] spriteNumberArray;


	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		
		spriteNumberArray = new UnityEngine.Sprite[4][];
		spriteNumberArray[0] = uiCtrl.spriteCollection.number25;
		spriteNumberArray[1] = uiCtrl.spriteCollection.number25;
		spriteNumberArray[2] = uiCtrl.spriteCollection.number45;
		spriteNumberArray[3] = uiCtrl.spriteCollection.number45;
				
		spriteRenderer = new SpriteRenderer[TimeDigit];
		for (int i = 0; i < TimeDigit; i++) {
			spriteRenderer[i] = transform.Find("Time" + (i+1) + "degit").GetComponent<SpriteRenderer>();
		}
	}
	
	public void _Start () {
		for (int i = 0; i < TimeDigit; i++) {
			spriteRenderer[i].sprite = spriteNumberArray[i][0];
		}
	}
	
	/// <summary>
	/// 残り時間の表示
	/// </summary>
	/// <param name="time">表示秒数（残りFrame数）</param>
	public void DisplayTime (int time) {
		int[] number = new int[TimeDigit];
		int second = time % 100;
		int minutes = (time >= 100 ? (time - second)/100 : 0);
		time = minutes * 100 + second;

		uiCtrl.DecomposeNumber(time, ref number);
		for (int i = 0; i < TimeDigit; i++) {
			spriteRenderer[i].sprite = spriteNumberArray[i][number[i]];
		}
	}
	
}
