using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sanoki.Lobby
{
    public class TargetHit : MonoBehaviour
    {
        Animator ani;
        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Matsumoto.Weapon.ModuleObject>())
            {
                ani.SetTrigger("Hit");
            }
        }


    }
}
