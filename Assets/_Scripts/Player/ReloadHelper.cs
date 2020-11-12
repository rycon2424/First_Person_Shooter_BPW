using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadHelper : MonoBehaviour
{
    private FPSPlayer player;
    private void Start()
    {
        player = FindObjectOfType<FPSPlayer>();
    }
    //Called in an animation event
    public void FinishReload()
    {
        player.FinishReload();
    }
}
