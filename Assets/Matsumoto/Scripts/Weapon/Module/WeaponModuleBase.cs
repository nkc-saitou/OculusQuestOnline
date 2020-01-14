 using UnityEngine;
using System.Collections;
using UniRx;
using Nakajima.Player;

namespace Matsumoto.Weapon {

	public class WeaponModuleBase : MonoBehaviour {

        public ReactiveProperty<PlayerHand> Owner
        {
            get;
        } = new ReactiveProperty<PlayerHand>();

        [SerializeField]
		protected WeaponModuleData _moduleData;

		public virtual void ModuleInitialize(WeaponBase weapon) { }

		public virtual void ModuleFinalize(WeaponBase weapon) { }

		public virtual void OnUseModule(WeaponBase weapon) { }



	}
}