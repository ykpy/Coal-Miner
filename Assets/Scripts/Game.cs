public class Game {
	public bool isStarted;
	public float limitTime;
	public uint limitCreate;
	public uint limitBreak;
	public int coinCount;
	public readonly int coinMax;
	public bool isPinch;

	public bool IsTimeOver {
		get { return limitTime <= 0f; }
	}

	public Game(Stage stage) {
		isStarted = false;
		limitTime = stage.timeLimit;
		limitCreate = stage.createTime;
		limitBreak = stage.breakTime;
		coinCount = 0;
		coinMax = stage.GetCoinCount();
		isPinch = false;
	}

	public void StartGame() {
		isStarted = true;
	}

	public void FinishGame() {
		isStarted = false;
	}
}
