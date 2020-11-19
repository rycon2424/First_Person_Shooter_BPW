using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearInstance : MonoBehaviour
{
    public static GearInstance instance;
    public Weapon primary;
    public Weapon secondary;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void Primary(string weaponName)
    {
        if (primary != null)
        {
            primary.weaponName = weaponName;
            return;
        }
        primary = gameObject.AddComponent<Weapon>();
        primary.weaponName = weaponName;
        primary.primary = true;
    }

    public void Secondary(string weaponName)
    {
        if (secondary != null)
        {
            secondary.weaponName = weaponName;
            return;
        }
        secondary = gameObject.AddComponent<Weapon>();
        secondary.weaponName = weaponName;
        secondary.primary = false;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}

