﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Matsumoto.Weapon {

	public class WeaponManager : MonoBehaviour {

		public const string WeaponPrefabPath = "Weapon/Prefabs/";

		private Dictionary<string, WeaponBase> _weaponBaseDictionary;

		public void Start() {


		}

		[ContextMenu("LoadWeapon")]
		public void LoadWeapon() {
			_weaponBaseDictionary = Resources.LoadAll<WeaponBase>(WeaponPrefabPath)
				.ToDictionary(item => item.name, item => item);
		}

		/// <summary>
		/// 読み込んでいるすべての武器の名前を取得する
		/// </summary>
		/// <returns></returns>
		public string[] GetWeaponNameAll() {
			return _weaponBaseDictionary
				.Select(item => item.Key)
				.ToArray();
		}

		private WeaponBase CreateWeapon(WeaponBase weaponBase) {

			// 武器生成
			var weapon = Instantiate(weaponBase);

			// モジュール生成
			for(int i = 0;i < weapon.WeaponModules.Length;i++) {
				ref var arrayPos = ref weapon.WeaponModules[i];
				var module = Instantiate(arrayPos, weapon.transform);
				module.ModuleInitialize(weapon);
				arrayPos = module;
			}

			return weapon;
		}

		/// <summary>
		/// 両手武器か判断する
		/// </summary>
		/// <param name="name">武器の名前</param>
		/// <returns>両手武器ならtrue</returns>
		public bool HasOtherWeapon(string name) {

			if(!_weaponBaseDictionary.ContainsKey(name)) {
				return false;
			}

			var weapon = _weaponBaseDictionary[name];
			return weapon.OtherWeapon;
		}

        /// <summary>
        /// 武器を生成する
        /// </summary>
        /// <param name="name">武器の名前</param>
        /// <param name="fadeTime">出現までの時間</param>
        /// <returns>武器のインターフェース1~2個(利き手用、その他(あれば、なければ入らない))</returns>
        public IWeapon[] CreateWeapon(string name, float fadeTime) {

			if(!_weaponBaseDictionary.ContainsKey(name)) {
				return null;
			}

			var weaponArray = new List<WeaponBase>();

			var weapon = CreateWeapon(_weaponBaseDictionary[name]);
			weaponArray.Add(weapon);

			if(!weapon.OtherWeapon) {
			    weapon.Initialize(fadeTime);
				return weaponArray.ToArray();
			}

			// 両手武器用
			var otherWeapon = CreateWeapon(weapon.OtherWeapon);
			weapon.OtherWeapon = otherWeapon;
			weaponArray.Add(otherWeapon);

            weapon.Initialize(fadeTime);
            otherWeapon.Initialize(fadeTime);

            return weaponArray.ToArray();
		}

		[ContextMenu("OverrideData")]
		public void OverrideData() {
			WeaponDataDownloader.OverrideDataAll().Forget();
		}
	}


}

