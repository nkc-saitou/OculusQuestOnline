using UnityEngine;
using UnityEditor;

namespace Matsumoto.Weapon {

	[CreateAssetMenu(menuName = "Create/Create WeaponData")]
	public class WeaponData : ScriptableObject {

		public string DisplayName;
		public int GuardPower;

		public WeaponModuleData[] ModuleDataArray;

	}
}
