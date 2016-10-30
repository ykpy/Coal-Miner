public enum Block {
	Empty,
	Ground,
	Breakable,
	Unbreakable,
	Start,
	Goal,
}

public static class BlockExtensions {
	public static int ToInt(this Block self) {
		return (int) self;
	}
}

