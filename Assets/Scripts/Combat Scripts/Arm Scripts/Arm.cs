using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : MonoBehaviour
{
    public ArmVariables armVariable;

    [SerializeField]
    protected List<GameObject> projectiles;

    [SerializeField]
    protected GameObject shootPoint;

    protected GameObject basicProjectile;
    private float _ultimateCharge;



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

    // Higher the Divisor, the slower the charging rate
    public void ChargeUltimate(float charge, float divisor)
    {
        if (divisor < 1)
        {
            divisor = 1;
        }
        UltimateCharge += (charge / divisor);
        Debug.Log(armVariable.armName + " Ultimate Charge: " + UltimateCharge);
    }

    public float UltimateCharge
    {
        get
        {
            return _ultimateCharge;
        }
        set
        {
            if (value >= 100)
            {
                _ultimateCharge = 100;
            }
            else
            {
                _ultimateCharge = value;
            }

        }
    }
}
