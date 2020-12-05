using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

/*
for nodes, we care about the type and ports connection for now
*/

public enum SimpleNodeType
{
    StartNode = 0, // 0 in 1 out
    NormalNode = 1, // 1 in 1 out
    BranchNode = 2,// 1 in 2 out or....
}

public class SimpleNode : Node
{
    public System.Guid m_nodeGuid = System.Guid.Empty;
    public string m_guidShortStr = string.Empty;

    public List<Port> m_inputPorts = null;
    public List<Port> m_outputPorts = null;
    private SimpleNodeType m_nodeType = default;

    public SimpleNodeType NodeType => m_nodeType;

    public SimpleNode(System.Guid guid, SimpleNodeType nodeType)
    {
        m_nodeGuid = guid;
        m_guidShortStr = m_nodeGuid.ToString().Substring(0, 5);
        m_nodeType = nodeType;

        m_inputPorts = new List<Port>();
        m_outputPorts = new List<Port>();

        this.title = m_guidShortStr;
    }
}
