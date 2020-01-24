using UnityEngine;
using System.Collections;
using UniRx.Async;
using UniRx;
using UniRx.Triggers;

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

		private Renderer[] _rendererArray;

		public bool IsUsable {
			get; protected set;
		} = false;


		public virtual void Initialize(float fadeTime) {

			_rendererArray = GetComponentsInChildren<Renderer>();

			// 再生成
			for(int i = 0;i < _rendererArray.Length;i++) {
				var r = _rendererArray[i];
				r.material = new Material(r.material);
				r.material.SetFloat("_Value", 0.0f);
			}

			StartCoroutine(SpawnAnim(fadeTime));
		}

		public virtual UniTask Destroy(float fadeTime) {

			StartCoroutine(DestroyAnim(fadeTime));
			Destroy(gameObject);
			return new UniTask();
		}

		public virtual GameObject GetBody() {
			return gameObject;
		}

		public virtual Transform GetGrabAnchor() {
			return transform;
		}

        public virtual void SetOwner(Nakajima.Player.PlayerHand owner)
        {
            foreach (var item in WeaponModules)
            {
                item.Owner.Value = owner;
            }
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

		private IEnumerator SpawnAnim(float fadeTime) {

			var rn = 1 / fadeTime;
			var t = 0.0f;

			Debug.Log("fadeOut");

			while(t < 1.0f) {

				t = Mathf.Min(1.0f, t + rn * Time.deltaTime);

				for(int i = 0;i < _rendererArray.Length;i++) {
					var m = _rendererArray[i].material;
					m.SetFloat("_Value", t);
				}
				Debug.Log("fadeOut:" + t);

				yield return null;

			}

			IsUsable = true;

		}

		private IEnumerator DestroyAnim(float fadeTime) {

			var rn = 1 / fadeTime;
			var t = 1.0f;
			IsUsable = false;

			Debug.Log("fadeOut");

			while(t > 0.0f) {

				t = Mathf.Max(0.0f, t - rn * Time.deltaTime);

				for(int i = 0;i < _rendererArray.Length;i++) {
					var m = _rendererArray[i].material;
					m.SetFloat("_Value", t);
				}

				yield return null;

			}


		}
	}
}


