using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleGraphView : GraphView
{
    private readonly Vector2 NODE_SIZE = new Vector2(200.0f, 200.0f);
    private System.Type m_objectType = null;
    private System.Type m_boolType = null;

    // private List<VisualElement> m_tempStuff = null;

    public SimpleGraphView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        m_objectType = typeof(object);
        m_boolType = typeof(bool);

        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        this.styleSheets.Add(Resources.Load<StyleSheet>("SimpleGraphView"));
        Insert(0, gridBackground);

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
        Dictionary<System.Guid, TestNode> tempNodes = new Dictionary<System.Guid, TestNode>();
        for (int i = 0; i < nodeDatas.Length; i++)
        {
            System.Guid nodeGuid = default;
            if (!System.Guid.TryParse(nodeDatas[i].NodeGuidString, out nodeGuid))
                nodeGuid = System.Guid.NewGuid();

            TestNode node = CreateNode((NodeType)nodeDatas[i].NodeType, nodeGuid);
            node.SetPosition(new Rect(nodeDatas[i].NodePosition.x, nodeDatas[i].NodePosition.y, node.contentRect.width, node.contentRect.height));
            tempNodes.Add(nodeGuid, node);
        }

        // how should I load the link
        for (int i = 0; i < nodeLinkDatas.Length; i++)
        {
            TestNode inputNode = tempNodes[System.Guid.Parse(nodeLinkDatas[i].InputNodeGuidString)];
            TestNode outputNode = tempNodes[System.Guid.Parse(nodeLinkDatas[i].OutputNodeGuidString)];
            Port inputPort = inputNode.m_inputPorts[nodeLinkDatas[i].InputPortIndex];
            Port outputPort = outputNode.m_outputPorts[nodeLinkDatas[i].OutputPortIndex];
            Edge edge = inputPort.ConnectTo(outputPort);
            AddElement(edge);
        }
    }

    public TestNode CreateNode(NodeType nodeType, System.Guid nodeGuid = default)
    {
        TestNode node = null;

        switch (nodeType)
        {
            case NodeType.StartNode:
                node = CreateStartNode(nodeGuid);
                break;
            case NodeType.NormalNode:
                node = CreateNormalNode(nodeGuid);
                break;
            case NodeType.IfNode:
                node = CreateBrunchNode(nodeGuid);
                break;
            default:
                node = CreateNormalNode(nodeGuid);
                break;
        }
        return node;
    }

    public TestNode CreateStartNode(System.Guid guid = default)
    {
        TestNode node = null;
        if (default == guid)
            node = new TestNode();
        else
            node = new TestNode(guid);

        node.m_guidShortStr = node.m_nodeGuid.ToString().Substring(0, 5);
        node.title = $"start node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(400.0f, 400.0f, 200.0f, 200.0f));
        node.m_nodeType = NodeType.StartNode;

        Port outPort = AddNodeOutoutPort(node, Direction.Output);
        node.m_outputPorts.Add(outPort);
        node.outputContainer.Add(outPort);
        node.RefreshPorts();
        node.RefreshExpandedState();

        AddElement(node);
        return node;
    }

    public TestNode CreateNormalNode(System.Guid guid = default)
    {
        TestNode node = null;
        if (default == guid)
            node = new TestNode();
        else
            node = new TestNode(guid);

        node.title = $"normal node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(200.0f, 200.0f, NODE_SIZE.x, NODE_SIZE.y));

        Port outputPort = AddNodeOutoutPort(node, Direction.Output);
        outputPort.portName = "next";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        Port inputPort = AddNodeOutoutPort(node, Direction.Input);
        inputPort.portName = "prev";
        node.outputContainer.Add(inputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_inputPorts.Add(inputPort);
        node.m_nodeType = NodeType.NormalNode;

        AddElement(node);
        return node;
    }

    public TestNode CreateBrunchNode(System.Guid guid = default)
    {
        TestNode node = null;
        if (default == guid)
            node = new TestNode();
        else
            node = new TestNode(guid);
        node.title = $"branch node - {node.m_guidShortStr}";
        node.SetPosition(new Rect(200.0f, 200.0f, NODE_SIZE.x, NODE_SIZE.y));

        Port outputPort = AddNodeOutoutPort(node, Direction.Output);
        outputPort.portName = "true";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        outputPort = AddNodeOutoutPort(node, Direction.Output);
        outputPort.portName = "false";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        Port inputPort = AddNodeInputPort(node, Direction.Input, m_boolType);
        inputPort.portName = "if";
        node.outputContainer.Add(inputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_inputPorts.Add(inputPort);
        node.SetPosition(new Rect(200.0f, 200.0f, NODE_SIZE.x, NODE_SIZE.y));
        node.m_nodeType = NodeType.IfNode;

        AddElement(node);
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        this.ports.ForEach((port) =>
        {
            if (port != startPort && port.node != startPort.node && port.direction != startPort.direction)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port AddNodeOutoutPort(in TestNode node, Direction protDirection, Port.Capacity portCapcity = Port.Capacity.Single)
    {
        if (null != node) // check Orientation with direction
            return node.InstantiatePort(Orientation.Horizontal, protDirection, portCapcity, m_objectType);
        else
            return null;
    }

    private Port AddNodeInputPort(in TestNode node, Direction protDirection, System.Type inputDataType, Port.Capacity portCapcity = Port.Capacity.Single)
    {
        if (null != node) // check Orientation with direction
            return node.InstantiatePort(Orientation.Horizontal, protDirection, portCapcity, inputDataType);
        else
            return null;
    }

}