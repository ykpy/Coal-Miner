public enum Block {
	Empty,
	Ground,
	Breakable,
	Unbreakable,
}

public static class BlockExtensions {
	public static int ToInt(this Block self) {
		return (int) self;
	}
}

