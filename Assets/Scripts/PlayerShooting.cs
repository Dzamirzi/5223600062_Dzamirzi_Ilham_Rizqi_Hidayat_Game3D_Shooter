using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    public Weapon weapon;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            weapon.Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weapon.StopShoot();
        }
    }
}