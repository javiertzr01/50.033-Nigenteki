using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientData
{
    public ulong clientId;
    public int characterId = -1;
    public int teamId = -1;
    public int leftArmId = -1;
    public int rightArmId = -1;

    public ClientData(ulong clientId)
    {
        this.clientId = clientId;

    }
}
