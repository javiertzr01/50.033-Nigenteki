using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour
{
    public ArmVariables armVariable;

    [SerializeField]
    protected List<GameObject> projectiles;

    [SerializeField]
    protected GameObject shootPoint;

    protected GameObject basicProjectile;


    void Awake()
    {
        Initialize();
    }

    // Initialize the arm with its variable settings
    public virtual void Initialize()
    {
        // Initialize arm with the variables from armVariable.
        // E.g., set the arm's sprite, attack power, etc.
        basicProjectile = projectiles[0];
    }

    // The basic attack method
    public abstract void CastBasicAttack();

    // The skill method
    public abstract void CastSkill();

    // The ultimate skill method
    public abstract void CastUltimate();
}
