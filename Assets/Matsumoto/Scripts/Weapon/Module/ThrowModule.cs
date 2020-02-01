using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nakajima.Player;
using UniRx;

namespace Matsumoto.Weapon {

	public class ThrowModule : WeaponModuleBase {

        [SerializeField]
		private ModuleObject _bomb;

		[SerializeField]
		private int _sampleCount;

		[SerializeField]
		private float _throwPower = 100;

        private static int _createCount = 0;
        private int _myID = _createCount++;

        private Transform _throwAnchor;
		private Vector3 _prevPosition;
		private List<Vector3> _samples = new List<Vector3>();
        private NetworkEventManager manager;

        private void Update() {

			// Sample Vec
			_samples.Add((Owner.Value.transform.localPosition - _prevPosition));
			_prevPosition = Owner.Value.transform.localPosition;

			if(_samples.Count > _sampleCount) {
				_samples.RemoveAt(0);
			}

		}

		public override void ModuleInitialize(WeaponBase weapon) {
			base.ModuleInitialize(weapon);

            _prevPosition = Owner.Value.transform.localPosition;
            _samples = new List<Vector3>();

            Owner.Subscribe(item =>
            {
                Debug.Log("OwnerSet :" + item);

                if (!item) return;
                var playerID = item.GetMyProvider.MyID;
                manager = FindObjectOfType<NetworkEventManager>();
                manager.AddSyncEvent(playerID, "ThrowModule_Throw" + _myID, (data) => {
                    var p = (Vector3)(data[0]);
                    var r = (Quaternion)(data[1]);
                    var v = (Vector3)(data[2]);
                    var b = Instantiate(_bomb, p, r);
					b.Owner = Owner.Value;
                    b.ModuleData = _moduleData;
                    b.Modular.Speed = v.magnitude;

					Audio.AudioManager.PlaySE("whip-gesture1", position: p);

				});
            });

            var transforms = weapon.transform.GetComponentsInChildren<Transform>();
			foreach(Transform item in transforms) {
				if(item.name == "[ShotAnchor]") {
					_throwAnchor = item;
					return;
				}
			}

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

			var throwVector = CalcThrowVector() * _throwPower;
			Debug.Log("throwVec :" + throwVector);
			_throwAnchor.forward = throwVector;
			Debug.DrawLine(_throwAnchor.position, _throwAnchor.position + throwVector, Color.red, 1);

            manager.CallSyncEvent("ThrowModule_Throw" + _myID, new object[] { _throwAnchor.position, _throwAnchor.rotation, throwVector });

		}

		private Vector3 CalcThrowVector() {

			for(int i = 0;i < _samples.Count;i++) {
				_samples[i] += Owner.Value.transform.position;
			}

			var lastVector = _samples.Last();

			DebugLine(_samples.ToArray(), Color.white);

			// 計算に使うベクトルを求める
			var tmpSamples = _samples
				// 最後から見て後ろ向きのベクトルを除外
				.Where(item => Vector3.Dot(item, lastVector) > 0)
				.Select(item => (item, item.sqrMagnitude));

			DebugLine(tmpSamples.Select(item => item.item).ToArray(), Color.red);


			// 標準偏差を求める
			var sum = 0.0f;
			var sum2 = 0.0f;
			foreach(var item in tmpSamples) {
				sum += item.Item2;
				sum2 += item.Item2 * item.Item2;
			}

			var avg = sum / tmpSamples.Count();
			var normalDev = Mathf.Sqrt(sum2 / tmpSamples.Count() - avg * avg);

			var filteredSamples = tmpSamples
				// 偏差値を求め、一定の偏差値を持つものを信頼するベクトルとして採用する
				.Where(item => {

					var diffSum = item.Item2 - avg;
					var diffDev = Mathf.Abs(diffSum) * 10 / normalDev;

					// 偏差値を求める
					var dev = 50.0f;
					if(diffSum > 0) {
						dev += diffDev;
					}
					else if(diffSum < 0) {
						dev -= diffDev;
					}
					Debug.Log("dev" + dev);

					// 偏差値60以上の値のみを利用
					return dev >= 55;
				})
				.Select(item => item.item)
				.ToArray();

			DebugLine(filteredSamples.ToArray(), Color.yellow);

			// ローパスフィルター
			var ratio = 0.8f;
			for(int i = 1;i < filteredSamples.Length;i++) {
				filteredSamples[i] = ratio * filteredSamples[i - 1] + (1 - ratio) * filteredSamples[i];
				filteredSamples[i - 1] = filteredSamples[i];
			}

			DebugLine(filteredSamples, Color.green);
			Debug.Log("count : " + filteredSamples.Length);
			for(int i = 0;i < filteredSamples.Length;i++) {
				Debug.Log("item[" + i + "] : " + filteredSamples[i]);
			}

			// 最小二乗平面を用いた推測値を元に速度を求める
			var result = Extensions.MathExtensions.CalcLeastSquaresPlane(filteredSamples);
			var a = result[0];
			var b = result[1];
			var c = result[2];

			Debug.Log($"{a},{b},{c}");

			// サンプリングした最後のデータを用いて、理想平面の値を求める
			//var v = filteredSamples.Last();

			//if(float.IsNaN(y)) {
			//	y = 0;
			//}

			//var plane = new Vector3(a, b, c);
			var last = filteredSamples.Last();
			var y = a + (b * last.x) + (c * last.z);

			if(float.IsNaN(y)) y = 0;

			//Debug.DrawLine(new Vector3(), last, Color.magenta, 1);
			//Debug.DrawLine(new Vector3(), plane, Color.blue, 1);
			//Debug.DrawLine(new Vector3(), throwVec, Color.cyan, 1);
			//Debug.DrawLine(new Vector3(), new Vector3(last.x, y, last.z), Color.white, 1);
			var throwVec = new Vector3(last.x, y, last.z);
			// 実際に利用したいデータ
			return throwVec;
		}

		private void DebugLine(Vector3[] points, Color color) {

			for(int i = 0;i < points.Length;i++) {
				Debug.DrawLine(new Vector3(), points[i], color, 1);
			}

		}
	}
}