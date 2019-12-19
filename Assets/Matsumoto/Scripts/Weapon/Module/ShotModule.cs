using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class ShotModule : WeaponModuleBase {

		private const int EventID = 1000;

		[SerializeField]
		private ModuleObject _bullet;

		[SerializeField]
		EffectObject _muzzle;

		private Transform _shotAnchor;
		private NetworkEventManager manager;

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);


			var playerID = 1; // 1 or 2
			manager.AddSyncEvent(playerID, EventID, (data) => {
				var t = (TransformStamp)data;
				var b = Instantiate(_bullet, t.Position, t.Rotation);
				b.ModuleData = _moduleData;
			});

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

			//var b = Instantiate(_bullet, _shotAnchor.position, _shotAnchor.rotation);
			//b.ModuleData = _moduleData;
			manager.CallSyncEvent(EventID, new TransformStamp(System.DateTime.Now, _shotAnchor.position, _shotAnchor.rotation));

			var e = Instantiate(_muzzle, _shotAnchor);
			e.transform.localPosition = new Vector3();
			e.PlayEffect();

			Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ => {
				e.DestroyEffect();
			}).AddTo(this);
		}
	}
}