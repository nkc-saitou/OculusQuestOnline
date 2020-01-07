using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matsumoto {

	/// <summary>
	/// Oculusのコントローラーで操作できる
	/// </summary>
	public interface IOculusControllable {

		/// <summary>
		/// ボタンが押された瞬間
		/// </summary>
		/// <param name="button">対象のボタン</param>
		void OnButtonDown(OVRInput.Button button);

		/// <summary>
		/// ボタンの近くに指があるとき(触れてはいない)
		/// </summary>
		/// <param name="nearTouch">対象のボタン</param>
		void OnButtonNear(OVRInput.NearTouch nearTouch);

		/// <summary>
		/// ボタンに触れているとき(押してはいない)
		/// </summary>
		/// <param name="touch">対象のボタン</param>
		void OnButtonTouch(OVRInput.Touch touch);

		/// <summary>
		/// ボタンが押されているとき
		/// </summary>
		/// <param name="button">対象のボタン</param>
		void OnButton(OVRInput.Button button);

		/// <summary>
		/// ボタンが押された瞬間
		/// </summary>
		/// <param name="button">対象のボタン</param>
		void OnButtonUp(OVRInput.Button button);

		/// <summary>
		/// トリガーの値をアナログ値で与える
		/// </summary>
		/// <param name="type">対象のトリガー</param>
		/// <param name="axis">値</param>
		void OnTriggerAnalogValue(OVRInput.Axis1D type, float axis);

		/// <summary>
		/// スティックの値をアナログ値で与える
		/// </summary>
		/// <param name="type">対象のスティック</param>
		/// <param name="axis">値</param>
		void OnStickAnalogValue(OVRInput.Axis2D type, Vector2 axis);

		/// <summary>
		/// コントローラーの振動を取得する
		/// </summary>
		/// <param name="type">対象のスティック</param>
		/// <param name="axis">値</param>
		OVRHapticsClip GetHaptics();

	}
}


