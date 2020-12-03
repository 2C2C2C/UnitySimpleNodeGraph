using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class GraphSaveUtil
{
    private static readonly string SAVE_FOLDER_PATH = "Assets/";

    public static bool TrySaveGraphData(in string fileName, List<Node> graphNodes, List<Edge> graphLinks)
    {
        bool result = false;

        List<NodeData> nodeDatas = new List<NodeData>();
        NodeData nodeData = null;
        TestNode testNode = null;
        for (int i = 0; i < graphNodes.Count; i++)
        {
            nodeData = new NodeData();
            testNode = graphNodes[i] as TestNode;
            nodeData.NodeGuidString = testNode.m_nodeGuid.ToString();
            nodeData.NodeDataString = testNode.m_guidShortStr;
            nodeData.NodePosition = testNode.GetPosition().position;
            nodeData.NodeType = (int)testNode.m_nodeType;

            nodeDatas.Add(nodeData);
        }

        List<NodeLinkData> nodeLinkDatas = new List<NodeLinkData>();
        NodeLinkData linkData = null;
        TestNode inputNode = null;
        TestNode outputNode = null;
        for (int i = 0; i < graphLinks.Count; i++)
        {
            linkData = new NodeLinkData();
            inputNode = graphLinks[i].input.node as TestNode;
            linkData.InputPortName = graphLinks[i].input.name;
            linkData.InputNodeGuidString = inputNode.m_nodeGuid.ToString();
            int inputNodeIndex = inputNode.m_inputPorts.FindIndex(port => port == graphLinks[i].input);
            linkData.InputPortIndex = inputNodeIndex;

            outputNode = graphLinks[i].output.node as TestNode;
            linkData.OutputNodeGuidString = outputNode.m_nodeGuid.ToString();
            linkData.OutputPortName = graphLinks[i].output.name;
            int outputNodeIndex = outputNode.m_outputPorts.FindIndex(port => port == graphLinks[i].output);
            linkData.OutputPortIndex = outputNodeIndex;

            nodeLinkDatas.Add(linkData);
        }

        GraphDataContainer container = new GraphDataContainer();
        container.m_nodeDataList = nodeDatas.ToArray();
        container.m_nodeLinkList = nodeLinkDatas.ToArray();

        string path = $"{SAVE_FOLDER_PATH}{fileName}.asset";
        AssetDatabase.CreateAsset(container, path);

        return result;
    }

    public static bool TryLoadGraphData(out NodeData[] outNodeData, out NodeLinkData[] outLinkData)
    {
        outNodeData = null;
        outLinkData = null;

        string path = EditorUtility.OpenFilePanel("title", SAVE_FOLDER_PATH, "asset");
        // AssetDatabase.va
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.Log($"path empty");
            return false;
        }

        // Debug.Log($"select graph data {path}");
        int cutIndex = path.IndexOf("Assets/");
        path = path.Substring(cutIndex);
        GraphDataContainer data = AssetDatabase.LoadAssetAtPath<GraphDataContainer>(path);

        if (data == null)
        {
            // Debug.Log($"load data fail");
            return false;
        }

        NodeData[] nodeDatas = data.m_nodeDataList;
        NodeLinkData[] nodeLInks = data.m_nodeLinkList;
        outNodeData = new NodeData[nodeDatas.Length];
        outLinkData = new NodeLinkData[nodeLInks.Length];
        System.Array.Copy(nodeDatas, outNodeData, outNodeData.Length);
        System.Array.Copy(nodeLInks, outLinkData, outLinkData.Length);

        return true;
    }
}
