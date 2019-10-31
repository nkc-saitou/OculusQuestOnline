using UnityEngine;
using System.Collections;

namespace Matsumoto.Weapon {

	public class TestBullet : ModuleObject {

		public float LifeTime = 10;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		private void Start() {
			
		}

		private void Update() {
			transform.position += transform.forward * ModuleData.Speed * Time.deltaTime;
		}
	}
}

