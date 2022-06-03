using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PKU.Draw
{
    public class DrawDataManager : NetworkBehaviour
    {
        public List<DrawData> drawDataList = new List<DrawData>();

    }
}
