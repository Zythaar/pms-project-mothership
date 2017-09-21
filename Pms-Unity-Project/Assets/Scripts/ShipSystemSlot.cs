using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipSystemClassType { Worker, Transporter, Warrior, Scout };

[CreateAssetMenu(menuName = "SystemSlot/ShipSlotClass")]
public class ShipSystemSlot : SystemSlot 
{
    
    public ShipSystemClassType shipSlotClass;

	
}
