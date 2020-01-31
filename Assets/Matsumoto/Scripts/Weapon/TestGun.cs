using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class TestGun : WeaponBase {

		public override void Initialize(float fadeTime) {
			base.Initialize(fadeTime);

			((ShotModule)_weaponModules[0]).ShotSEName = "beamgun1";
		}

		// Update is called once per frame
		void Update() {

			// テストでキーでも動く
			if(Input.GetKeyDown(KeyCode.E)) {
				OnButtonDown(OVRInput.Button.One);
			}

		}

		public override void OnButtonDown(OVRInput.Button button) {
			base.OnButtonDown(button);
			if(!IsUsable) return;

			// 0番目のモジュールを使用
			if(button == OVRInput.Button.One) {
				_weaponModules[0].OnUseModule(this);
			}
		}
	}
}


