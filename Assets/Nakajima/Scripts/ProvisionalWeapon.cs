using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仮武器クラス
/// </summary>
namespace Nakajima.Weapon
{
    public class ProvisionalWeapon : MonoBehaviour
    {
        // 自身の生成元の参照
        private WeaponCreate weaponCreate;

        // x,y座標の移動量
        float moveX;
        float moveZ;

        // 自身の原点
        private Vector3 originPos;

        void Start()
        {
            originPos = transform.position;
            weaponCreate = FindObjectOfType<WeaponCreate>();
        }

        void Update()
        {
            Move();
        }

        /// <summary>
        /// 円移動を繰り返す
        /// </summary>
        private void Move()
        {
            transform.Rotate(0.0f, 0.5f, 0.0f);
        }
    }
}

