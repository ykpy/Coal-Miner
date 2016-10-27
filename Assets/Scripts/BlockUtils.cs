using UnityEngine;
using System.Collections;

public static class BlockUtils {

	public static Vector3 RoundPosition(Vector3 position) {
		position.x = Mathf.Round(position.x);
		position.y = Mathf.Round(position.y);
		position.z = Mathf.Round(position.z);
		return position;
	}

	public static Texture GetTextureFromMaterial(Material material) {
		return material.GetTexture("_MainTex");
	}

	public static Material GetMaterial(this GameObject obj) {
		return obj.GetComponent<Renderer>().sharedMaterial;
	}
}
