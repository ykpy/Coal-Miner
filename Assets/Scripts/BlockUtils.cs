using UnityEngine;
using System.Collections;

public static class BlockUtils {

	public static Vector3 RoundPosition(Vector3 position) {
		position.x = Mathf.Round(position.x);
		position.y = Mathf.Round(position.y);
		position.z = Mathf.Round(position.z);
		return position;
	}
}
