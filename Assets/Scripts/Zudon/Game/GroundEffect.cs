using UnityEngine;
using System.Collections;

public class GroundEffect : MonoBehaviour {
	public float alphaSpeed; 		// 明滅スピード
	public float minAlpha;			// アルファ最小値
	
	private Color spriteColor;
	private float alpha;
	
	private float alphaRange;
	private float middleAlpha;

	private int frame;
	
	private void Start () {
		spriteColor = renderer.material.GetColor("_TintColor");
		
		middleAlpha = (1.0f + minAlpha) * 0.5f;
		alphaRange = (1.0f - minAlpha) * 0.5f;
		
		spriteColor.a = middleAlpha;
		renderer.material.SetColor("_TintColor", spriteColor);
		
		frame = 0;
	}
	
	private void Update () {
		if (!renderer.enabled) return;
		
		alpha = Mathf.Sin(frame * alphaSpeed) * alphaRange + middleAlpha;
		spriteColor.a = alpha;
		renderer.material.SetColor("_TintColor", spriteColor);
		
		frame++;
	}
}
