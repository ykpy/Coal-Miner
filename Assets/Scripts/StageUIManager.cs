using UnityEngine.UI;

public class StageUIManager : SingletonMonoBehaviour<StageUIManager> {

	// 残り時間UI
	public Text timeLimit;

	void Start() {
		timeLimit.text = "";
	}

	/// <summary>
	/// 残り時間を画面に表示します
	/// </summary>
	/// <param name="limitTime">残り時間</param>
	void ShowTimeLimit(int limitTime) {
		timeLimit.text = limitTime.ToString();
	}
}
