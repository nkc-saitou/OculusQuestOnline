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
        [Header("<親オブジェクト>")]
        public GameObject targetObj;
        private WeaponCreate weaponCreate;
        
        [SerializeField, Header("回転スピード")]
        private float rotateSpeed;
        // 差分
        [SerializeField]
        private Vector3 offset;

        void Start()
        {

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
            if (targetObj == null) return;

            // 差分計算
            transform.position = targetObj.transform.position + offset;
            transform.Rotate(0.0f, rotateSpeed, 0.0f);
        }
    }
}

