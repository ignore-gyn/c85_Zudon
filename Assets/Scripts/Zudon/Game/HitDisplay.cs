using UnityEngine;
using System.Collections;

public class HitDisplay : MonoBehaviour {
	private int frame = 0;
	
	private Color spriteColor;
	private float alpha;
	private float alphaSpeed = 20;
	
	private float posYSpeed = 40;
	
	// Use this for initialization
	void Start () {
		spriteColor = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		frame++;
		
		//alpha = Mathf.Log(frame, alphaSpeed);
		//alpha = Mathf.PingPong(frame, alphaSpeed)  / alphaSpeed;
		//alpha = Mathf.Sin(frame * alphaSpeed) * alphaRange + middleAlpha;
		alpha = Mathf.Sin(frame / alphaSpeed) ;
		spriteColor.a = alpha;
		renderer.material.SetColor("_Color", spriteColor);
		
		Vector3 pos = transform.position;
		//pos.y += posYSpeed * Mathf.Log(frame, alphaSpeed);
		//pos.y += posYSpeed * frame * frame;
		pos.y += Mathf.Sin(frame / posYSpeed) * 0.01f ;
		transform.position = pos;

		if (alpha <= 0) {
			Destroy(gameObject);
		}
	}
}
