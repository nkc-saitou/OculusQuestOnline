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
    public override void Move(Vector3 _vec)
    {
        // 移動手段に応じて移動方法を定義
        switch (movementState)
        {
            case MovementState.MOVE_STICK:
                AddInputVector(_vec);
                break;
            case MovementState.MOVE_INCLINATION:
                break;
        }
    }


}
