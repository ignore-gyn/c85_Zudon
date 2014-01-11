using UnityEngine;
using System.Collections;


public class UICombo : MonoBehaviour, IComponents {
	
	private int ComboDigit = 3;
	private Transform ComboStringObject;
	
	// Cache of Components
	private UIController uiCtrl;
	
	private SpriteRenderer[] spriteRenderer;
	private UnityEngine.Sprite[] spriteNumberArray;
	
	
	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		spriteNumberArray = uiCtrl.spriteCollection.number45;
		
		ComboStringObject =  transform.Find("ComboString");
		spriteRenderer = new SpriteRenderer[ComboDigit];
		for (int i = 0; i < ComboDigit; i++) {
			spriteRenderer[i] = transform.Find("Combo" + (i+1) + "degit").GetComponent<SpriteRenderer>();
		}
	}
	
	public void _Start () {
		ComboStringObject.renderer.enabled = false;
		
		for (int i = 0; i < ComboDigit; i++) {
			spriteRenderer[i].sprite = null;
		}
	}
	
	/// <summary>
	/// コンボの表示
	/// </summary>
	/// <param name="combo">表示コンボ数</param>
	public void DisplayCombo (int combo) {
		int[] number = new int[ComboDigit];
		int validDegit;
		
		if (combo == 0) {
			validDegit = 0;
			ComboStringObject.renderer.enabled = false;
		} else {
			validDegit = uiCtrl.DecomposeNumber(combo, ref number);
			ComboStringObject.renderer.enabled = true;
		}
		
		for (int i = 0; i < ComboDigit; i++) {
			if (i < validDegit) { 
				spriteRenderer[i].sprite = spriteNumberArray[number[i]];
			} else {
				spriteRenderer[i].sprite = null;
			}
		}
	}
	
}
