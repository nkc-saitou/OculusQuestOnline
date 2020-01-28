using UnityEngine;
using System.Collections;
using Nakajima.Player;

namespace Matsumoto.Weapon {

	public abstract class ModuleObject : MonoBehaviour {
		
		public PlayerHand Owner {
			get; set;
		}

		public abstract WeaponModuleData ModuleData {
			get; set;
		}

		public virtual ModuleDataModular Modular {
			get;
		} = new ModuleDataModular();

		/// <summary>
		/// 最終攻撃力を取得します
		/// </summary>
		/// <returns>オブジェクトの攻撃力</returns>
		public virtual int GetPower() {
			return ModuleData.Power * Modular.Power;
		}

	}

}

