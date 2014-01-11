using UnityEngine;
using System.Collections;

public class UIChargeGauge : MonoBehaviour, IComponents {

	public void _Awake () {
		;
	}
	
	public void _Start () {
		renderer.material.SetFloat("_Cutoff", 1);
	}
	
	/// <summary>
	/// チャージゲージの表示, /Custom/AlphaMaskを使用
	/// </summary>
	/// <param name="threshold">チャージ値(0～1)</param>
	public void DisplayChargeGauge (float threshold) {
		float alpha = 1 - threshold;
		renderer.material.SetFloat("_Cutoff", alpha);
	}
}
