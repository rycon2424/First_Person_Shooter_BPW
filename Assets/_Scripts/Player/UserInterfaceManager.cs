using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    public GameObject[] bullets;
    public GameObject[] mags;
    public Slider playerHp;
    public Text weaponName;

    public void UpdateWeaponName(string newName)
    {
        weaponName.text = newName;
    }

    public void UpdateMags(int currentmags)
    {
        foreach (var m in mags)
        {
            m.SetActive(false);
        }
        for (int i = 0; i < currentmags; i++)
        {
            mags[i].SetActive(true);
        }
    }

    public void UpdateAmmo(int currentbullets)
    {
        foreach (var b in bullets)
        {
            b.SetActive(false);
        }
        for (int i = 0; i < currentbullets; i++)
        {
            bullets[i].SetActive(true);
        }
    }
}
