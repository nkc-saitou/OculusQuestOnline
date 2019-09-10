using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Movement;

/// <summary>
/// Movement派生クラス
/// </summary>
public class Mover : MovementComponetBase
{
    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move(float _value)
    {
        // 移動手段に応じて移動方法を定義
        switch (movementState)
        {
            case MovementState.MOVE_STICK:
                break;
            case MovementState.MOVE_INCLINATION:
                break;
        }
    }
}
