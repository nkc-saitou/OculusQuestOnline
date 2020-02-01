using UnityEngine;
using System.Collections;
using UniRx;
using System;
using Nakajima.Player;

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
            if (collision.collider.GetComponent<ModuleObject>()) return;
            if (collision.collider.GetComponent<WeaponBase>()) return;

            var player = collision.collider.GetComponent<DisplayPlayerProvider>();
            if (player.MyID == Owner.GetMyProvider.MyID) return;

			OnHit();
		}

		private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<ModuleObject>()) return;
            if (other.GetComponent<WeaponBase>()) return;

            // プレイヤー(自分だけ)は判定から除外
            // 自分サイド
            var player = other.gameObject.GetComponent<PlayerMaster>();
            if (player != null)
            {
                if (player.ID == Owner.GetMyProvider.MyID) return;
            }
            // 敵サイド
            var enemy = other.gameObject.GetComponent<DisplayPlayerProvider>();
            if (enemy != null)
            {
                if (enemy.MyID == Owner.GetMyProvider.MyID) return;
            }

            // 自分の手も判定外
            var hand = other.gameObject.GetComponent<HandMaster>();
            if (hand != null)
            {
                if (hand.GetMyProvider.MyID == Owner.GetMyProvider.MyID) return;
            }

            OnHit();
		}
	}
}

