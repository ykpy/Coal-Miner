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

	public static Vector3 GetSurface(RaycastHit hit) {
		if (hit.collider == null) {
			return Vector3.zero;
		}

		var blockPosition = hit.collider.gameObject.transform.position;

		var distance = hit.point - blockPosition;
		var absDistance = new Vector3(Mathf.Abs(distance.x), Mathf.Abs(distance.y), Mathf.Abs(distance.z));

		if (absDistance.x > absDistance.y && absDistance.x > absDistance.z) {
			if (distance.x > 0) {
				blockPosition.x += 1f;
			} else {
				blockPosition.x -= 1f;
			}
		} else if (absDistance.y > absDistance.x && absDistance.y > absDistance.z) {
			if (distance.y > 0) {
				blockPosition.y += 1f;
			} else {
				blockPosition.y -= 1f;
			}
		} else if (absDistance.z > absDistance.x && absDistance.z > absDistance.y) {
			if (distance.z > 0) {
				blockPosition.z += 1f;
			} else {
				blockPosition.z -= 1f;
			}
		}

		return blockPosition;
	}

}
