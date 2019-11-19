using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class ShotModule : WeaponModuleBase {

		[SerializeField]
		private ModuleObject _bullet;

		[SerializeField]
		EffectObject _muzzle;

		private Transform _shotAnchor;

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);

			var transforms = weapon.transform.GetComponentsInChildren<Transform>();
			foreach(Transform item in transforms) {
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

			var e = Instantiate(_muzzle, _shotAnchor);
			e.transform.localPosition = new Vector3();
			e.PlayEffect();

			Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ => {
				e.DestroyEffect();
			}).AddTo(this);
		}
	}
}