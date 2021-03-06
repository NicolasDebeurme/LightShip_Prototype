using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StoryTreeEditor : EditorWindow
{
    private List<Node> nodes;
    private List<Connection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;

    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 drag;
    private Vector2 offset;

    private string treeName = "DefaultTree";

    //dropDownButton
    GenericMenu menu = new GenericMenu();
    private Serialized_Tree[] _availableTrees;
    //

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        StoryTreeEditor window = GetWindow<StoryTreeEditor>();
        window.titleContent = new GUIContent("StoryTree Editor");
    }

    private void OnEnable()
    {
        nodes = null;
        connections = null;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);


        LoadTreesFromAssets();

        foreach (var tree in _availableTrees)
        {
            if (tree.name == treeName && tree != null)
            {
                OnLoad(tree);
            }
        }
    }

    private void OnDisable()
    {
        OnSave();
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (EditorGUILayout.DropdownButton( new GUIContent(treeName), FocusType.Keyboard))
        {
            foreach (var item in _availableTrees)
            {
                menu.AddItem(new GUIContent(item.name), false, handleItemClicked, item);
            }
            Rect myRectPos = GUILayoutUtility.GetLastRect();
            menu.DropDown(myRectPos);
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("SAVE"))
        {
            OnSave();
        }

        if (GUILayout.Button("RESET"))
        {
            OnReset();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        treeName = EditorGUILayout.TextField("Tree name:",treeName);

        EditorGUILayout.Space();

        if (GUILayout.Button("DELETE TREE"))
        {
            OnDeleteTree();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (GUI.changed) Repaint();
        LoadTreesFromAssets();
    }

    private void handleItemClicked(object parameter)
    {
        var data = parameter as Serialized_Tree;

        treeName = data.name;
        OnLoad(data);
    }
    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawNodes()
    {
        if(nodes != null)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
            nodes.Add(new Node(mousePosition, 200, 200, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }
        else
            nodes.Add(new Node(mousePosition, 200, 200, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node && IsConnectionPointFree(selectedInPoint, selectedOutPoint))
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node && IsConnectionPointFree(selectedInPoint,selectedOutPoint))
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private bool IsConnectionPointFree(ConnectionPoint nodeInPoint, ConnectionPoint nodeOutPoint)
    {
        if (connections != null)
        {
            foreach (var connection in connections)
            {
                if (connection.inPoint == nodeInPoint || connection.outPoint == nodeOutPoint)
                    return false;
            }
            return true;
        }
        else
            return true;
    }
    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || node.outPoints.Contains(connections[i].outPoint))
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);

        if (nodes.Count == 0)
            nodes = null;
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void LoadTree(NTree<StoryTreeNodeInfo> treeNode, int depth, int siblingCount)
    {
        StoryTreeEditor window = GetWindow<StoryTreeEditor>();
        Vector2 nodePosition= window.position.center;

        if (nodes == null)
        {
            nodes = new List<Node>();
        }
        else
        {
            nodePosition = new Vector2(window.position.center.x+300*depth, window.position.center.y+300*(siblingCount));
        }


        Node newNode = new Node(nodePosition, 200, 200, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        newNode.content.LoadContent(treeNode.data.place,treeNode.data.dialogueText,treeNode.data.textToBeChose,treeNode.data.visibilitys, treeNode.data.action);
        nodes.Add(newNode);

        if(nodes.Count > 1) // notRootNode
        {
            selectedInPoint = newNode.inPoint;
            CreateConnection();
        }

        int counter = 0;
        if(treeNode.children != null)
        {
            foreach (var child in treeNode.children)
            {
                selectedOutPoint = newNode.outPoints[counter];
                LoadTree(child, depth + 1, counter + siblingCount);
                counter++;
            }
        }

    }

    private NTree<StoryTreeNodeInfo> SaveTree(NTree<StoryTreeNodeInfo> treeNode , Node actualNode)
    {
        List<Connection> nodeConnections = new List<Connection>();
        if (connections != null)
                foreach(var connection in connections)
                {
                    if(actualNode.outPoints.Contains(connection.outPoint))
                    {
                        nodeConnections.Add(connection);
                    }
                }

        if (nodeConnections != null)
            {
                foreach (var node in nodes)
                {
                    foreach(var connection in nodeConnections)
                    {
                        if(connection.inPoint == node.inPoint)
                        {
                            NTree<StoryTreeNodeInfo> Nnode = treeNode.AddChild();
                            Nnode.data = new StoryTreeNodeInfo(node.content.question, node.content.placeName, node.content.textToBeChose, node.content.visibilitys, node.content.action);
                            SaveTree(Nnode, node);
                        }
                    }
                        
                }
            }

        return treeNode;
    }


    private void OnSave()
    {
        bool isNew = false;

        if (nodes != null && treeName.Length > 0 )
        {
            Serialized_Tree oui = AssetDatabase.LoadAssetAtPath<Serialized_Tree>("Assets/Resources/StoryTree/" + treeName + ".asset")  ;

            if(oui == null)
            {
                isNew = true;
                oui = CreateInstance<Serialized_Tree>();
            }    

            NTree <StoryTreeNodeInfo> treeRoot= new NTree<StoryTreeNodeInfo>();
            treeRoot.data = new StoryTreeNodeInfo(nodes[0].content.question, nodes[0].content.placeName, nodes[0].content.textToBeChose, nodes[0].content.visibilitys, nodes[0].content.action);
            oui.root = treeRoot;
            SaveTree(treeRoot, nodes[0]);

            if(isNew)
            {
                AssetDatabase.CreateAsset(oui, "Assets/Resources/StoryTree/" + treeName + ".asset");
            }

            oui.OnBeforeSerialize();
            EditorUtility.SetDirty(oui);

            AssetDatabase.SaveAssets();

            LoadTreesFromAssets();
        }
    }
    private void OnReset()
    {
        nodes = null;
        connections = null;
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    private void OnDeleteTree()
    {
        AssetDatabase.DeleteAsset("Assets/Resources/StoryTree/" + treeName + ".asset");
        treeName = _availableTrees[0].name;

        if (_availableTrees[0].root.data != null)
            OnLoad(_availableTrees[0]);
    }
    private void OnLoad(Serialized_Tree treeToLoad)
    {
        nodes = null;
        connections = null;

        treeToLoad.OnBeforeSerialize();
        treeToLoad.OnAfterDeserialize();

        if (treeToLoad.root.data != null)
        {
            LoadTree(treeToLoad.root, 0, 0); 
        }
            

        selectedInPoint = null;
        selectedOutPoint = null;
    }
    public void LoadTreesFromAssets()
    {
        _availableTrees = null;
        _availableTrees = UnityEngine.Resources.LoadAll<Serialized_Tree>("StoryTree");
    }

}
