using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Matsumoto.Weapon {

	public class ThrowModule : WeaponModuleBase {

		[SerializeField]
		private ModuleObject _bomb;

		[SerializeField]
		private int _sampleCount;

		private Transform _throwAnchor;
		private Vector3 _prevPosition;
		private List<Vector3> _samples = new List<Vector3>();

		private void Update() {

			// Sample Vec
			_samples.Add(transform.position - _prevPosition);

			if(_samples.Count > _sampleCount) {
				_samples.RemoveAt(0);
			}

		}

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);

			var transforms = weapon.transform.GetComponentsInChildren<Transform>();
			foreach(Transform item in transforms) {
				if(item.name == "[ShotAnchor]") {
					_throwAnchor = item;
					return;
				}
			}

			_prevPosition = transform.position;
			_samples = new List<Vector3>();

			Debug.LogWarning("not found child object. name : [ShotAnchor]");
		}

		public override void OnUseModule(WeaponBase weapon) {
			base.OnUseModule(weapon);

			if(!_throwAnchor) {
				return;
			}

			if(!_bomb) {
				Debug.LogWarning("not seted Bullet.");
				return;
			}

			var throwVector = CalcThrowVector();
			_throwAnchor.forward = throwVector;

			var b = Instantiate(_bomb, _throwAnchor.position, _throwAnchor.rotation);
			b.ModuleData = _moduleData;
			b.Modular.Speed = throwVector.magnitude;
		}

		private Vector3 CalcThrowVector() {

			var lastVector = _samples.Last();

			// 計算に使うベクトルを求める
			var tmpSamples = _samples
				// 最後から見て後ろ向きのベクトルを除外
				.Where(item => Vector3.Dot(item, lastVector) > 0)
				.Select(item => (item, item.sqrMagnitude));

			// 標準偏差を求める
			var sum = 0.0f;
			var sum2 = 0.0f;
			foreach(var item in tmpSamples) {
				sum += item.sqrMagnitude;
				sum2 += item.sqrMagnitude * item.sqrMagnitude;
			}

			var avg = sum / tmpSamples.Count();
			var normalDev = Mathf.Sqrt(sum2 / tmpSamples.Count() - avg * avg);

			var filteredSamples = tmpSamples
				// 偏差値を求め、一定の偏差値を持つものを信頼するベクトルとして採用する
				.Where(item => {

					var diffSum = item.sqrMagnitude - avg;
					var diffDev = Mathf.Abs(diffSum) * 10 / normalDev;

					// 偏差値を求める
					var dev = 50.0f;
					if(diffSum > 0) {
						dev += diffDev;
					}
					else if(diffSum < 0) {
						dev -= diffDev;
					}

					// 偏差値60以上の値のみを利用
					return dev >= 60;
				})
				.Select(item => item.item)
				.ToArray();

			// ローパスフィルター
			var ratio = 0.8f;
			for(int i = 1;i < filteredSamples.Length;i++) {
				filteredSamples[i] = ratio * filteredSamples[i - 1] + (1 - ratio) * filteredSamples[i];
				filteredSamples[i - 1] = filteredSamples[i];
			}

			// 最小二乗平面を用いた推測値を元に速度を求める
			float[] result = Extensions.MathExtensions.CalcLeastSquaresPlane(filteredSamples);
			float a = result[0];
			float b = result[1];
			float c = result[2];

			// サンプリングした最後のデータを用いて、理想平面の値を求める
			float y = a + (b * lastVector.x) + (c * lastVector.z);

			// 実際に利用したいデータ
			return new Vector3(lastVector.x, y, lastVector.z);
		}
	}
}