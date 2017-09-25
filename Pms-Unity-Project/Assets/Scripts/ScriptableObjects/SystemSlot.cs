using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SystemSlotClassType { Engine, Defense, Ship, Station }

[CreateAssetMenu(menuName = "SystemSlot/SlotClass")]
public class SystemSlot : ScriptableObject 
{

    new public string name;
    public SystemSlotClassType slotClass;

    void Install()
    {

    }
	
}
