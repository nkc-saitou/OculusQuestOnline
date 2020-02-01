using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace Matsumoto.Weapon {

	public class TestBombObject : ModuleObject {

		public float LifeTime = 10;
		public float ExplosionStart = 0.5f;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private Collider _explosiveCollision;

		[SerializeField]
		private GameObject _explosionEffect;

		[SerializeField]
		private GameObject _body;

		private float _timer = 0.0f;

		public override WeaponModuleData ModuleData {
			get; set;
		}

		private void Start() {
			_explosiveCollision.enabled = false;
			_rigidbody.AddForce(transform.forward * ModuleData.Speed * Modular.Speed);
		}

		private void Update() {
			
			if((_timer += Time.deltaTime) > LifeTime) {
				Explosion();
			}

		}

		private void Explosion() {

			_explosiveCollision.enabled = true;

			// effect
			var e = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
			Destroy(e.gameObject, 10);

			_body.SetActive(false);
			Destroy(gameObject, 0.2f);

			Audio.AudioManager.PlaySE("bomb1", position: transform.position);
			
		}

		private void OnCollisionEnter(Collision collision) {
            // プレイヤーは判定から除外
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) return;

			Explosion();
		}
	}
}

