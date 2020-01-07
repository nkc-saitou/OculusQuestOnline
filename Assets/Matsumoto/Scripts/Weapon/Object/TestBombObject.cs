using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace Matsumoto.Weapon {

	public class TestBombObject : ModuleObject {

		public float LifeTime = 10;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private ParticleSystem _explosionEffect;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		private void Start() {

			Destroy(gameObject, LifeTime);
			_rigidbody.AddForce(transform.forward * ModuleData.Speed * Modular.Speed);

		}

		private void OnDestroy() {

			var e = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
			Destroy(e.gameObject, 10);
		}

		private void Update() {
			
		}
	}
}

