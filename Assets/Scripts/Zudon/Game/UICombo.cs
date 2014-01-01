using UnityEngine;
using System.Collections;


public class UICombo : MonoBehaviour, IComponents {
	
	private int ComboDigit = 3;
	private Transform[,] ComboNumberObjects;
	private Transform[] currentDisplayNumberObjects;
	private Transform ComboStringObject;
	
	// Cache of Components
	private UIController uiCtrl;
	
	
	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		
		currentDisplayNumberObjects = new Transform[ComboDigit];
		CacheComboNumberObjects();
	}
	
	public void _Start () {
		ComboStringObject.renderer.enabled = false;
		
		for (int i = 0; i < ComboDigit; i++) {
			currentDisplayNumberObjects[i] = null;
			for (int j = 0; j < 10; j++) {
				ComboNumberObjects[i, j].renderer.enabled = false;
			}
		}
	}
	
	/// <summary>
	/// コンボの表示(表示中のオブジェクトを無効にし,新しく表示するオブジェクトを有効にする)
	/// </summary>
	/// <param name="combo">表示コンボ数</param>
	public void DisplayCombo (int combo) {
		int[] number = new int[ComboDigit];

		foreach (Transform numberObject in currentDisplayNumberObjects) {
			if (numberObject != null) numberObject.renderer.enabled = false;
		}
		
		int validDegit = uiCtrl.DecomposeNumber(combo, ref number);
		if (combo == 0) validDegit = 0;
		
		for (int i = 0; i < ComboDigit; i++) {
			if (i < validDegit) {
				currentDisplayNumberObjects[i] = ComboNumberObjects[i, number[i]];
				currentDisplayNumberObjects[i].renderer.enabled = true;
			} else {
				currentDisplayNumberObjects[i] = null;
			}
		}
		
		ComboStringObject.renderer.enabled = (combo != 0 ? true : false);
	}
	
	private void CacheComboNumberObjects () {
		ComboStringObject = transform.Find("ComboString");
		
		ComboNumberObjects = new Transform[ComboDigit, 10];
		for (int i = 0; i < 10; i++) {
			ComboNumberObjects[0, i] = transform.Find("Combo1degit").Find("Number_" + i);
			ComboNumberObjects[1, i] = transform.Find("Combo2degit").Find("Number_" + i);
			ComboNumberObjects[2, i] = transform.Find("Combo3degit").Find("Number_" + i);
		}
	}
}
