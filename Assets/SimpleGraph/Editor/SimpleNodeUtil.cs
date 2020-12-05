using UnityEditor.Experimental.GraphView;

public static class SimpleNodeUtil
{
    private static System.Type m_objectType = typeof(SimpleNode); // temp stuff
    private static System.Type m_boolType = typeof(bool);

    public static SimpleNode CreateNode(SimpleNodeType nodeType, System.Guid nodeGuid = default)
    {
        if (default == nodeGuid)
            nodeGuid = System.Guid.NewGuid();

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
                node = CreateBranchNode(nodeGuid);
                break;
            default:
                node = CreateNormalNode(nodeGuid);
                break;
        }

        return node;
    }

    private static SimpleNode CreateStartNode(System.Guid nodeGuid)
    {
        SimpleNode node = null;
        node = new SimpleNode(nodeGuid, SimpleNodeType.StartNode);

        Port outPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, m_objectType);
        node.m_outputPorts.Add(outPort);
        node.outputContainer.Add(outPort);
        node.RefreshPorts();
        node.RefreshExpandedState();

        return node;
    }

    private static SimpleNode CreateNormalNode(System.Guid nodeGuid)
    {
        SimpleNode node = null;
        node = new SimpleNode(nodeGuid, SimpleNodeType.NormalNode);

        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, m_objectType);
        outputPort.portName = "next";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, m_objectType);
        inputPort.portName = "prev";
        node.outputContainer.Add(inputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_inputPorts.Add(inputPort);

        return node;
    }

    private static SimpleNode CreateBranchNode(System.Guid nodeGuid)
    {
        SimpleNode node = null;
        node = new SimpleNode(nodeGuid, SimpleNodeType.BranchNode);

        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, m_objectType);
        outputPort.portName = "true";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, m_objectType);
        outputPort.portName = "false";
        node.outputContainer.Add(outputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_outputPorts.Add(outputPort);

        Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, m_objectType);
        inputPort.portName = "prev";
        node.outputContainer.Add(inputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_inputPorts.Add(inputPort);

        inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, m_boolType);
        inputPort.portName = "if";
        node.outputContainer.Add(inputPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.m_inputPorts.Add(inputPort);

        return node;
    }

}
