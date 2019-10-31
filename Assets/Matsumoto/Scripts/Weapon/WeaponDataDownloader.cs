using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Matsumoto.GoogleSpreadSheetLoader;
using UniRx.Async;

namespace Matsumoto.Weapon {

	public static class WeaponDataDownloader {

		public const string WeaponDataPath = "Weapon/Data/";
		public const string WeaponModuleDataPath = "Weapon/Module/Data/";

		private static WeaponData[] _weaponDataArray;
		private static WeaponModuleData[] _weaponModuleDataArray;

		static WeaponDataDownloader() {
			Initialize();
		}

		public static void Initialize() {
			_weaponDataArray = Resources.LoadAll<WeaponData>(WeaponDataPath);
			_weaponModuleDataArray = Resources.LoadAll<WeaponModuleData>(WeaponModuleDataPath);
		}

		public static async UniTask OverrideDataAll() {

			await OverrideWeaponData();
			await OverrideWeaponModuleData();
		}

		public static async UniTask OverrideWeaponData() {

			var sheet = await SpreadSheetLoader.LoadSheet(DefinedData.ApiKey, DefinedData.SheetID, "WeaponDataOverride");
			for(int i = 1;i < sheet.Values.Count;i++) {

				var row = sheet.Values[i];

				var data = _weaponDataArray.FirstOrDefault(item => item.name == row[0]);
				if(!data) {
					continue;
				}

				if(row[1] == "") {
					continue;
				}
				while(row.Count < 4) {
					row.Add("");
				}
				SetData(row[2], ref data.DisplayName);
				SetData(row[3], ref data.GuardPower);
			}
		}

		public static async UniTask OverrideWeaponModuleData() {

			var sheet = await SpreadSheetLoader.LoadSheet(DefinedData.ApiKey, DefinedData.SheetID, "WeaponModuleDataOverride");
			for(int i = 1;i < sheet.Values.Count;i++) {

				var row = sheet.Values[i];

				var data = _weaponModuleDataArray.FirstOrDefault(item => item.name == row[0]);
				if(!data) {
					continue;
				}

				if(row[1] == "") {
					continue;
				}

				while(row.Count < 9) {
					row.Add("");
				}
				SetData(row[2], ref data.ModuleName);
				SetData(row[3], ref data.GuardPower);
				SetData(row[4], ref data.Power);
				SetData(row[5], ref data.Speed);
				SetData(row[6], ref data.Size);
				SetData(row[7], ref data.UseWaitSec);
				SetData(row[8], ref data.IsAutoUse);
			}
		}

		private static void SetData<T>(string value, ref T data) {
			if(value == "") {
				return;
			}

			data = (T)Convert.ChangeType(value, typeof(T));
		}
	}
}
