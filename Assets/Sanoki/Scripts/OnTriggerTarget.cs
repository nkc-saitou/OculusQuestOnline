using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sanoki.Lobby
{
    public class OnTriggerTarget : MonoBehaviour
    {
        AniController aniCon;
        Sanoki.Online.EntrySystem entrySystem;
        // Start is called before the first frame update
        void Start()
        {
            aniCon = FindObjectOfType<AniController>();
            entrySystem = FindObjectOfType<Sanoki.Online.EntrySystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                aniCon.AniStop();
                aniCon.EntryColor();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                aniCon.AniMove();
                aniCon.ExitColor();
            }

        }
        private void OnTriggerEnter(Collider other)
        {
            Matsumoto.Weapon.ModuleObject obj;
            obj = other.GetComponent<Matsumoto.Weapon.ModuleObject>();
            if (TestOnlineData.PlayerID == obj.Owner.GetMyProvider.MyID)
            {
                entrySystem.Entry();
                aniCon.AniStop();
                aniCon.EntryColor();
            }
        }
    }
}
