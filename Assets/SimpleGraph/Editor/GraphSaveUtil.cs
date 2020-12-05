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
        List<NodeData> nodeDatas = new List<NodeData>();
        NodeData nodeData = null;
        SimpleNode testNode = null;
        for (int i = 0; i < graphNodes.Count; i++)
        {
            nodeData = new NodeData();
            testNode = graphNodes[i] as SimpleNode;
            nodeData.NodeGuidString = testNode.m_nodeGuid.ToString();
            nodeData.NodeDataString = testNode.m_guidShortStr;
            nodeData.NodePosition = testNode.GetPosition().position;
            nodeData.NodeType = (int)testNode.NodeType;

            nodeDatas.Add(nodeData);
        }

        List<NodeLinkData> nodeLinkDatas = new List<NodeLinkData>();
        NodeLinkData linkData = null;
        SimpleNode inputNode = null;
        SimpleNode outputNode = null;
        for (int i = 0; i < graphLinks.Count; i++)
        {
            linkData = new NodeLinkData();
            inputNode = graphLinks[i].input.node as SimpleNode;
            linkData.InputPortName = graphLinks[i].input.name;
            linkData.InputNodeGuidString = inputNode.m_nodeGuid.ToString();
            int inputNodeIndex = inputNode.m_inputPorts.FindIndex(port => port == graphLinks[i].input);
            linkData.InputPortIndex = inputNodeIndex;

            outputNode = graphLinks[i].output.node as SimpleNode;
            linkData.OutputNodeGuidString = outputNode.m_nodeGuid.ToString();
            linkData.OutputPortName = graphLinks[i].output.name;
            int outputNodeIndex = outputNode.m_outputPorts.FindIndex(port => port == graphLinks[i].output);
            linkData.OutputPortIndex = outputNodeIndex;

            nodeLinkDatas.Add(linkData);
        }

        GraphDataContainer container = new GraphDataContainer();
        GraphData graphData = new GraphData();
        graphData.NodeDatas = nodeDatas.ToArray();
        graphData.NodeLinks = nodeLinkDatas.ToArray();
        container.GraphData = graphData;

        string path = EditorUtility.SaveFilePanel("title", SAVE_FOLDER_PATH, fileName, "asset");
        int cutIndex = path.IndexOf("Assets/");
        string assetsPath = path.Substring(cutIndex);
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(container, assetsPath);
            return true;
        }

        return false;
    }

    public static bool TryLoadGraphData(out NodeData[] outNodeData, out NodeLinkData[] outLinkData)
    {
        outNodeData = null;
        outLinkData = null;

        string path = EditorUtility.OpenFilePanel("title", SAVE_FOLDER_PATH, "asset");
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.Log($"path empty");
            return false;
        }

        // Debug.Log($"select graph data {path}");
        int cutIndex = path.IndexOf("Assets/");
        string assetsPath = path.Substring(cutIndex);
        GraphDataContainer dataContainer = AssetDatabase.LoadAssetAtPath<GraphDataContainer>(assetsPath);
        if (dataContainer == null)
        {
            Debug.Log($"load data fail");
            return false;
        }

        NodeData[] nodeDatas = dataContainer.GraphData.NodeDatas;
        NodeLinkData[] nodeLInks = dataContainer.GraphData.NodeLinks;
        outNodeData = new NodeData[nodeDatas.Length];
        outLinkData = new NodeLinkData[nodeLInks.Length];
        System.Array.Copy(nodeDatas, outNodeData, outNodeData.Length);
        System.Array.Copy(nodeLInks, outLinkData, outLinkData.Length);

        return true;
    }
}
