using UnityEngine;

namespace Matsumoto.Weapon.Test {
	public class TestWeaponCreator : MonoBehaviour{

		public WeaponManager Manager;
		public string CreateWeaponName;

		[ContextMenu("CreateWeapon")]
		public void Create() {
			Manager.CreateWeapon(CreateWeaponName);
		}

	}
}
