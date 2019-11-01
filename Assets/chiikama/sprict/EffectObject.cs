using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public GameObject[] Effects;

    public void DestroyEffect()
    {
        foreach (var item in Effects)
        {
            item.transform.SetParent(null);
            Destroy(item,10);
        }
    }
}
  
