using UnityEngine;

namespace Matsumoto.Weapon.Test {
	public class TestWeaponCreator : MonoBehaviour{

		public WeaponManager Manager;
		public string CreateWeaponName;

		public IWeapon[] CreatedWeapon;

		[ContextMenu("CreateWeapon")]
		public void Create() {
			CreatedWeapon = Manager.CreateWeapon(CreateWeaponName);
		}

		[ContextMenu("DeleteWeapon")]
		public void Delete() {
			for(int i = 0;i < CreatedWeapon.Length;i++) {
				CreatedWeapon[i].Destroy(1.0f);
			}
		}

	}
}
