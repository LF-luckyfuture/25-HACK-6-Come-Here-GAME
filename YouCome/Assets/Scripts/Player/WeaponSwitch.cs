using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponSwitch : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;
    public AudioTest AudioTest;
    void Start()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[currentWeaponIndex].SetActive(true);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SwitchWeapon();
        }
    }
    void SwitchWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        weapons[currentWeaponIndex].SetActive(true);

        if (currentWeaponIndex == 0)
        {
            AudioTest.FistSwitch();
        }
        else if (currentWeaponIndex == 1)
        {
            AudioTest.MacheteSwitch();
        }
    }
}
