using UnityEngine;
using System.Collections;

public class UIBase : MonoBehaviour, IComponents {

	public float AlphaSpeed; 		// 明滅スピード
	public float MinAlpha;			// アルファ最小値
	
	//--------------------------------------------------------------
	
	private Transform baseUI;
	private Color color;
	
	private float rangeAlpha;
	private float middleAlpha;
	
	// Cache of Components
	private UIController uiCtrl;
	private GameManager gameManager;
	
	
	public void _Awake () {
		// Ancestor Components
		uiCtrl = transform.parent.GetComponent<UIController>();
		gameManager = uiCtrl.gameManager;
		
		// Set constant parameters
		color = renderer.material.color;
		middleAlpha = (1.0f + MinAlpha) * 0.5f;
		rangeAlpha = (1.0f - MinAlpha) * 0.5f;
	}
	
	public void _Start () {
		;
	}

	// Blink by changing alpha parameter
	public void Blink () {
		if (!renderer.enabled) return;
		
		float alpha = Mathf.Sin(gameManager.GameFrame * AlphaSpeed) * rangeAlpha + middleAlpha;
		color.a = alpha;
		renderer.material.SetColor("_Color", color);
	}
}
