﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphDataContainer : ScriptableObject
{
    public NodeData[] m_nodeDataList = null;
    public NodeLinkData[] m_nodeLinkList = null;
}
