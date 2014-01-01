using UnityEngine;
using System.Collections;

public class GameUIController : MonoBehaviour {
/*
	public GUISkin skin;
	private string labelText = "";
	
	private CharaCtrlBase player1Ctrl;
	private CharaCtrlBase player2Ctrl;
	
	public void Awake() {
		enabled = false;
	}
	
	public void Start() {
		player1Ctrl = GetComponent<BattleController>().player1Ctrl;
		player2Ctrl = GetComponent<BattleController>().player2Ctrl;
	}
	
	/// <summary>
	/// 画面の1/8の幅を持ったインフォバーを左右に表示する（ゲーム領域が4：3になる）
	/// インフォバーにHPを表示する
	/// </summary>
	public void OnGUI () {
		float uiW = Screen.width / 8;
		float uiH = Screen.height;
		float uiX = Screen.width - uiW;
		
		if (player1Ctrl == null || player2Ctrl == null) return;
		
		int player1HP = player1Ctrl.HP;
		int player2HP = player2Ctrl.HP;
		string player1HPText;
		string player2HPText;

		player1HPText = "HP\n" +  player1HP.ToString() + "\n/1000";
		player2HPText = "HP\n" +  player2HP.ToString() + "\n/1000";
		
		GUI.skin = skin;
		
		// Create two sides boxes and show two characters' HP
		GUI.Box(new Rect(0,0,uiW,uiH), player1HPText);
		GUI.Box(new Rect(uiX, 0,uiW,uiH), player2HPText);
		
		if (labelText != "") {
			GUI.Box(new Rect(0, (Screen.height-100)/2, Screen.width, 100),
			        labelText);
		}
	}
	
	/// <summary>
	/// 勝敗結果を表示する
	/// </summary>
	public IEnumerator DisplayResult (string s) {
		// Back to main menu
		Debug.Log ("Get " + labelText);
		labelText = s;
		yield return new WaitForSeconds(3);
		//Application.LoadLevel(Application.loadedLevel - 1);
	}
*/
}
