using UnityEngine;
using System.Collections;
using UniRx;
using Nakajima.Player;

namespace Matsumoto.Weapon {

	public class ShotModule : WeaponModuleBase {

		[SerializeField]
		private ModuleObject _bullet;

		[SerializeField]
		EffectObject _muzzle;

        private static int _createCount = 0;
        private int _myID = _createCount++;

        private Transform _shotAnchor;
		private NetworkEventManager manager;

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);

            Owner.Subscribe(item =>
            {
                if (!item) return;
                var playerID = item.myProvider.MyID;
                manager = FindObjectOfType<NetworkEventManager>();
                manager.AddSyncEvent(playerID, "ShotModule_Shot" + _myID, (data) => {
                    var p = (Vector3)(data[0]);
                    var r = (Quaternion)(data[1]);
                    var b = Instantiate(_bullet, p, r);
                    b.ModuleData = _moduleData;
                });
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
			manager.CallSyncEvent("ShotModule_Shot" + _myID, new object[] { _shotAnchor.position, _shotAnchor.rotation });

			var e = Instantiate(_muzzle, _shotAnchor);
			e.transform.localPosition = new Vector3();
			e.PlayEffect();

			Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ => {
				e.DestroyEffect();
			}).AddTo(this);
		}
	}
}