﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

namespace Matsumoto.Weapon {

	/// <summary>
	/// コントローラーで操作できる武器
	/// </summary>
	public interface IWeapon : IOculusControllable {

		UniTask Destroy(float fadeTime);

		GameObject GetBody();

		Transform GetGrabAnchor();

        void SetOwner(Nakajima.Player.PlayerHand owner);

		bool IsUsable {
			get;
		}

	}


}

