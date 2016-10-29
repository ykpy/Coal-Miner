using UnityEngine;

public class FileButton : MonoBehaviour {

	public FileExplorer explorer;

	public void OnFileClick() {
		var fileName = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text;

		explorer.SelectFile(fileName);
	}
}
