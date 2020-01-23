using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace Matsumoto.Weapon {

	public class TestBullet : ModuleObject {

		public float LifeTime = 10;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private EffectObject _hitEffect;


		private EffectObject _effect;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		private void Start() {

			_effect = GetComponentInChildren<EffectObject>();

			Observable.Timer(TimeSpan.FromSeconds(LifeTime)).Subscribe(_ =>
			{
                if(_effect)
				    _effect.DestroyEffect();
				Destroy(gameObject);
			}).AddTo(this);
		}

		private void Update() {
			var pos = transform.position + transform.forward * ModuleData.Speed * Time.deltaTime;
			_rigidbody.MovePosition(pos);
		}

		private void OnHit() {

			if(_effect)
				_effect.DestroyEffect();
			var e = Instantiate(_hitEffect, transform.position, transform.rotation);
			e.Invoke(nameof(e.DestroyEffect), 3);
			Destroy(gameObject);

			Audio.AudioManager.PlaySE("beamgun2", position: transform.position);

		}

		private void OnCollisionEnter(Collision collision) {
			OnHit();
		}

		private void OnTriggerEnter(Collider other) {
			OnHit();
		}
	}
}

