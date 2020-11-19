using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Animator anim;

    [Header("MainMenu")]
    public GameObject mainMenu;

    [Header("Mission Select")]
    public GameObject missionSelect;
    public Text mapName;
    public GameObject[] maps;
    public string[] mapNames;
    private int currentMap;

    [Header("EquipmentSelect")]
    private int currentPrimaryWeapon;
    private int currentSecondaryWeapon;
    public GameObject equipmentSelect;
    public GameObject secondarySelect;
    public weapontypes[] primaryWeapons;
    public weapontypes[] secondaryWeapons;
    public Text weaponname;
    public Text auto;
    public GameObject[] leth;
    public GameObject[] loud;
    public GameObject[] acc;
    public GameObject[] fire;
    public GameObject[] ammo;

    void Start()
    {
        anim = GetComponent<Animator>();
        //DisplayStats(primaryWeapons[0]);
    }

    public void NextMenu(int menu)
    {
        anim.SetInteger("Menu", menu);
    }

    void OpenMenu(int menu)
    {
        switch (menu)
        {
            case 0:
                mainMenu.SetActive(true);
                break;
            case 1:
                missionSelect.SetActive(true);
                break;
            case 2:
                equipmentSelect.SetActive(true);
                break;
            case 3:
                secondarySelect.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void CloseMenu(int menu)
    {
        switch (menu)
        {
            case 0:
                mainMenu.SetActive(false);
                break;
            case 1:
                missionSelect.SetActive(false);
                break;
            case 2:
                equipmentSelect.SetActive(false);
                break;
            case 3:
                secondarySelect.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void PreviousMap()
    {
        if (currentMap == 0)
        {
            return;
        }
        else
        {
            currentMap--;
        }
        ShowMap(currentMap);
    }

    public void NextMap()
    {
        if (currentMap == maps.Length)
        {
            return;
        }
        else
        {
            currentMap++;
        }
        ShowMap(currentMap);
    }

    void ShowMap(int map)
    {
        maps[map].SetActive(true);
        mapName.text = mapNames[map];
    }

    //Called in animator that cant pass booleans
    public void ShowPrimary(int boolean)
    {
        if (boolean == 1)
        {
            DisplayStats(primaryWeapons[currentPrimaryWeapon]);
            return;
        }
        DisplayStats(secondaryWeapons[currentSecondaryWeapon]);
    }
    
    public void PreviousWeapon(bool primary)
    {
        if (primary)
        {
            if (currentPrimaryWeapon == 0)
            {
                return;
            }
            primaryWeapons[currentPrimaryWeapon].weapon.SetActive(false);
            currentPrimaryWeapon--;
            DisplayStats(primaryWeapons[currentPrimaryWeapon]);
        }
        else
        {
            if (currentSecondaryWeapon == 0)
            {
                return;
            }
            secondaryWeapons[currentSecondaryWeapon].weapon.SetActive(false);
            currentSecondaryWeapon--;
            DisplayStats(secondaryWeapons[currentSecondaryWeapon]);
        }
    }

    public void NextWeapon(bool primary)
    {
        if (primary)
        {
            if (currentPrimaryWeapon == primaryWeapons.Length - 1)
            {
                return;
            }
            primaryWeapons[currentPrimaryWeapon].weapon.SetActive(false);
            currentPrimaryWeapon++;
            DisplayStats(primaryWeapons[currentPrimaryWeapon]);
        }
        else
        {
            if (currentSecondaryWeapon == secondaryWeapons.Length - 1)
            {
                return;
            }
            secondaryWeapons[currentSecondaryWeapon].weapon.SetActive(false);
            currentSecondaryWeapon++;
            DisplayStats(secondaryWeapons[currentSecondaryWeapon]);
        }
    }

    void DisplayStats(weapontypes w)
    {
        weaponname.text = w._name;
        DisableBlocks(leth, w._lethality);
        DisableBlocks(loud, w._loudness);
        DisableBlocks(acc, w._accuracy);
        DisableBlocks(fire, w._fireRate);
        DisableBlocks(ammo, w._ammo);
        w.weapon.SetActive(true);
        if (w.auto)
        {
            auto.text = "FULL AUTO";
        }
        else
        {
            auto.text = "SEMI AUTO";
        }
    }

    void DisableBlocks(GameObject[] ob, int amount)
    {
        foreach (var o in ob)
        {
            o.SetActive(false);
        }
        for (int i = 0; i < amount; i++)
        {
            ob[i].SetActive(true);
        }
    }

    [System.Serializable]
    public class weapontypes
    {
        public string _name;
        public bool auto;
        public int _lethality;
        public int _loudness;
        public int _accuracy;
        public int _fireRate;
        public int _ammo;
        public GameObject weapon;
    }
}
