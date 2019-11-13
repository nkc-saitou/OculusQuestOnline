using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class TestBomb : WeaponBase {

		// Update is called once per frame
		void Update() {

			// テストでキーでも動く
			if(Input.GetKeyUp(KeyCode.E)) {
				OnButtonUp(OVRInput.Button.One);
			}

		}

		public override void OnButtonUp(OVRInput.Button button) {
			base.OnButtonUp(button);

			// 0番目のモジュールを使用
			if(button == OVRInput.Button.One) {
				_weaponModules[0].OnUseModule(this);
			}
		}
	}
}


