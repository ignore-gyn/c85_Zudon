using UnityEngine;
using System.Collections;

public class UITimeup : MonoBehaviour, IComponents {

	private int frame = 0;

 	private Color spriteColor;
	private float alpha;
	
	private float alphaSpeed = 0.05f; 	
 	private float minAlpha = 0.4f;
 	private float alphaRange;
 	private float middleAlpha;
	
	public void _Awake () {
		spriteColor = renderer.material.color;
		
		middleAlpha = (1.0f + minAlpha) * 0.5f;
		alphaRange = (1.0f - minAlpha) * 0.5f;
		
		spriteColor.a = middleAlpha;
	}
	
	public void _Start () {
		renderer.enabled = false;
	}
	
	public void Blink () {
		//if (!renderer.enabled) return;
		renderer.enabled = true;
		
		alpha = Mathf.Sin(frame * alphaSpeed) * alphaRange + middleAlpha;
		spriteColor.a = alpha;
		renderer.material.SetColor("_Color", spriteColor);
		
		frame++;
	}
}
