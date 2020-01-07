 using UnityEngine;
using System.Collections;

namespace Matsumoto.Weapon {

	public class WeaponModuleBase : MonoBehaviour {

		[SerializeField]
		protected WeaponModuleData _moduleData;

		public virtual void ModuleInitialize(WeaponBase weapon) { }

		public virtual void ModuleFinalize(WeaponBase weapon) { }

		public virtual void OnUseModule(WeaponBase weapon) { }



	}
}