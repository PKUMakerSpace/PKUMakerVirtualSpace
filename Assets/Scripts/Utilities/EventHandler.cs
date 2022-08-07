using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PKU.Event
{
    public static class EventHandler
    {
        public static event Action PlayerReadEvent;
        public static void CallPlayerReadEvent()
        {
            PlayerReadEvent?.Invoke();
        }


    }

}
