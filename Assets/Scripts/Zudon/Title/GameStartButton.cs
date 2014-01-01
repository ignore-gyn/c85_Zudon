using UnityEngine;
using System.Collections;

public class GameStartButton : MonoBehaviour {

	public int frame = 0;

 	private Color spriteColor;
	private float alpha;
	
	public float alphaSpeed; 	
 	public float minAlpha;
 	private float alphaRange;
 	private float middleAlpha;
	
	void Start () {
		spriteColor = renderer.material.color;
		
		middleAlpha = (1.0f + minAlpha) * 0.5f;
		alphaRange = (1.0f - minAlpha) * 0.5f;
		
		spriteColor.a = middleAlpha;
	}
	
	void Update () {
	
		if (!renderer.enabled) return;

		alpha = Mathf.Sin(frame * alphaSpeed) * alphaRange + middleAlpha;
		spriteColor.a = alpha;
		renderer.material.SetColor("_Color", spriteColor);
		
		frame++;
	}
}
