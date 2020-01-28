using UnityEngine;
using System.Collections;
using UniRx;

namespace Matsumoto.Weapon {

	public class TestCrossBow : WeaponBase {

		[SerializeField]
		private Transform _bowBody;

		[SerializeField]
		private Transform _arrow;

		[SerializeField]
		private GameObject _arrowBody;

		[SerializeField]
		private Collider _arrowCollider;

		[SerializeField]
		private Collider _gripCollider;

		[SerializeField]
		private float _maxLength;

		private float _length;
		private float _arrowPosition;

		private bool _isSettedArrow;
		private bool _isStayArrowPosition;
		private Vector3 _handArrowOffset;
		private bool _isStayGripPosition;
		private Quaternion _handGripOffset;
		private bool _hasGrip;
		private bool _hasArrow;

		// Update is called once per frame
		void Update() {

			// テストでキーでも動く
			//if(Input.GetKeyDown(KeyCode.E)) {
			//	OnButtonDown(OVRInput.Button.One);
			//}

			if(Input.GetKeyDown(KeyCode.E)) {
				OnButtonDown(OVRInput.Button.One);
			}

			if(_hasGrip) {
				//var pos = OtherWeapon.transform.position - _handGripOffset;
				//var diff = pos - _bowBody.position;
				//var rad = Vector3.Angle(diff, _bowBody.forward) * Mathf.Deg2Rad;
				//var p = _bowBody.forward * Mathf.Cos(rad) * diff.magnitude;
				//_bowBody.forward = pos - (diff - p);

				_bowBody.forward = OtherWeapon.transform.position - _bowBody.position;
				_bowBody.rotation *= _handGripOffset;
			}

			if(_hasArrow) {

				var pos = OtherWeapon.transform.position - _handArrowOffset;
				var diff = pos - _bowBody.position;
				var rad = Vector3.Angle(diff, _bowBody.forward) * Mathf.Deg2Rad;

				_length = Mathf.Cos(rad) * diff.magnitude * _bowBody.localScale.x;
				if(_length <= _maxLength) {
					_hasArrow = false;
					_isSettedArrow = true;
					Debug.Log("setted");
				}
				UpdateArrow(_length);
			}

		}

		private void UpdateArrow(float length) {
			var pos = _arrow.localPosition;
			pos.z = length;
			_arrow.localPosition = pos;
		}

		public override void Initialize(float fadeTime) {
			base.Initialize(fadeTime);

			var input = (WeaponOtherInput)_otherWeapon;

			_arrowPosition = _arrow.localPosition.z;

			input.OnButtonDownRecieved
				.Subscribe(button => {
					if(button != OVRInput.Button.One) return;
					if(!IsUsable) return;

					if(_isStayGripPosition) {
						//_handGripOffset = OtherWeapon.transform.position - _gripCollider.transform.position;
						_handGripOffset = Quaternion.FromToRotation(OtherWeapon.transform.position - _bowBody.position, _bowBody.forward);
						_hasGrip = true;
					}
					if(_isStayArrowPosition) {
						_handArrowOffset = OtherWeapon.transform.position - _arrowCollider.transform.position;
						_hasArrow = true;
					}

				})
				.AddTo(this);

			input.OnButtonUpRecieved
				.Subscribe(button => {
					if(button != OVRInput.Button.One) return;
					if(!IsUsable) return;

					if(_hasGrip) {
						_hasGrip = false;
					}
					if(_hasArrow) {
						_arrowBody.SetActive(false);
						_hasArrow = false;

						// 矢を元の位置に戻す
						UpdateArrow(_arrowPosition);
					}
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
			if(!IsUsable) return;

			if(!_isSettedArrow) {
				return;
			}

			// 0番目のモジュールを使用
			if(button == OVRInput.Button.One) {
				_weaponModules[0].OnUseModule(this);

				_isSettedArrow = false;
				_arrowBody.SetActive(false);

				// 矢を元の位置に戻す
				UpdateArrow(_arrowPosition);
			}

		}

		private void OnTriggerEnter(Collider other) {
			if(!IsUsable) return;

			if(other == _arrowCollider) {
				_isStayArrowPosition = true;
				_isStayGripPosition = false;
				_arrowBody.SetActive(true);
			}
			else if(other == _gripCollider) {
				_isStayGripPosition = true;
			}
		}

		private void OnTriggerExit(Collider other) {
			if(!IsUsable) return;

			if(other == _arrowCollider) {
				_isStayArrowPosition = false;
				if(!_hasArrow && !_isSettedArrow)
					_arrowBody.SetActive(false);
			}
			else if(other == _gripCollider) {
				_isStayGripPosition = false;
			}
		}

		[ContextMenu("Switch")]
		private void Switch() {
			_hasGrip = !_hasGrip;
		}
	}
}


