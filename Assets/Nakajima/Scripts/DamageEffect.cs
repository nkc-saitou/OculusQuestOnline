using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ダメージエフェクトクラス
/// </summary>
namespace Nakajima.Player
{
    public class DamageEffect : MonoBehaviour
    {
        private Material myEffectMaterial;

        private bool isFade;

        // キャンバスに使用するアルファ値(今回は1.0fで見えない)
        private float alpha = 0.0f;

        void Start()
        {
            // マテリアルのセット
            myEffectMaterial = GetComponent<MeshRenderer>().material;
        }
        
        void Update()
        {

        }

        /// <summary>
        /// ダメージを受けた場合の処理
        /// </summary>
        [ContextMenu("Damage")]
        public void OnDamage()
        {
            StartCoroutine(ChangeParam(1.0f, 1.0f));
        }

        /// <summary>
        /// パラメータの変更
        /// </summary>
        /// <param name="_param">目標のパラメータ</param>
        /// <param name="_interval">変更にかかる時間</param>
        public IEnumerator ChangeParam(float _param, float _interval)
        {
            if (isFade) yield break;

            isFade = true;

            // 経過時間
            float time = 0.0f;
            while (time < _interval)
            {
                alpha = Mathf.Lerp(alpha, _param, time / _interval);

                myEffectMaterial.SetFloat("_GradationPower", alpha);

                time += Time.deltaTime;

                yield return null;
            }

            isFade = false;

            if (alpha == 1.0f) ResetCanvas();
        }

        /// <summary>
        /// キャンバスの状態をリセットする
        /// </summary>
        private void ResetCanvas()
        {
            StartCoroutine(ChangeParam(0.0f, 1.0f));
        }
    }
}


