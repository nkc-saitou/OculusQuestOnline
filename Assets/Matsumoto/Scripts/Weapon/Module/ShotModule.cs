using UnityEngine;
using System.Collections;

namespace Matsumoto.Weapon {

	public class ShotModule : WeaponModuleBase {

		[SerializeField]
		private ModuleObject _bullet;

		private Transform _shotAnchor;

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);

			var tranforms = weapon.transform.GetComponentsInChildren<Transform>();
			foreach(Transform item in tranforms) {
				if(item.name == "[ShotAnchor]") {
					_shotAnchor = item;
					return;
				}
			}

			Debug.LogWarning("not found child object. name : [ShotAnchor]");
		}

		public override void OnUseModule(WeaponBase weapon) {
			base.OnUseModule(weapon);

			if(!_shotAnchor) {
				return;
			}

			if(!_bullet) {
				Debug.LogWarning("not seted Bullet.");
				return;
			}

			var b = Instantiate(_bullet, _shotAnchor.position, _shotAnchor.rotation);
			b.ModuleData = _moduleData;
		}
	}
}