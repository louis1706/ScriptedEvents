﻿using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class LockDoorAction : IAction
    {
        public string Name => "LOCKDOOR";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing arguments: doorType, duration(optional)");

            if (!ScriptHelper.TryGetDoors(Arguments.ElementAt(0), out List<Door> doors))
                return new(false, "Invalid door(s) provided!");

            foreach (var door in doors)
            {
                var locks = (DoorLockType)door.Base.NetworkActiveLocks;
                locks |= DoorLockType.AdminCommand;
                door.Base.NetworkActiveLocks = (ushort)locks;
                if (Arguments.Length == 2)
                {
                    if (!ScriptHelper.TryConvertNumber(Arguments[1], out int duration))
                        return new(false, "Second argument must be an int or range of ints!");
                    Timing.CallDelayed(duration, () =>
                    {
                        var locks = (DoorLockType)door.Base.NetworkActiveLocks;
                        locks &= ~DoorLockType.AdminCommand;
                        door.Base.NetworkActiveLocks = (ushort)locks;
                    });
                }
            }
            return new(true, string.Empty);
        }
    }
}
