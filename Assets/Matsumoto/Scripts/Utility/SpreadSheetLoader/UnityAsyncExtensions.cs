using UnityEngine;
using System.Collections;
using UniRx.Async;
using System.Collections.Generic;
using System.Threading;
using System;

namespace Matsumoto.Extensions {

	/// <summary>
	/// 非同期系の命令追加
	/// </summary>
	public static class UnityAsyncExtensions {

		/// <summary>
		/// InstantiateをAsyncで提供する。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="prefab"></param>
		/// <param name="count">生成の合計数</param>
		/// <param name="createPerFrame">1フレームに生成する数</param>
		/// <returns></returns>
		public static IEnumerator InstantiateAsync<T>(this UnityEngine.Object obj, T prefab, int count, int createPerFrame, Action<T[]> callback) where T : MonoBehaviour {

			var generated = new List<T>();
			var created = 0;
			while(true) {

				for(int i = 0;i < createPerFrame;i++) {
					generated.Add(UnityEngine.Object.Instantiate(prefab));
					if(created++ >= count) {
						callback?.Invoke(generated.ToArray());
						yield break;
					}
				}
				yield return null;
			}
		}
	}
}
