using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleGraphView : GraphView
{
    private readonly Vector2 NODE_SIZE = new Vector2(200.0f, 200.0f);

    public SimpleGraphView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        this.styleSheets.Add(Resources.Load<StyleSheet>("SimpleGraphView"));
        Insert(0, gridBackground);

        // do we need to un-register those call back if we want to use our own callback, some of them may cause problem 
        // RegisterCallback<ValidateCommandEvent>(OnValidateCommand);
        // RegisterCallback<ExecuteCommandEvent>(OnExecuteCommand);
        // RegisterCallback<AttachToPanelEvent>(OnEnterPanel);
        // RegisterCallback<DetachFromPanelEvent>(OnLeavePanel);
        // RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenu);

        CreateStartNode();
    }

    public void ClearData()
    {
        List<GraphElement> tempElement = new List<GraphElement>();

        tempElement.AddRange(this.edges.ToList());
        for (int i = 0; i < tempElement.Count; i++)
            RemoveElement(tempElement[i]);

        tempElement.AddRange(this.nodes.ToList());
        for (int i = 0; i < tempElement.Count; i++)
            RemoveElement(tempElement[i]);
    }

    public void InjectGraphData(NodeData[] nodeDatas, NodeLinkData[] nodeLinkDatas)
    {
        ClearData();

        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        this.styleSheets.Add(Resources.Load<StyleSheet>("SimpleGraphView"));
        Insert(0, gridBackground);

        // should load data correctly
        Dictionary<System.Guid, SimpleNode> tempNodes = new Dictionary<System.Guid, SimpleNode>();
        for (int i = 0; i < nodeDatas.Length; i++)
        {
            System.Guid nodeGuid = default;
            if (!System.Guid.TryParse(nodeDatas[i].NodeGuidString, out nodeGuid))
                nodeGuid = System.Guid.NewGuid();

            SimpleNode node = CreateNode((SimpleNodeType)nodeDatas[i].NodeType, nodeGuid);
            node.SetPosition(new Rect(nodeDatas[i].NodePosition.x, nodeDatas[i].NodePosition.y, node.contentRect.width, node.contentRect.height));
            tempNodes.Add(nodeGuid, node);
        }

        // how should I load the link
        for (int i = 0; i < nodeLinkDatas.Length; i++)
        {
            SimpleNode inputNode = tempNodes[System.Guid.Parse(nodeLinkDatas[i].InputNodeGuidString)];
            SimpleNode outputNode = tempNodes[System.Guid.Parse(nodeLinkDatas[i].OutputNodeGuidString)];
            Port inputPort = inputNode.m_inputPorts[nodeLinkDatas[i].InputPortIndex];
            Port outputPort = outputNode.m_outputPorts[nodeLinkDatas[i].OutputPortIndex];
            Edge edge = inputPort.ConnectTo(outputPort);
            AddElement(edge);
        }
    }

    public SimpleNode CreateNode(SimpleNodeType nodeType, System.Guid nodeGuid = default)
    {
        SimpleNode node = null;

        switch (nodeType)
        {
            case SimpleNodeType.StartNode:
                node = CreateStartNode(nodeGuid);
                break;
            case SimpleNodeType.NormalNode:
                node = CreateNormalNode(nodeGuid);
                break;
            case SimpleNodeType.BranchNode:
                node = CreateBrunchNode(nodeGuid);
                break;
            default:
                node = CreateNormalNode(nodeGuid);
                break;
        }
        return node;
    }

    public SimpleNode CreateStartNode(System.Guid guid = default)
    {
        if (default == guid)
            guid = System.Guid.NewGuid();

        SimpleNode node = SimpleNodeUtil.CreateNode(SimpleNodeType.StartNode, guid);
        node.title = $"start node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(400.0f, 400.0f, 200.0f, 200.0f));
        AddElement(node);
        return node;
    }

    public SimpleNode CreateNormalNode(System.Guid guid = default)
    {
        if (default == guid)
            guid = System.Guid.NewGuid();

        SimpleNode node = SimpleNodeUtil.CreateNode(SimpleNodeType.NormalNode, guid);
        node.title = $"normal node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(400.0f, 400.0f, NODE_SIZE.x, NODE_SIZE.y));
        AddElement(node);
        return node;
    }

    public SimpleNode CreateBrunchNode(System.Guid guid = default)
    {
        if (default == guid)
            guid = System.Guid.NewGuid();

        SimpleNode node = SimpleNodeUtil.CreateNode(SimpleNodeType.BranchNode, guid);
        node.title = $"branch node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(400.0f, 400.0f, NODE_SIZE.x, NODE_SIZE.y));
        AddElement(node);
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        foreach (var port in ports.ToList())
        {
            if (startAnchor.node == port.node ||
                startAnchor.direction == port.direction ||
                startAnchor.portType != port.portType)
            {
                continue;
            }

            compatiblePorts.Add(port);
        }
        return compatiblePorts;
    }

}