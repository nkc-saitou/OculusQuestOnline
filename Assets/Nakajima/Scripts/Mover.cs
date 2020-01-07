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
            // スティック移動
            case MovementState.MOVE_STICK:
                AddInputVector(_vec);
                break;
            // 傾き移動
            case MovementState.MOVE_INCLINATION:
                AddInputVector_Tracking(_vec);
                break;
        }
    }
}
