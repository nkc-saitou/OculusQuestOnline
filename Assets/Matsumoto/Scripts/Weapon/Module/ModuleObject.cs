using UnityEngine;
using System.Collections;

namespace Matsumoto.Weapon {

	public abstract class ModuleObject : MonoBehaviour {

		public abstract WeaponModuleData ModuleData {
			get; set;
		}

		public abstract WeaponModuleData MagnificationData {
			get; set;
		}

	}

}

