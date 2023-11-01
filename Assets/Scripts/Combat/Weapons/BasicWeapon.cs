using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicWeapon : MonoBehaviour
{
    public CombatSO combatSO;
    public abstract void Shoot();
}