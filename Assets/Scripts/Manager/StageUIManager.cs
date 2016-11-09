using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : BaseStageUIManager {

	// 残り時間UI
	[SerializeField]
	Text timeLimit;

	[SerializeField]
	Text createLimit;

	[SerializeField]
	Text breakLimit;

	[SerializeField]
	Text coin;

	/// <summary>
	/// 残り時間を画面に表示します
	/// </summary>
	/// <param name="limitTime">残り時間</param>
	public void ShowTimeLimit(int limitTime) {
		timeLimit.text = limitTime.ToString();
	}

	public void ShowCreateLimit(uint createLimitNum) {
		createLimit.text = createLimitNum.ToString();
	}

	public void ShowBreakLimit(uint breakLimitNum) {
		breakLimit.text = breakLimitNum.ToString();
	}

	public override void ShowStageInformation(Stage stage) {
		ShowTimeLimit(stage.timeLimit);
	}

	public void ShowCoinInformation(int count, int max) {
		coin.text = string.Format("{0} / {1}", count, max);
	}

	public void ShowGameInformation(Game game) {
		ShowTimeLimit(Mathf.CeilToInt(game.limitTime));
		ShowCreateLimit(game.limitCreate);
		ShowBreakLimit(game.limitBreak);
		ShowCoinInformation(game.coinCount, game.coinMax);
	}

}
