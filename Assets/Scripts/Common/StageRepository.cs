using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class StageRepository {

	List<Stage> cachedStages = null;

	public StageRepository() {
		cachedStages = LoadStageData();
	}

	/// <summary>
	/// ステージデータを全て取得します
	/// </summary>
	/// <returns>ステージデータ</returns>
	public List<Stage> findAll() {
		return cachedStages ?? (cachedStages = LoadStageData());
	}

	/// <summary>
	/// ステージ名からステージを検索します
	/// </summary>
	/// <param name="stageName">ステージ名</param>
	/// <returns>合致するステージ</returns>
	public Stage FindByName(string stageName) {
		if (cachedStages == null)
			cachedStages = LoadStageData();

		return cachedStages.Where(stage => stage.StageName == stageName).First();
	}

	public List<Stage> FindByLimitAndOffset(int limit, int offset) {
		return cachedStages.Select((s, i) => new { s, i })
			.Where(s => s.i >= offset && s.i < offset + limit)
			.Select(s => s.s)
			.ToList();
	}

	public bool HasNext(int limit, int offset) {
		return offset < cachedStages.Count && (offset + limit) < cachedStages.Count;
	}

	public bool HasPrev(int limit, int offset) {
		return offset >= 0;
	}

	/// <summary>
	/// ステージファイルからステージデータを全て読み込みます
	/// </summary>
	/// <returns></returns>
	List<Stage> LoadStageData() {
		var directory = new DirectoryInfo(Stage.StageDataDirectoryPath);
		var files = directory.GetFiles("*.dat", SearchOption.TopDirectoryOnly);

		List<Stage> stages = new List<Stage>();
		foreach (var file in files) {
			using (var reader = file.OpenText()) {
				var data = reader.ReadToEnd();
				stages.Add(new Stage(data));
			}
		}

		return stages;
	}
}
