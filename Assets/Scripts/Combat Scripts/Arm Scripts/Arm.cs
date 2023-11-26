using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour, INetworkSerializable
{
    public ArmVariables armVariable; // Ensure ArmVariables is serializable if it contains network-relevant data

    // Serialize references by ID or some other network-friendly method, not directly
    [SerializeField]
    protected List<GameObject> projectiles; // Consider serializing relevant state information instead of GameObjects

    [SerializeField]
    protected GameObject shootPoint; // Same as above, consider what needs to be synchronized

    protected GameObject basicProjectile;
    private float _ultimateCharge;

    void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        basicProjectile = projectiles[0];
    }

    private float _skillCoolDown;

    public float SkillCoolDown
    {
        get => _skillCoolDown;
        set => _skillCoolDown = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default) { }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastSkillServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastSkillClientRpc(ClientRpcParams clientRpcParams = default) { }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default) { }

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
        get => _ultimateCharge;
        set => _ultimateCharge = Mathf.Clamp(value, 0, 100);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}
