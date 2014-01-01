using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour, IComponents {

	// Cache of Components
	public GameManager gameManager;
	private Player player;
	
	public UIBase uiBase;
	public UITime uiTime;
	public UIChargeGauge uiChargeGauge;
	public UIScore uiScore;
	public UIStock uiStock;
	public UICombo uiCombo;
	public UIReady uiReady;
	public UIGo uiGo;
	public UITimeup uiTimeup;
	// <-- Add Child UI Components
	private List<IComponents> uiChildComponents = new List<IComponents>();
	
	
	public void _Awake () {
		// Parent Component
		gameManager = transform.parent.GetComponent<GameManager>();
		player = gameManager.player;
		
		// Child Components	
		uiBase = transform.Find("Base").GetComponent<UIBase>();
		uiChildComponents.Add (uiBase);
		
		uiTime = transform.Find("Time").GetComponent<UITime>();
		uiChildComponents.Add (uiTime);
		
		uiChargeGauge = transform.Find("ChargeGauge").GetComponent<UIChargeGauge>();
		uiChildComponents.Add (uiChargeGauge);
		
		uiScore  = transform.Find("Score").GetComponent<UIScore>();
		uiChildComponents.Add (uiScore);
		
		uiStock = transform.Find("Stock").GetComponent<UIStock>();
		uiChildComponents.Add (uiStock);
		
		uiCombo = transform.Find("Combo").GetComponent<UICombo>();
		uiChildComponents.Add (uiCombo);
		
		uiReady = transform.Find("Ready").GetComponent<UIReady>();
		uiChildComponents.Add (uiReady);
		
		uiGo = transform.Find("Go").GetComponent<UIGo>();
		uiChildComponents.Add (uiGo);
		
		uiTimeup = transform.Find("Timeup").GetComponent<UITimeup>();
		uiChildComponents.Add (uiTimeup);
		
		foreach (IComponents child in uiChildComponents) {
			child._Awake();
		}
	}
	
	public void _Start () {
		foreach (IComponents child in uiChildComponents) {
			child._Start();
		}
	}
	
	private void FixedUpdate () {
		// 子UIオブジェクトの更新
		uiBase.Blink();
		
		switch (gameManager.state) {
		case GameManager.State.Ready:
			uiReady.Blink();
			uiGo.renderer.enabled = false;
			uiTimeup.renderer.enabled = false;
			break;
		case GameManager.State.Start:
			uiGo.Blink();
			uiReady.renderer.enabled = false;
			uiTimeup.renderer.enabled = false;
			break;
		case GameManager.State.Game:
			uiReady.renderer.enabled = false;
			uiGo.renderer.enabled = false;
			uiTimeup.renderer.enabled = false;
			break;
		case GameManager.State.Timeup:
			uiTimeup.Blink();
			uiReady.renderer.enabled = false;
			uiGo.renderer.enabled = false;
			break;
		}	
	}
	
	/// <summary>
	/// 数字を1桁ずつ切り出して返す（桁数は渡された配列サイズ依存）
	/// </summary>
	/// <returns>有効な桁数(number=100, result.Length=4の場合3. number=0の場合1)</returns>
	/// <param name="number">分解する数字</param>
	/// <param name="result">結果を格納する配列(余った桁は0で埋める)</param>
	public int DecomposeNumber (int number, ref int[] result) {
		int validDegit = 0;
		
		for (int i = 0; i < result.Length; i++) {
			result[i] = number % 10;
			if (number >= 10) {
				number = (number - result[i]) / 10;
			} else {
				if (validDegit == 0) validDegit = i+1;
				number = 0;
			}
		}
		return validDegit;
	}
}
