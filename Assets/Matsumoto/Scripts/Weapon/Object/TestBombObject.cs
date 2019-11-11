using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace Matsumoto.Weapon {

	public class TestBombObject : ModuleObject {

		public float LifeTime = 10;

		[SerializeField]
		private Rigidbody _rigidbody;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		public override WeaponModuleData MagnificationData {
			get; set;
		}

		private void Start() {

			Destroy(gameObject, LifeTime);
			_rigidbody.AddForce(transform.forward * ModuleData.Speed * MagnificationData.Speed);
		}

		private void Update() {
			
		}
	}
}

