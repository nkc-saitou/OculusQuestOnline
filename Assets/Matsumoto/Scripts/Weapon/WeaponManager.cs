using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Matsumoto.GoogleSpreadSheetLoader;
using UniRx.Async;

namespace Matsumoto.Weapon {

	public class WeaponManager : MonoBehaviour {

		public const string WeaponDataPath = "Weapon/";

		public async UniTask Start() {

			await OverrideDataAll();

		}

		public async UniTask OverrideDataAll() {

			var sheet = await SpreadSheetLoader.LoadSheet(DefinedData.ApiKey, DefinedData.SheetID, "WeaponDataOverride");
			for(int i = 0;i < sheet.Values.Count;i++) {
				var str = "";
				for(int j = 0;j < sheet.Values[i].Count;j++) {
					str += sheet.Values[i][j] + ",";
				}
				Debug.Log(str);
			}
		}

		public IWeapon CreateWeapon() {
			return null;
		}

	}


}

