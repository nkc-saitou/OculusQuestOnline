using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniController : MonoBehaviour
{
    public Animator[] moveAni=new Animator[2];
    public Animator[] colorAni=new Animator[2];

    public void AniMove()
    {
        for (int i = 0; i < 2; i++) {
            moveAni[i].SetBool("IsDone", false);
        }
    }
    public void AniStop()
    {
        for (int i = 0; i < 2; i++)
        {
            moveAni[i].SetBool("IsDone", true);
        }
    }

    public void EntryColor()
    {
        for (int i = 0; i < 2; i++)
        {
            colorAni[i].SetTrigger("Entry");
        }
    }
    public void ExitColor()
    {
        for (int i = 0; i < 2; i++)
        {
            colorAni[i].SetTrigger("Exit");
        }
    }
}
