using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class TestGun : WeaponBase {

		// Update is called once per frame
		void Update() {

			// テストでキーでも動く
			if(Input.GetKeyDown(KeyCode.E)) {
				OnButtonDown(OVRInput.Button.One);
			}

		}

		public override void OnButtonDown(OVRInput.Button button) {
			base.OnButtonDown(button);


			// 0番目のモジュールを使用
			if(button == OVRInput.Button.One) {
				_weaponModules[0].OnUseModule(this);
			}
		}
	}
}


