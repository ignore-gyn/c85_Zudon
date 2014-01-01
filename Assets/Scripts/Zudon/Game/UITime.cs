using UnityEngine;
using System.Collections;

public class UITime : MonoBehaviour, IComponents {
	
	private int TimeDigit = 4;
	private Transform[,] TimeNumberObjects;
	private Transform[] currentDisplayNumberObjects;
	
	// Cache of Components
	private UIController uiCtrl;
	

	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		
		currentDisplayNumberObjects = new Transform[TimeDigit];
		CacheTimeNumberObjects();
	}
	
	public void _Start () {
		for (int i = 0; i < TimeDigit; i++) {
			currentDisplayNumberObjects[i] = null;
			for (int j = 0; j < 10; j++) {
				TimeNumberObjects[i, j].renderer.enabled = false;
			}
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
		
		foreach (Transform numberObject in currentDisplayNumberObjects) {
			if (numberObject != null) numberObject.renderer.enabled = false;
		}
		
		uiCtrl.DecomposeNumber(time, ref number);
		
		for (int i = 0; i < TimeDigit; i++) {
			currentDisplayNumberObjects[i] = TimeNumberObjects[i, number[i]];
			currentDisplayNumberObjects[i].renderer.enabled = true;
		}
	}
	
	private void CacheTimeNumberObjects () {
		TimeNumberObjects = new Transform[TimeDigit, 10];
		for (int i = 0; i < 10; i++) {
			TimeNumberObjects[0, i] = transform.Find("Time1degit").Find("Number_" + i);
			TimeNumberObjects[1, i] = transform.Find("Time2degit").Find("Number_" + i);
			TimeNumberObjects[2, i] = transform.Find("Time3degit").Find("Number_" + i);
			TimeNumberObjects[3, i] = transform.Find("Time4degit").Find("Number_" + i);
		}
	}
}
