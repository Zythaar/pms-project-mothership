using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCommandAction : CommandAction 
{

    public override bool DoCommandAction(CommandController controller)
    {
        //if (controller.roomNavigation.currentRoom.roomName == requiredString)
        //{
        //    controller.roomNavigation.currentRoom = roomToChangeTo;
        //    controller.DisplayRoomText();
        //    return true;
        //}

        return false;
    }

}
