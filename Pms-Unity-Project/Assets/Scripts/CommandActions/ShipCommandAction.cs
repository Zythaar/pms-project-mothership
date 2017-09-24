using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CommandAction/Ship Command")]
public class ShipCommandAction : CommandAction 
{

    public override bool DoCommandAction(CommandController controller)
    {
        controller.Move();
        //if (controller.roomNavigation.currentRoom.roomName == requiredString)
        //{
        //    controller.roomNavigation.currentRoom = roomToChangeTo;
        //    controller.DisplayRoomText();
        //    return true;
        //}

        return false;
    }

}
