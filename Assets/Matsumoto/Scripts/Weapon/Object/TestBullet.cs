using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace Matsumoto.Weapon {

	public class TestBullet : ModuleObject {

		public float LifeTime = 10;

		private EffectObject _effect;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		public override WeaponModuleData MagnificationData {
			get; set;
		}

		private void Start() {

			_effect = GetComponentInChildren<EffectObject>();

			Observable.Timer(TimeSpan.FromSeconds(LifeTime)).Subscribe(_ =>
			{
				_effect.DestroyEffect();
				Destroy(gameObject);
			}).AddTo(this);
		}

		private void Update() {
			transform.position += transform.forward * ModuleData.Speed * Time.deltaTime;
		}
	}
}

