using UnityEngine;
using System.Collections;
using UniRx.Async;

namespace Matsumoto.Weapon {

	public class WeaponBase : MonoBehaviour, IWeapon {

		[SerializeField]
		protected WeaponData _weaponData;
		public WeaponData WeaponData {
			get { return _weaponData; }
		}

		[SerializeField]
		protected WeaponModuleBase[] _weaponModules;
		public WeaponModuleBase[] WeaponModules {
			get { return _weaponModules; }
		}

		[SerializeField]
		protected WeaponBase _otherWeapon;
		public WeaponBase OtherWeapon {
			get { return _otherWeapon; }
			set { _otherWeapon = value; }
		}

		public virtual void Initialize() { }

		public virtual UniTask Destroy() {
			Destroy(gameObject);
			return new UniTask();
		}

		public virtual GameObject GetBody() {
			return gameObject;
		}

		public virtual Transform GetGrabAnchor() {
			return transform;
		}

		public virtual OVRHapticsClip GetHaptics() {
			return null;
		}

		public virtual void OnButton(OVRInput.Button button) {}

		public virtual void OnButtonDown(OVRInput.Button button) {}

		public virtual void OnButtonNear(OVRInput.NearTouch nearTouch) {}

		public virtual void OnButtonTouch(OVRInput.Touch touch) {}

		public virtual void OnButtonUp(OVRInput.Button button) {}

		public virtual void OnStickAnalogValue(OVRInput.Axis2D type, Vector2 axis) {}

		public virtual void OnTriggerAnalogValue(OVRInput.Axis1D type, float axis) {}

	}
}


