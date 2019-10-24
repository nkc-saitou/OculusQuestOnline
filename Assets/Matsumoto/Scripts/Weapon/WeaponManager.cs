using UnityEngine;
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

		/// <summary>
		/// 武器を生成する
		/// </summary>
		/// <param name="name">武器の名前</param>
		/// <returns>武器のインターフェース</returns>
		public IWeapon CreateWeapon(string name) {
			return null;
		}

	}


}

