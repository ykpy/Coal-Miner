using UnityEngine;
using System.Collections;

public static class MessageUtils {
	public static string FormatXYZ(params object[] args) {
		return string.Format("X: {0}  Y: {1}  Z: {2}", args);
	}
}
