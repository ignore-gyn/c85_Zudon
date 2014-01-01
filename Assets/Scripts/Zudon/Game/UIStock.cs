using UnityEngine;
using System.Collections;

public class UIStock : MonoBehaviour, IComponents {

	private int maxBombStock = 3;
	private Transform[] StockObjects;
	
	
	public void _Awake () {
		CacheStockObjects();
	}
	
	public void _Start () {
		DisplayStock(0);
	}
		
	/// <summary>
	/// ボムストックの表示
	/// </summary>
	/// <param name="bombCount">ボムストック数</param>
	public void DisplayStock (int bombCount) {
		for (int i = 0; i < maxBombStock; i++) {
			StockObjects[i].renderer.enabled = (i < bombCount ? true : false);
		}
	}
	
	private void CacheStockObjects () {
		StockObjects = new Transform[maxBombStock];
		
		for (int i = 0; i < maxBombStock; i++) {
			StockObjects[i] = transform.Find("Stock" + (i+1));
			StockObjects[i].renderer.enabled = false;
		}
	}
}
