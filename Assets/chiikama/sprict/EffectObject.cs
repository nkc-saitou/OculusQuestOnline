using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public ParticleSystem[] Effects;

	public void PlayEffect() {
		foreach(var item in Effects) {
			item.Play();
		}
	}

	public void DestroyEffect()
    {
		Destroy(gameObject, 10);
		foreach(Transform item in transform) {
			item.transform.SetParent(null);
			Destroy(item.gameObject, 10);
		}

		foreach(var item in Effects) {
			item.transform.SetParent(null);
			item.Stop();
		}
	}
}
  
