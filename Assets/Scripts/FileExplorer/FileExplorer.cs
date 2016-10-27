using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileExplorer : MonoBehaviour {

	List<FileInfo> files;
	public ScrollRect explorer;
	public RectTransform viewport;
	public RectTransform content;
	public Button button;

	public Vector2 padding = new Vector2(0, 10);

	Vector2 offset = new Vector2(0.1f, 1f);

	void Start() {
		OpenDirectory(Stage.StageDataDirectoryPath);
	}

	public void InitializeViewer() {
		foreach (var obj in content.GetComponentsInChildren<RectTransform>()) {
			if (obj.gameObject == content.gameObject)
				continue;
			Destroy(obj.gameObject);
		}
	}

	public void OpenDirectory(string directoryPath) {
		InitializeViewer();

		if (!Directory.Exists(directoryPath))
			return;

		var directory = new DirectoryInfo(directoryPath);
		files = directory.GetFiles().ToList();

		var contentRect = content.rect;

		contentRect.height = (button.GetComponent<RectTransform>().rect.height + padding.y) * files.Count;

		content.sizeDelta = Vector2.zero;
		content.sizeDelta = new Vector2(contentRect.width, contentRect.height);

		Vector2 position = Vector2.zero + new Vector2(0, content.rect.height / 2 + button.GetComponent<RectTransform>().rect.height / 2);
		foreach (var file in files) {
			var _button = Instantiate(button);
			var rect = _button.GetComponent<RectTransform>();
			rect.SetParent(content);
			position -= (new Vector2(0, rect.rect.height) + padding);

			rect.anchoredPosition = position;
			_button.transform.GetChild(0).GetComponent<Text>().text = file.Name;

			var fileButton = _button.GetComponent<FileButton>();
			fileButton.explorer = this;
		}
	}

	public void SelectFile(string fileName) {
		StageManager.Instance.LoadStage(fileName);
	}
}
