using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;

public class SimpleGraphWindow : EditorWindow
{
    private SimpleGraphView m_graphView = null;
    private string m_graphFileName = string.Empty;

    [MenuItem("Window/Graph/SimpleGraphWindow")]
    public static void ShowWinodw()
    {
        SimpleGraphWindow wnd = GetWindow<SimpleGraphWindow>();
        wnd.titleContent = new GUIContent("Simple Graph Window");
    }

    private void CreateGraph()
    {
        m_graphView = new SimpleGraphView();
        m_graphView.name = "simple graph view";
        m_graphView.StretchToParentSize();
        m_graphView.SetEnabled(true);

        this.rootVisualElement.Add(m_graphView);
    }

    private void CreateToolBar()
    {
        Toolbar toolbar = new Toolbar();

        Button nodeCreateButton = new Button(() =>
        {
            // Debug.Log("create button test");
            m_graphView.CreateNormalNode();
        });
        nodeCreateButton.text = "create node";
        toolbar.Add(nodeCreateButton);

        Button branchNodeCreateButton = new Button(() =>
        {
            // Debug.Log("create branch button test");
            m_graphView.CreateBrunchNode();
        });
        branchNodeCreateButton.text = "create branch node";
        toolbar.Add(branchNodeCreateButton);

        TextField nameTextField = new TextField();
        nameTextField.SetValueWithoutNotify(m_graphFileName);
        nameTextField.MarkDirtyRepaint();
        nameTextField.RegisterValueChangedCallback<string>(eventData =>
        {
            m_graphFileName = eventData.newValue;
        });
        toolbar.Add(nameTextField);

        Button saveGraphButton = new Button(() =>
        {
            SaveGraphData();
        });
        saveGraphButton.text = "save data";
        toolbar.Add(saveGraphButton);

        Button loadGraphButton = new Button(() =>
        {
            LoadGraphData();
        });
        loadGraphButton.text = "load data";
        toolbar.Add(loadGraphButton);

        this.rootVisualElement.Add(toolbar);
    }

    private void SaveGraphData()
    {
        Debug.Log("save data");
        if (string.IsNullOrEmpty(m_graphFileName))
        {
            EditorUtility.DisplayDialog("error", "file name is empty", "ok");
            return;
        }

        GraphSaveUtil.TrySaveGraphData(m_graphFileName, m_graphView.nodes.ToList(), m_graphView.edges.ToList());
    }

    private void LoadGraphData()
    {
        // Debug.Log("load data");
        if (GraphSaveUtil.TryLoadGraphData(out NodeData[] outNodeData, out NodeLinkData[] outLinkData))
        {
            // clear and create a new graph for those data
            m_graphView.InjectGraphData(outNodeData, outLinkData);
        }
    }

    private void Awake()
    {
        m_graphFileName = "new graph file";
    }

    private void OnEnable()
    {
        CreateGraph();
        CreateToolBar();
    }

    private void OnDisable()
    {
        m_graphView.SetEnabled(false);
        this.rootVisualElement.Remove(m_graphView);
    }

}