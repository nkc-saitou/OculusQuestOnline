using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matsumoto.GoogleSpreadSheetLoader;
using UniRx.Async;

namespace Matsumoto.Weapon {

	public static class WeaponDataDownloader {

		public const string WeaponDataPath = "Weapon/Data/";
		public const string WeaponModuleDataPath = "Weapon/ModuleData/";

		private static WeaponData[] _weaponDataArray;
		private static WeaponModuleData[] _weaponModuleDataArray;

		static WeaponDataDownloader() {}

		public static void Initialize() {
			_weaponDataArray = Resources.LoadAll<WeaponData>(WeaponDataPath);
			_weaponModuleDataArray = Resources.LoadAll<WeaponModuleData>(WeaponModuleDataPath);
		}

		public static async UniTask OverrideDataAll() {

			var sheet = await SpreadSheetLoader.LoadSheet(DefinedData.ApiKey, DefinedData.SheetID, "WeaponDataOverride");
			for(int i = 0;i < sheet.Values.Count;i++) {
				var str = "";
				for(int j = 0;j < sheet.Values[i].Count;j++) {
					str += sheet.Values[i][j] + ",";
				}
				Debug.Log(str);
			}
		}
	}
}
