using UnityEngine;
using System.Collections;

public static class MessageUtils {
	public static string FormatXYZ(params object[] args) {
		return string.Format("X: {0}  Y: {1}  Z: {2}", args);
	}

	/// <summary>
	/// ファイル名に拡張子をつけます
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	/// <param name="extension">ドットつき拡張子（.txt, .mp3）</param>
	/// <returns></returns>
	public static string AddFileExtension(this string fileName, string extension) {
		if (fileName.EndsWith(extension))
			return fileName;
		return fileName + extension;
	}

	public const string StageNameNullErrorMessage = "ステージ名が入力されていません。";

	public const string StageDataSaveSuccessMessage = "ステージデータを保存しました。";

	public const string StageDataLoadSuccessMessage = "ステージデータを読み込みました。";
}
