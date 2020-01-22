using UnityEngine;
using UnityEditor;

namespace Matsumoto.Weapon {

	[CreateAssetMenu(menuName = "Create/Create WeaponModuleData")]
	public class WeaponModuleData : ScriptableObject {

		public string ModuleName;
		public int GuardPower;

		public int Power;
		public float Speed;
		public float Size = 1;
		public float UseWaitSec;
		public bool IsAutoUse;

	}
}