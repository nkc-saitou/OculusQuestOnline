using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class TestBow : WeaponBase {

		[SerializeField]
		private Transform _bowBody;

		[SerializeField]
		private Transform _arrow;

		private float _length;
		private float _arrowPosition;

		private bool _isStayArrowPosition;
		private bool _isChargingArrow;

		// Update is called once per frame
		void Update() {

			// テストでキーでも動く
			//if(Input.GetKeyDown(KeyCode.E)) {
			//	OnButtonDown(OVRInput.Button.One);
			//}

			if(Input.GetKeyUp(KeyCode.E)) {
				OnButtonUp(OVRInput.Button.One);
			}

			if(_isChargingArrow) {
				UpdateBowAngle(OtherWeapon.transform.position);
			}

		}

		private void UpdateBowAngle(Vector3 otherHandPosition) {

			var diff = _bowBody.position - otherHandPosition;
			_length = -diff.magnitude;

			_bowBody.forward = diff;

			UpdateArrow(_length);
		}

		private void UpdateArrow(float length) {
			var pos = _arrow.localPosition;
			pos.z = length;
			_arrow.localPosition = pos;
		}

		public override void Initialize() {
			base.Initialize();

			var input = (WeaponOtherInput)_otherWeapon;

			_arrowPosition = _arrow.localPosition.z;

			input.OnButtonDownRecieved
				.Where(_ => _isStayArrowPosition)
				.Subscribe(button => {
					if(button != OVRInput.Button.One) return;

					_isChargingArrow = true;
				})
				.AddTo(this);

			input.OnButtonUpRecieved
				.Subscribe(button => {
					if(button != OVRInput.Button.One) return;

					_weaponModules[0].OnUseModule(this);
					_isChargingArrow = false;

					// 矢を元の位置に戻す
					UpdateArrow(_arrowPosition);
				})
				.AddTo(this);

			input.OnTriggerEnterRecieved
				.Subscribe(OnTriggerEnter)
				.AddTo(this);

			input.OnTriggerExitRecieved
				.Subscribe(OnTriggerExit)
				.AddTo(this);
		}

		public override void OnButtonDown(OVRInput.Button button) {
			base.OnButtonDown(button);


			// 0番目のモジュールを使用
			if(button == OVRInput.Button.One) {
				_weaponModules[0].OnUseModule(this);
			}
		}

		private void OnTriggerEnter(Collider other) {
			_isStayArrowPosition = true;
		}

		private void OnTriggerExit(Collider other) {
			_isStayArrowPosition = false;
		}

		[ContextMenu("Switch")]
		private void Switch() {
			_isChargingArrow = !_isChargingArrow;
		}
	}
}


