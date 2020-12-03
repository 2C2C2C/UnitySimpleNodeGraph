using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public enum NodeType
{
    StartNode = 0, // 0 in 1 out
    NormalNode = 1, // 1 in 1 out
    IfNode = 2,// 1 in 2 out or....
}

public class TestNode : Node
{
    public System.Guid m_nodeGuid = System.Guid.Empty;
    public string m_guidShortStr = string.Empty;
    public bool m_isEntryNode = false;

    public List<Port> m_inputPorts = null;
    public List<Port> m_outputPorts = null;
    public NodeType m_nodeType = default;

    public TestNode()
    {
        m_nodeGuid = System.Guid.NewGuid();
        m_guidShortStr = m_nodeGuid.ToString().Substring(0, 5);
        this.title = m_guidShortStr;
        m_isEntryNode = false;

        m_inputPorts = new List<Port>();
        m_outputPorts = new List<Port>();
    }

    public TestNode(System.Guid guid)
    {
        m_nodeGuid = guid;
        m_guidShortStr = m_nodeGuid.ToString().Substring(0, 5);
        this.title = m_guidShortStr;
        m_isEntryNode = false;

        m_inputPorts = new List<Port>();
        m_outputPorts = new List<Port>();
    }
}
