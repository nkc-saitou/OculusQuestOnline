using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;
using System.Collections.Generic;
using UniRx.Async;

namespace Matsumoto.GoogleSpreadSheetLoader {

	public enum SpreadSheetLoadStatus {
		Complete,
		HttpError,
		NetWorkError,
	}

	/// <summary>
	/// GoogleSpradSheetの内容を読み込む
	/// </summary>
	public class SpreadSheetLoader {

		public static async UniTask<SpreadSheet> LoadSheet(string apiKey, string sheetId, string range) {
			var url = "https://sheets.googleapis.com/v4/spreadsheets/" + sheetId + "/values/" + range + "?key=" + apiKey;
			return await LoadCoroutine(url);
		}

		private static async UniTask<SpreadSheet> LoadCoroutine(string url) {
			Debug.Log("StartRequest : " + url);
			using(var request = UnityWebRequest.Get(url)) {
				var result = await request.SendWebRequest();

				if(request.isHttpError) {
					Debug.Log(SpreadSheetLoadStatus.HttpError);
					Debug.Log(request.downloadHandler.text);
					return null;
				}
				if(request.isNetworkError) {
					Debug.Log(SpreadSheetLoadStatus.NetWorkError);
					Debug.Log(request.downloadHandler.text);
					return null;
				}

				Debug.Log(SpreadSheetLoadStatus.Complete);

				var json = request.downloadHandler.text; // Googleスプレッドシートから取得してきたJSON
				Debug.Log(json);

				var dict = (IDictionary)MiniJSON.Json.Deserialize(json);
				var sheet = new SpreadSheet();
				sheet.Range = (string)dict["range"];
				sheet.MajorDimension = (string)dict["majorDimension"];
				sheet.Values = ((List<object>)dict["values"])
					.Select(item => ((List<object>)item)
						.Select(item2 => item2 as string)
						.ToList())
					.ToList();
					
				return sheet;
			}
		}
	}

}
