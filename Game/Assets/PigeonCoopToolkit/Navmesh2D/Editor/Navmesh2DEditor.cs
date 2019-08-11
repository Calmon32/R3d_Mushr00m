using System;
using System.Linq;
using PigeonCoopToolkit.Generic;
using PigeonCoopToolkit.Generic.Editor;
using PigeonCoopToolkit.Utillities;
using UnityEditor;
using UnityEngine;
using ClipperLib;
using Poly2Tri;
using System.Collections.Generic;

namespace PigeonCoopToolkit.Navmesh2D.Editor
{
    public class Navmesh2DEditor : VersionedEditorWindow
    {
        public static Color NavMeshBaseColor = new Color(92 / 255f, 147 / 255f, 205 / 255f, 255 / 255f);
        public static Color NavMeshBaseSelectedColor = new Color(80 / 255f, 228 / 255f, 233 / 255f, 255 / 255f);

        public NavMesh2DBehaviour.NavmeshGenerationInformation GenerationInformation;

        public bool _showNavmesh, _showNavmeshWire, _showQuadTree, _pathTester;
        public Color _drawColor;
        public Vector2 _pathTesterStart, _pathTesterEnd;

        private NavMesh2DBehaviour _sceneNavmesh;
        private Material _navMeshPreviewMat;

        public GUIStyle LightBackgroundStyle;
        public GUIStyle DarkBackgroundStyle;
        public GUIStyle NoBackgroundBorderStyle;
        public Vector2 _scrollBarPos;

        public Texture2D NavMesh2DLogo;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Pigeon Coop Toolkit/Navmesh 2D Generator", false, 1)]
        static void Init() 
        {
            // Get existing open window or if none, make a new one:
            Navmesh2DEditor win = (Navmesh2DEditor)GetWindow(typeof(Navmesh2DEditor), false, "Navigation 2D");
            win.minSize = new Vector2(500, 650);

            win.NavMesh2DLogo = Resources.Load<Texture2D>("PCTK/NM2DG/NavMesh2DLogo");
            win.LightBackgroundStyle = new GUIStyle(EditorStyles.largeLabel);
            win.LightBackgroundStyle.padding = new RectOffset(15, 25, 25, 25);
            win.LightBackgroundStyle.margin = new RectOffset();
            win.LightBackgroundStyle.border = new RectOffset(0, 0, 0, 1);
            win.LightBackgroundStyle.normal.background = Resources.Load<Texture2D>("PCTK/Generic/LightBackground"); ;

            win.DarkBackgroundStyle = new GUIStyle(EditorStyles.largeLabel);
            win.DarkBackgroundStyle.padding = new RectOffset(15, 25, 25, 10);
            win.DarkBackgroundStyle.margin = new RectOffset();
            win.DarkBackgroundStyle.border = new RectOffset(0, 0, 1, 0);
            win.DarkBackgroundStyle.normal.background = Resources.Load<Texture2D>("PCTK/Generic/DarkBackground"); ;

            win.NoBackgroundBorderStyle = new GUIStyle(EditorStyles.largeLabel);
            win.NoBackgroundBorderStyle.padding = new RectOffset(15, 15, 20, 20);
            win.NoBackgroundBorderStyle.margin = new RectOffset();
            win.NoBackgroundBorderStyle.border = new RectOffset(0, 0, 0, 1);
            win.NoBackgroundBorderStyle.normal.background = Resources.Load<Texture2D>("PCTK/Generic/JustBorder"); ;

            win._scrollBarPos = Vector2.zero;
        }

        void Update()
        {
            RefreshReferences();
        }

        void OnGUI()
        {
            if(NavMesh2DLogo == null || LightBackgroundStyle ==  null || DarkBackgroundStyle == null || NoBackgroundBorderStyle == null)
            {
                return;
            }

            if (EditorApplication.isPlaying)
            {
                GUI.enabled = false;
            }


            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
           

            if(GUILayout.Button("Reset to default",EditorStyles.toolbarButton))
            {
                EditorPrefs.DeleteKey("PCTK_NM2DG_ShowNavMesh");
                EditorPrefs.DeleteKey("PCTK_NM2DG_ShowNavMeshWire");
                EditorPrefs.DeleteKey("PCTK_NM2DG_ShowQuadTree");
                EditorPrefs.DeleteKey("PCTK_NM2DG_ShowPathTester");
                EditorPrefs.DeleteKey("PCTK_NM2DG_DrawColor");
                EditorPrefs.DeleteKey("PCTK_NM2DG_EnableGrid");
                EditorPrefs.DeleteKey("PCTK_NM2DG_GridSize");
                EditorPrefs.DeleteKey("PCTK_NM2DG_CircleColliderSubdivisionFactor");
                EditorPrefs.DeleteKey("PCTK_NM2DG_CalculationScaleFactor");
                EditorPrefs.DeleteKey("PCTK_NM2DG_ColliderPadding");
                EditorPrefs.DeleteKey("PCTK_NM2DG_WalkableColliderLayer");
                EditorPrefs.DeleteKey("PCTK_NM2DG_ObstructionColliderLayer");
                RefreshReferences();
            }
            if (GUILayout.Button("About", EditorStyles.toolbarButton))
            {
                ShowAbout(NavMesh2DLogo, Application.dataPath + "/PigeonCoopToolkit/__Navmesh2D Examples/Pigeon Coop Toolkit - NavMesh2D.pdf");
            }
            EditorGUILayout.EndHorizontal();

            _scrollBarPos = EditorGUILayout.BeginScrollView(_scrollBarPos);

            EditorGUILayout.BeginHorizontal(LightBackgroundStyle,GUILayout.Height(60));
            EditorGUILayout.LabelField("");
            GUI.color = new Color(_drawColor.r, _drawColor.g, _drawColor.b,1);
            if(NavMesh2DLogo != null)
                GUI.DrawTexture(new Rect(15, 18, 276, 40), NavMesh2DLogo);
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            Color newColor = EditorGUILayout.ColorField(_drawColor,GUILayout.Width(60));
            if (newColor != _drawColor)
            {
                _drawColor = newColor;
                EditorPrefs.SetString("PCTK_NM2DG_DrawColor", _drawColor.r + "," + _drawColor.g + "," + _drawColor.b + "," + _drawColor.a);
            }

            if(GUILayout.Button("Restore",EditorStyles.miniButton))
            {
                EditorPrefs.DeleteKey("PCTK_NM2DG_DrawColor");    
                RefreshReferences();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(NoBackgroundBorderStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layer settings", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.largeLabel);

            EditorGUILayout.BeginVertical(EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Walkable Layer", EditorStyles.boldLabel);

            string newWalkable = GUI_DoLayerList(GenerationInformation.WalkableColliderLayer);
            if (newWalkable != GenerationInformation.WalkableColliderLayer)
            {
                GenerationInformation.WalkableColliderLayer = newWalkable;
                EditorPrefs.SetString("PCTK_NM2DG_WalkableColliderLayer", GenerationInformation.WalkableColliderLayer);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Obstruction Layer", EditorStyles.boldLabel);

            string newObstruction = GUI_DoLayerList(GenerationInformation.ObstructionColliderLayer);
            if (newObstruction != GenerationInformation.ObstructionColliderLayer)
            {
                GenerationInformation.ObstructionColliderLayer = newObstruction;
                EditorPrefs.SetString("PCTK_NM2DG_ObstructionColliderLayer", GenerationInformation.ObstructionColliderLayer);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(NoBackgroundBorderStyle);


            EditorGUILayout.BeginHorizontal(EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.largeLabel);

            float subdivisionFactor = EditorGUILayout.Slider("Circle Subdivision Factor:",
                                                             GenerationInformation.CircleColliderSubdivisionFactor, 0.01f, 5f);
            if (Math.Abs(subdivisionFactor - GenerationInformation.CircleColliderSubdivisionFactor) > Mathf.Epsilon)
            {
                EditorPrefs.SetFloat("PCTK_NM2DG_CircleColliderSubdivisionFactor", subdivisionFactor);
                GenerationInformation.CircleColliderSubdivisionFactor = subdivisionFactor;
            }

            int floatAccuracy = EditorGUILayout.IntSlider("Float Precision:",
                                                             Mathf.RoundToInt(Mathf.Log10(GenerationInformation.CalculationScaleFactor)), 1, 9);
            if (floatAccuracy != Mathf.RoundToInt(Mathf.Log10(GenerationInformation.CalculationScaleFactor)))
            {
                GenerationInformation.CircleColliderSubdivisionFactor = Mathf.RoundToInt(Mathf.Pow(10, floatAccuracy));
                EditorPrefs.SetFloat("PCTK_NM2DG_CalculationScaleFactor", GenerationInformation.CircleColliderSubdivisionFactor);
            }

            float padding = EditorGUILayout.Slider("Obstruction Padding:",
                                                             GenerationInformation.ColliderPadding, 0, 3f);
            if (Math.Abs(padding - GenerationInformation.ColliderPadding) > Mathf.Epsilon)
            {
                EditorPrefs.SetFloat("PCTK_NM2DG_ColliderPadding", padding);
                GenerationInformation.ColliderPadding = padding;
            }

            JoinType jt = (JoinType)EditorGUILayout.EnumPopup("Join Type:", GenerationInformation.JoinType);
            if (jt != (JoinType)GenerationInformation.JoinType)
            {
                EditorPrefs.SetInt("PCTK_NM2DG_JoinType", (int)jt);
                GenerationInformation.JoinType = (global::PigeonCoopToolkit.Navmesh2D.NavMesh2DBehaviour.JoinType)jt;
            }

            bool dg = EditorGUILayout.Toggle("Bake Grid: ", GenerationInformation.UseGrid);
            if (dg != GenerationInformation.UseGrid)
            {
                GenerationInformation.UseGrid = dg;
                EditorPrefs.SetBool("PCTK_NM2DG_EnableGrid",GenerationInformation.UseGrid);
            }

            if (GenerationInformation.UseGrid)
            {
                Vector2 newGridSize = EditorGUILayout.Vector2Field("Grid Size: ", GenerationInformation.GridSize);
                newGridSize.x = Mathf.Clamp(newGridSize.x,0.25f, float.MaxValue);
                newGridSize.y = Mathf.Clamp(newGridSize.y, 0.25f, float.MaxValue);
                if (newGridSize != GenerationInformation.GridSize)
                {
                    GenerationInformation.GridSize = newGridSize;
                    EditorPrefs.SetString("PCTK_NM2DG_GridSize", string.Format("{0},{1}", GenerationInformation.GridSize.x, GenerationInformation.GridSize.y));

                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            
            

            if(_sceneNavmesh != null)
            {
                EditorGUILayout.BeginVertical(DarkBackgroundStyle);
                EditorGUILayout.BeginHorizontal(EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField("Current NavMesh2D Properties", EditorStyles.whiteMiniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Walkable Layer: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.WalkableColliderLayer, EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Obstruction Layer: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.ObstructionColliderLayer, EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Circle Subdivision Factor: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.CircleColliderSubdivisionFactor.ToString(), EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Float Precision: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(Mathf.Log10(_sceneNavmesh.GenerationInformation.CalculationScaleFactor).ToString(), EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Obstruction Padding: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.ColliderPadding.ToString(), EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Join Type: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.JoinType.ToString(), EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Baked Grid: ", EditorStyles.miniLabel);
                EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.UseGrid ? "Yes" : "No", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                if (_sceneNavmesh.GenerationInformation.UseGrid)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Grid Size: ", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField(_sceneNavmesh.GenerationInformation.GridSize.ToString(), EditorStyles.miniLabel);
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.EndVertical();
            }
            

            GUILayout.BeginHorizontal(EditorStyles.largeLabel);
            if (_sceneNavmesh != null)
            {
                if (GUILayout.Button("Clear",GUILayout.MinWidth(100)))
                {
                    if(EditorUtility.DisplayDialog("Are you sure?","You will have to rebake the NavMesh if you want to use 2D Navigation","Yes","No"))
                    {
                        DestroySceneNavMesh();
                    }
                }

                if (GUILayout.Button("Bake",GUILayout.MinWidth(100)))
                {
                    RunGeneration();
                }
            }
            else
            {
                if (GUILayout.Button("Bake",GUILayout.MinWidth(100)))
                {
                    RunGeneration();
                }
            }
            GUILayout.EndHorizontal();



            EditorGUILayout.EndScrollView();
            
        }

        string GUI_DoLayerList(string currentLayer)
        {
            string newLayerName = currentLayer;

            bool prevStatus = newLayerName == "";

            GUILayout.BeginHorizontal();
            bool currStatus = EditorGUILayout.Toggle("Nothing", prevStatus, EditorStyles.toggle);
            GUILayout.EndHorizontal();

            if (prevStatus != currStatus && prevStatus == false)
            {
                newLayerName = "";
            }

            for (int i = 0; i < 32; i++)
            {

                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    prevStatus = currentLayer == layerName;
                    GUILayout.BeginHorizontal();
                    currStatus = EditorGUILayout.Toggle(layerName, prevStatus, EditorStyles.toggle);
                    GUILayout.EndHorizontal();
                    if (prevStatus != currStatus)
                    {
                        if (prevStatus == false)
                        {
                            newLayerName = layerName;
                        }
                        else
                        {
                            newLayerName = "";
                        }
                    }
                }
            }

            return newLayerName;
        }

        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            RefreshReferences();

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.RepaintAll();
        }

        void OnSceneGUI(SceneView view)
        {

            if (_showNavmesh && _sceneNavmesh && _sceneNavmesh.NavMesh && Event.current.type == EventType.Repaint)
            {
                _navMeshPreviewMat.color = _drawColor;
                _navMeshPreviewMat.SetPass(0);
                Graphics.DrawMeshNow(_sceneNavmesh.NavMesh, Matrix4x4.identity);
                if(_showNavmeshWire)
                {
                    GL.wireframe = true;
                    Graphics.DrawMeshNow(_sceneNavmesh.NavMesh, Matrix4x4.identity);
                    GL.wireframe = false;
                
                }
                
                foreach (NavMesh2DBehaviour.MeshContour contour in _sceneNavmesh.Contours)
                {
                    DrawPointListLine(contour.Points, Color.white, Matrix4x4.identity, true);
                }
                
            }

            if (_showQuadTree && _sceneNavmesh != null && _sceneNavmesh.NodeNodeIndexQuadTree != null && _sceneNavmesh.NodeNodeIndexQuadTree.NodeIndexQuadTreeNode != null && Event.current.type == EventType.Repaint)
            {
                DrawQuadTreeNodeRecursive(_sceneNavmesh.NodeNodeIndexQuadTree.NodeIndexQuadTreeNode);
            }

            if(_pathTester && _sceneNavmesh != null)
            {
                Handles.color = Color.green;
                _pathTesterStart = Handles.FreeMoveHandle(_pathTesterStart, Quaternion.identity, 0.1f, Vector3.zero, Handles.CircleCap);
                Handles.color = Color.red;
                _pathTesterEnd = Handles.FreeMoveHandle(_pathTesterEnd, Quaternion.identity, 0.1f, Vector3.zero, Handles.CircleCap);

                if(Event.current.type == EventType.Repaint)
                {
                    List<Vector2> PathFound = NavMesh2D.GetSmoothedPath(_pathTesterStart, _pathTesterEnd);

                    DrawPointListLine(PathFound.ToArray(), Color.white, Matrix4x4.identity, false);
                }
               
            }

            Handles.BeginGUI();
            // All GUI.Window or GUILayout.Window must come inside here
            Rect winRec = new Rect(view.position.width - 200, 40, 0, 0);
            GUILayout.Window(198123, winRec, ToggleNavmeshDisplayWindow, "Navmesh 2D Display");
            Handles.EndGUI();
        }

        void ToggleNavmeshDisplayWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Show NavMesh 2D", GUILayout.Width(140));

            bool newShowNavmesh = GUILayout.Toggle(_showNavmesh, "", GUILayout.Width(20));
            if (newShowNavmesh != _showNavmesh)
            {
                _showNavmesh = newShowNavmesh;
                EditorPrefs.SetBool("PCTK_NM2DG_ShowNavMesh", _showNavmesh);
            }
            GUILayout.EndHorizontal();

            if(_showNavmesh)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Show Wire", GUILayout.Width(140));

                bool showWire = GUILayout.Toggle(_showNavmeshWire, "", GUILayout.Width(20));
                if (showWire != _showNavmeshWire)
                {
                    _showNavmeshWire = showWire;
                    EditorPrefs.SetBool("PCTK_NM2DG_ShowNavMeshWire", _showNavmeshWire);
                }
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();

            GUILayout.Label("Show Quad Tree", GUILayout.Width(140));
            bool newShowQuadTree = GUILayout.Toggle(_showQuadTree, "", GUILayout.Width(20));
            if (newShowQuadTree != _showQuadTree)
            {
                _showQuadTree = newShowQuadTree;
                EditorPrefs.SetBool("PCTK_NM2DG_ShowQuadTree", _showQuadTree);
            }

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label("Path Tester", GUILayout.Width(140));
            bool newpter = GUILayout.Toggle(_pathTester, "", GUILayout.Width(20));
            if (newpter != _pathTester)
            {
                _pathTesterStart = Vector2.right;
                _pathTesterEnd = -Vector2.right;
                _pathTester = newpter;
                EditorPrefs.SetBool("PCTK_NM2DG_ShowPathTester", _pathTester);
            }

            GUILayout.EndHorizontal();

            
        }

        void RefreshReferences()
        {
            _showNavmesh = EditorPrefs.GetBool("PCTK_NM2DG_ShowNavMesh", true); 
            _showNavmeshWire = EditorPrefs.GetBool("PCTK_NM2DG_ShowNavMeshWire", false);
            _showQuadTree = EditorPrefs.GetBool("PCTK_NM2DG_ShowQuadTree", false);
            _pathTester = EditorPrefs.GetBool("PCTK_NM2DG_ShowPathTester", false);
            string[] gridDrawColor = EditorPrefs.GetString("PCTK_NM2DG_DrawColor", "0,0.627,1,0.5").Split(',');
            _drawColor = new Color(float.Parse(gridDrawColor[0]), float.Parse(gridDrawColor[1]),
                                   float.Parse(gridDrawColor[2]), float.Parse(gridDrawColor[3]));

            string[] gridSizeString = EditorPrefs.GetString("PCTK_NM2DG_GridSize", "1.5,1.5").Split(',');
            GenerationInformation = new NavMesh2DBehaviour.NavmeshGenerationInformation
            {
                CalculationScaleFactor = EditorPrefs.GetFloat("PCTK_NM2DG_CalculationScaleFactor", 100f),
                CircleColliderSubdivisionFactor = EditorPrefs.GetFloat("PCTK_NM2DG_CircleColliderSubdivisionFactor", 0.25f),
                ColliderPadding = EditorPrefs.GetFloat("PCTK_NM2DG_ColliderPadding", 0.1f),
                UseGrid = EditorPrefs.GetBool("PCTK_NM2DG_EnableGrid", true),
                GridSize =  new Vector2(float.Parse(gridSizeString[0]), float.Parse(gridSizeString[1])),
                JoinType = (global::PigeonCoopToolkit.Navmesh2D.NavMesh2DBehaviour.JoinType)EditorPrefs.GetInt("PCTK_NM2DG_JoinType", (int)JoinType.jtSquare),
                ObstructionColliderLayer = EditorPrefs.GetString("PCTK_NM2DG_ObstructionColliderLayer", ""),
                WalkableColliderLayer = EditorPrefs.GetString("PCTK_NM2DG_WalkableColliderLayer", ""),
            };

            _navMeshPreviewMat = Resources.Load<Material>("PCTK/NM2DG/NavmeshOverlay");
            
            NavMesh2DBehaviour[] sceneNavMeshes = FindObjectsOfType<NavMesh2DBehaviour>();

            if (sceneNavMeshes.Length > 1)
            {
                for (int i = 0; i < sceneNavMeshes.Length; i++)
                {
                    DestroyImmediate(sceneNavMeshes[i].gameObject);
                }

                _sceneNavmesh = null;
            }
            else if (sceneNavMeshes.Length == 1)
            {
                _sceneNavmesh = sceneNavMeshes[0];

                if (VersionInformation().Match(_sceneNavmesh.Version,true) == false)
                {
                    EditorUtility.DisplayDialog("NavMesh2D Version Mismatch",
                                                string.Format("The 2D navmesh for this scene was built with an older version ({0}) of NavMesh2D, you will have to rebuild it with this version ({1})", _sceneNavmesh.Version, VersionInformation()),"OK");
                    DestroySceneNavMesh();
                }
            }
            else
            {
                _sceneNavmesh = null;
            }

            


            SceneView.RepaintAll();
            Repaint();
        }

        void RunGeneration()
        {
            GameObject tempNavMeshObject = new GameObject("NavMesh2D");
            NavMesh2DBehaviour tempNavMesh = tempNavMeshObject.AddComponent<NavMesh2DBehaviour>();

            try
            {
                EditorUtility.ClearProgressBar();

                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Finding Scene Colliders", 0.05f))
                    throw new System.OperationCanceledException("User canceled.");


                Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
                if (allColliders.Length == 0)
                {
                    throw new System.Exception("No Collider2D's in Scene");
                }

                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Generating and clipping polygons", 0.1f))
                            throw new System.OperationCanceledException("User canceled.");

                PolyTreeEdgesRetained treeEdgesRetained = PrepairPolyTree(
                                                                allColliders.Where(a => a.gameObject.layer == LayerMask.NameToLayer(GenerationInformation.WalkableColliderLayer)).ToArray(),
                                                                allColliders.Where(a => a.gameObject.layer == LayerMask.NameToLayer(GenerationInformation.ObstructionColliderLayer)).ToArray()
                                                            );

                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Extracting visual data from polygons", 0.2f))
                    throw new System.OperationCanceledException("User canceled.");

                tempNavMesh.Contours = ExtractMeshContour(treeEdgesRetained);
                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Triangulating polygons", 0.3f))
                    throw new System.OperationCanceledException("User canceled.");

                List<Polygon> triangulatedPolygons = TriangulatePolyTree(treeEdgesRetained.Tree);
                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Genearting preview mesh", 0.4f))
                    throw new System.OperationCanceledException("User canceled.");
                tempNavMesh.NavMesh = GenerateMesh(triangulatedPolygons);
                if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Generating node network", 0.5f))
                    throw new System.OperationCanceledException("User canceled.");
                tempNavMesh.NavMesh2DNodes = GenerateNodeNetwork(triangulatedPolygons, tempNavMesh.transform);
                EditorUtility.DisplayProgressBar("Generating NavMesh2D", "Finishing up, may take a minute or two", 1f);

                tempNavMesh.NodeNodeIndexQuadTree = tempNavMesh.gameObject.AddComponent<NodeIndexQuadTree>();

                List<NodeIndexQuadTree.VectorIndexPair> QuadTreeObjects = new List<NodeIndexQuadTree.VectorIndexPair>();
                for (int i = 0; i < tempNavMesh.NavMesh2DNodes.Length; i++)
                {
                    QuadTreeObjects.Add(new NodeIndexQuadTree.VectorIndexPair
                                            {Obj = i, Position = tempNavMesh.NavMesh2DNodes[i].position});
                }

                tempNavMesh.NodeNodeIndexQuadTree.SetIndicies(QuadTreeObjects.ToArray());

                tempNavMesh.NodeNodeIndexQuadTree.GenerateQuadTree(GenerationInformation.ObstructionColliderLayer,6);

                tempNavMesh.GenerationInformation = GenerationInformation;

                tempNavMesh.Version = VersionInformation();

                EditorUtility.SetDirty(tempNavMesh);
                EditorUtility.SetDirty(tempNavMesh.NodeNodeIndexQuadTree);

                foreach (var l in FindObjectsOfType<OffMeshLink2D>())
                {
                    l.ForceReset();
                }
                DestroySceneNavMesh();
                _sceneNavmesh = tempNavMesh;
                
                SceneView.RepaintAll();
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayProgressBar("Generating NavMesh2D", "Cleaning up, discarding all my hard work",1);
                CleanNavMesh(tempNavMesh);
                DestroyImmediate(tempNavMeshObject);
                EditorUtility.ClearProgressBar();
                if (e is OperationCanceledException == false)
                {
                    if(e.Source == "Poly2Tri")
                    {
                        EditorUtility.DisplayDialog("Error", "Your mesh is too dense, try a larger padding, larger grid or pick Miter/Square as your join type instead of Round" , "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", e.Message, "OK");
                    }
                }
                
            }
        }


        private void CleanNavMesh(NavMesh2DBehaviour navMesh)
        {
            if (navMesh.NavMesh != null)
                DestroyImmediate(navMesh.NavMesh);

            if (navMesh.NavMesh2DNodes != null && navMesh.NavMesh2DNodes.Length != 0)
            {
                navMesh.NavMesh2DNodes = null;
            }

            if(navMesh.NodeNodeIndexQuadTree != null)
            {
                DestroyImmediate(navMesh.NodeNodeIndexQuadTree);
            }
            navMesh.NavMesh2DNodes = null;
            navMesh.Contours = null;
        }

        private void DestroySceneNavMesh()
        {
            if (_sceneNavmesh)
            {
                CleanNavMesh(_sceneNavmesh);
                DestroyImmediate(_sceneNavmesh.gameObject);
            }
        }

        private PolyTreeEdgesRetained PrepairPolyTree(Collider2D[] floorColliders, Collider2D[] wallColliders)
        {
            if(floorColliders.Length == 0)
            {
                throw new System.Exception("No colliders in the scene on the floor layer.");
            }

            PolyTreeEdgesRetained TreeAndEdges = new PolyTreeEdgesRetained();

            TreeAndEdges.Edges = new List<List<IntPoint>>();
            TreeAndEdges.Tree = new PolyTree();

            Clipper finalClipper = new Clipper();

            if (floorColliders.Length > 0)
            {
                Clipper tempC = new Clipper();
                ClipperOffset tempCo = new ClipperOffset();

                foreach (Collider2D d in floorColliders)
                {
                    List<IntPoint> p = PolygonFromCollider2D(d, false);
                    if (p != null && p.Count != 0)
                    {
                        if (ClipperLib.Clipper.Orientation(p))
                            p.Reverse();

                        tempC.AddPath(p, PolyType.ptSubject, true);
                    }
                }

                List<List<IntPoint>> solution = new List<List<IntPoint>>();
                tempC.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
                tempC.Clear();

                foreach (List<IntPoint> intPoints in solution)
                {
                    tempCo.AddPath(intPoints, (JoinType)GenerationInformation.JoinType, EndType.etClosedPolygon);
                    TreeAndEdges.Edges.Add(intPoints);
                }
                solution.Clear();
                tempCo.Execute(ref solution, -GenerationInformation.ColliderPadding * GenerationInformation.CalculationScaleFactor);
                finalClipper.AddPaths(solution, PolyType.ptSubject, true);
            }

            if(wallColliders.Length > 0)
            {
                Clipper tempC = new Clipper();
                ClipperOffset tempCo = new ClipperOffset();

                foreach (Collider2D d in wallColliders)
                {
                    List<IntPoint> p = PolygonFromCollider2D(d, false);
                    if (p != null && p.Count != 0)
                    {
                        if (ClipperLib.Clipper.Orientation(p))
                            p.Reverse();

                        tempC.AddPath(p, PolyType.ptSubject, true);
                    }
                }

                List<List<IntPoint>> solution = new List<List<IntPoint>>();
                tempC.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
                tempC.Clear();

                foreach (List<IntPoint> intPoints in solution)
                {
                    tempCo.AddPath(intPoints, (JoinType)GenerationInformation.JoinType, EndType.etClosedPolygon);
                    TreeAndEdges.Edges.Add(intPoints);
                }
                solution.Clear();

                tempCo.Execute(ref solution, GenerationInformation.ColliderPadding * GenerationInformation.CalculationScaleFactor);
                finalClipper.AddPaths(solution, PolyType.ptClip, true);
            }

            finalClipper.Execute(ClipType.ctDifference, TreeAndEdges.Tree, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);


            return TreeAndEdges;
        }

        private NavMesh2DBehaviour.MeshContour[] ExtractMeshContour(PolyTreeEdgesRetained ptcr)
        {
            List<NavMesh2DBehaviour.MeshContour> result = new List<NavMesh2DBehaviour.MeshContour>();

            foreach (PolyNode meshContour in ptcr.Tree.Childs)
            {
                List<Vector2> extractedPoints = new List<Vector2>();

                foreach (var point in meshContour.Contour)
                {
                    extractedPoints.Add(new Vector2(point.X / GenerationInformation.CalculationScaleFactor, point.Y / GenerationInformation.CalculationScaleFactor));
                }

                result.Add(new NavMesh2DBehaviour.MeshContour { Points = extractedPoints.ToArray() });


                foreach (PolyNode holes in meshContour.Childs)
                {
                    extractedPoints.Clear();

                    foreach (var point in holes.Contour)
                    {
                        extractedPoints.Add(new Vector2(point.X / GenerationInformation.CalculationScaleFactor, point.Y / GenerationInformation.CalculationScaleFactor));
                    }
                    result.Add(new NavMesh2DBehaviour.MeshContour { Points = extractedPoints.ToArray() });
                }


            }

            return result.ToArray();
        }

        private List<IntPoint> PolygonFromCollider2D(Collider2D c, bool addPadding)
        {
            List<Vector2> polygon = new List<Vector2>();
            List<IntPoint> polygonInt = new List<IntPoint>();

            if (c is CircleCollider2D)
            {
                CircleCollider2D circleCollider = c as CircleCollider2D;
                float radius = circleCollider.radius;

                float circumference = (2 * Mathf.PI * radius);
                int numSegments = Mathf.FloorToInt(circumference / GenerationInformation.CircleColliderSubdivisionFactor * circleCollider.transform.lossyScale.y);
                float segmentLength = circumference / numSegments;

                float leftOver = circumference - (numSegments * segmentLength);
                segmentLength += leftOver / numSegments;
                float angleStep = 360f / numSegments;
                for (int i = 0; i < numSegments; i++)
                {
                    polygon.Add((Vector2)(Quaternion.Euler(0, 0, angleStep * i) * Vector2.up * radius) + circleCollider.offset);
                }


            }
            else if (c is PolygonCollider2D)
            {
                PolygonCollider2D polygonCollider = c as PolygonCollider2D;
                for (int i = polygonCollider.points.Length - 1; i >= 0; i--)
                {
                    polygon.Add(polygonCollider.points[i]);
                }

            }
            else if (c is BoxCollider2D)
            {
                BoxCollider2D boxCollider = c as BoxCollider2D;

                polygon.Add(new Vector2(-boxCollider.size.x / 2, -boxCollider.size.y / 2) + boxCollider.offset);
                polygon.Add(new Vector2(boxCollider.size.x / 2, -boxCollider.size.y / 2) + boxCollider.offset);
                polygon.Add(new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2) + boxCollider.offset);
                polygon.Add(new Vector2(-boxCollider.size.x / 2, boxCollider.size.y / 2) + boxCollider.offset);
            }
            else
            {
                return null;
            }

            for (int i = 0; i < polygon.Count; i++)
            {
                polygon[i] = c.gameObject.transform.TransformPoint(polygon[i]);
            }

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 vector2 = polygon[i];

                if (addPadding)
                {
                    Vector2 prev = i == 0
                                   ? polygon[polygon.Count - 1]
                                   : polygon[i - 1];

                    prev = prev - vector2;
                    prev = prev.normalized;

                    Vector2 next = i == polygon.Count - 1
                                       ? polygon[0]
                                       : polygon[i + 1];

                    next = next - vector2;
                    next = next.normalized;

                    Vector2 normal = Vector3.Cross((next - prev).normalized, Vector3.forward);

                    vector2 += normal * GenerationInformation.ColliderPadding;
                }

                polygonInt.Add(new IntPoint(Mathf.RoundToInt(vector2.x * GenerationInformation.CalculationScaleFactor),
                                            Mathf.RoundToInt(vector2.y * GenerationInformation.CalculationScaleFactor)));
            }

            return polygonInt;
        }

        private List<Polygon> TriangulatePolyTree(PolyTree tree)
        {
            List<Polygon> polygonsGenerated = new List<Polygon>();
            List<PolygonPoint> AllPoints = new List<PolygonPoint>();
            foreach (PolyNode islands in tree.Childs)
            {
                List<PolygonPoint> floorPoints = new List<PolygonPoint>();
                foreach (IntPoint point in SubdividePolygon(islands.Contour))
                {
                    floorPoints.Add(new PolygonPoint(point.X / GenerationInformation.CalculationScaleFactor, point.Y / GenerationInformation.CalculationScaleFactor));
                }

                AllPoints.AddRange(floorPoints);
                Polygon floorPolygon = new Polygon(floorPoints);

                foreach (PolyNode nextNode in islands.Childs)
                {
                    if (nextNode.IsHole)
                    {
                        List<PolygonPoint> holePoints = new List<PolygonPoint>();
                        foreach (IntPoint point in SubdividePolygon(nextNode.Contour))
                        {
                            holePoints.Add(new PolygonPoint(point.X / GenerationInformation.CalculationScaleFactor, point.Y / GenerationInformation.CalculationScaleFactor));
                        }
                        AllPoints.AddRange(holePoints);
                        floorPolygon.AddHole(new Polygon(holePoints));
                    }
                }
                if (GenerationInformation.UseGrid)
                {
                    List<TriangulationPoint> steinerPoints = new List<TriangulationPoint>();
                    for (float x = (float)floorPolygon.BoundingBox.MinX - ((float)floorPolygon.BoundingBox.MinX % GenerationInformation.GridSize.x); x < floorPolygon.BoundingBox.MaxX; x += GenerationInformation.GridSize.x)
                    {
                        for (float y = (float)floorPolygon.BoundingBox.MinY - ((float)floorPolygon.BoundingBox.MinY % GenerationInformation.GridSize.y); y < floorPolygon.BoundingBox.MaxY; y += GenerationInformation.GridSize.y)
                        {
                            TriangulationPoint p = new TriangulationPoint(x, y);
                            if (floorPolygon.IsPointInside(p))
                                steinerPoints.Add(p);
                        }
                    }

                    CullSteinerPoints(ref steinerPoints, AllPoints, 0.1f);

                    floorPolygon.AddSteinerPoints(steinerPoints);    
                }

                floorPolygon.Prepare(P2T.CreateContext(TriangulationAlgorithm.DTSweep));
                P2T.Triangulate(floorPolygon);
                polygonsGenerated.Add(floorPolygon);
                 
            }


            return polygonsGenerated;

        }

        private void CullSteinerPoints(ref List<TriangulationPoint> steinerPoints, List<PolygonPoint> allPoints, float minDistance)
        {
            for (int index = steinerPoints.Count-1; index >= 0; index--)
            {
                Vector2 steinerVector = new Vector2(steinerPoints[index].Xf, steinerPoints[index].Yf);
                foreach (PolygonPoint polygonPoint in allPoints)
                {
                    if(Vector2.Distance(steinerVector,new Vector2(polygonPoint.Xf,polygonPoint.Yf)) < minDistance)
                    {
                        steinerPoints.RemoveAt(index);
                        break;
                    }
                }
            }
        }

        private List<IntPoint> SubdividePolygon(List<IntPoint> contour)
        {
            if(!GenerationInformation.UseGrid)
            {
                return contour;
            }

            List<IntPoint> subdivedPolygon = new List<IntPoint>();
            for (int i = 0; i < contour.Count; i++)
            {
                int nextI = i == contour.Count - 1
                                         ? 0
                                         : i+1;
                subdivedPolygon.Add(contour[i]);
                IntPoint point = contour[i];
                IntPoint nextPoint = contour[nextI];

                List<IntPoint> subdivPoints = new List<IntPoint>();
                Vector2 scaledGrid = GenerationInformation.GridSize*GenerationInformation.CalculationScaleFactor;
                
                if(point == nextPoint)
                {
                    //duplicate point, collapse it
                    contour.RemoveAt(nextI);
                    i--;
                    continue;
                }

                if(point.X == nextPoint.X)
                {
                    if (point.Y > nextPoint.Y)
                    {
                        point = contour[nextI];
                        nextPoint = contour[i];
                    }

                    float modulo = point.Y % scaledGrid.y;
                    if (point.Y < 0)
                        modulo = scaledGrid.y - modulo;
                    float y = (modulo + point.Y) - scaledGrid.y;

                    while(y < nextPoint.Y)
                    {
                        if(y > point.Y)
                        {
                             IntPoint? newPnt = PointAlongY(y, point, nextPoint);
                        if(newPnt.HasValue)
                            subdivPoints.Add(newPnt.Value);
                        }
                       

                        y += scaledGrid.y;
                    }

                }
                else if(point.Y == nextPoint.Y)
                {
                    if (point.X > nextPoint.X)
                    {
                        point = contour[nextI];
                        nextPoint = contour[i];
                    }

                    float modulo = point.X%scaledGrid.x;
                    if (point.X < 0)
                        modulo = scaledGrid.x - modulo;
                    float x = (modulo + point.X) - scaledGrid.x;

                    while (x < nextPoint.X)
                    {
                        if(x > point.X)
                        {
                            IntPoint? newPnt = PointAlongX(x, point, nextPoint);
                            if (newPnt.HasValue)
                                subdivPoints.Add(newPnt.Value);
                        }
                        x += scaledGrid.x;
                    }
                }
                else
                {
                    if (point.X > nextPoint.X)
                    {
                        point = contour[nextI];
                        nextPoint = contour[i];
                    }

                    float modulo = point.X % scaledGrid.x;
                    if (point.X < 0)
                        modulo = scaledGrid.x - modulo;
                    float x = (modulo + point.X) - scaledGrid.x;

                    while (x < nextPoint.X)
                    {
                        if (x > point.X)
                        {
                            IntPoint? newPnt = PointAlongX(x, point, nextPoint);
                            if (newPnt.HasValue)
                                subdivPoints.Add(newPnt.Value);
                        }
                        x += scaledGrid.x;
                    }

                    point = contour[i];
                    nextPoint = contour[nextI];

                    if (point.Y > nextPoint.Y)
                    {
                        point = contour[nextI];
                        nextPoint = contour[i];
                    }

                    modulo = point.Y % scaledGrid.y;
                    if (point.Y < 0)
                        modulo = scaledGrid.y - modulo;
                    float y = (modulo + point.Y) - scaledGrid.y;

                    while (y < nextPoint.Y)
                    {
                        if (y > point.Y)
                        {
                            IntPoint? newPnt = PointAlongY(y, point, nextPoint);
                            if (newPnt.HasValue)
                                subdivPoints.Add(newPnt.Value);
                        }


                        y += scaledGrid.y;
                    }
                }

                if (subdivPoints.Count > 0)
                {
                    subdivPoints = subdivPoints.OrderBy(a => Vector2.Distance(new Vector2(contour[i].X, contour[i].Y), new Vector2(a.X, a.Y))).ToList();
                    
                    subdivedPolygon.AddRange(subdivPoints);
                }
                
            }


            return subdivedPolygon;
        }

        private IntPoint? PointAlongX(double x, IntPoint start, IntPoint end)
        {
            //m=(y1−y0)/(x1−x0)
            float m = 0;
            if ((end.Y - start.Y) == 0)
            {
                return new IntPoint(x, start.Y);
            }
            else if ((end.X - start.X) == 0)
            {
                return null;
            }
            else
            {
                m = (float)(end.Y - start.Y) / (float)(end.X - start.X);
            }

            //y=m(x−x0)+y0
            return new IntPoint(x, m * (x - start.X) + start.Y);
        }

        private IntPoint? PointAlongY(double y, IntPoint start, IntPoint end)
        {
            float m = 0;
            if ((end.Y - start.Y) == 0)
            {
                return null;
            }
            else if ((end.X - start.X) == 0)
            {
                return new IntPoint(start.X, y);
            }
            else
            {
                m = (float)(end.Y - start.Y) / (float)(end.X - start.X);
            }

            //x = (y-y0)/m + x0
            return new IntPoint(((y - start.Y) / m) + start.X, y);
        }

        private NavMesh2DNode[] GenerateNodeNetwork(List<Polygon> triangulatedPolygon, Transform parent)
        {
            List<NavMesh2DNode> allGeneratedNodes = new List<NavMesh2DNode>();

            int totalTriangles = triangulatedPolygon.Sum(a => a.Triangles.Count)*2;
            int processedTriangles = 0;

            foreach (Polygon navmesh in triangulatedPolygon)
            {
                HashSet<uint> processedVertices = new HashSet<uint>();
                Dictionary<uint, NavMesh2DNode> navNodeToVertexCode = new Dictionary<uint, NavMesh2DNode>();

                foreach (DelaunayTriangle t in navmesh.Triangles)
                {
                    foreach (TriangulationPoint p in t.Points)
                    {
                        if (processedVertices.Contains(p.VertexCode))
                            continue;
                        processedVertices.Add(p.VertexCode);

                        NavMesh2DNode newMesh2DNode = new NavMesh2DNode();
                        newMesh2DNode.position = new Vector3((float)p.X, (float)p.Y, 0);
                        navNodeToVertexCode.Add(p.VertexCode, newMesh2DNode);
                    }

                    processedTriangles++;
                    if(EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Generating node network",
                                                     0.5f + (((float)processedTriangles / (float)totalTriangles) / 2)))
                        throw new System.OperationCanceledException("User canceled.");


                }

                allGeneratedNodes.AddRange(navNodeToVertexCode.Values.ToArray());
                processedVertices.Clear();

                foreach (DelaunayTriangle t in navmesh.Triangles)
                {
                    foreach (TriangulationPoint p in t.Points)
                    {
                        foreach (TriangulationPoint pp in t.Points)
                        {
                            if (pp.VertexCode == p.VertexCode)
                                continue;
                            navNodeToVertexCode[p.VertexCode].ConnectTo(allGeneratedNodes.IndexOf(navNodeToVertexCode[pp.VertexCode]), NavMesh2DConnection.ConnectionType.Standard);
                        }
                        if (processedVertices.Contains(p.VertexCode))
                            continue;

                        processedVertices.Add(p.VertexCode);
                        List<FixedArray3<TriangulationPoint>> connectedTriangles =
                            navmesh.Triangles.Where(a => a != t).Select(a => a.Points).Where(
                                a => a.Any(b => b.VertexCode == p.VertexCode)).ToList();

                        foreach (FixedArray3<TriangulationPoint> point in connectedTriangles)
                        {
                            foreach (TriangulationPoint triangulationPoint in point.Where(a => a.VertexCode != p.VertexCode))
                            {
                                navNodeToVertexCode[p.VertexCode].ConnectTo(allGeneratedNodes.IndexOf(navNodeToVertexCode[triangulationPoint.VertexCode]), NavMesh2DConnection.ConnectionType.Standard);
                            }
                        }

                        processedTriangles++;
                        if (EditorUtility.DisplayCancelableProgressBar("Generating NavMesh2D", "Generating node network",
                                                         0.5f + (((float)processedTriangles / (float)totalTriangles) / 2)))
                            throw new System.OperationCanceledException("User canceled.");
                    }

                }
            }


            return allGeneratedNodes.ToArray();
        }

        private Mesh GenerateMesh(List<Polygon> triangulatedPolygon)
        {
            List<Vector3> allVerticies = new List<Vector3>();
            List<int> allTriangles = new List<int>();
            List<Vector3> allNormals = new List<Vector3>();
            List<Vector2> allUvs = new List<Vector2>();
            int index = 0;

            foreach (Polygon navmesh in triangulatedPolygon)
            {
                Dictionary<uint, Vector3> meshVerticies = new Dictionary<uint, Vector3>();
                List<int> meshIndicies = new List<int>();
                Dictionary<uint, int> vertexCodeToIndex = new Dictionary<uint, int>();

                {
                    HashSet<uint> processedVertices = new HashSet<uint>();

                    foreach (DelaunayTriangle t in navmesh.Triangles)
                    {
                        foreach (TriangulationPoint p in t.Points)
                        {
                            if (processedVertices.Contains(p.VertexCode))
                                continue;
                            processedVertices.Add(p.VertexCode);

                            meshVerticies.Add(p.VertexCode, new Vector3((float) p.X, (float) p.Y, 0));
                            vertexCodeToIndex.Add(p.VertexCode, index);
                            index++;
                        }
                    }
                }

                {
                    HashSet<uint> processedVertices = new HashSet<uint>();

                    foreach (DelaunayTriangle t in navmesh.Triangles)
                    {
                        foreach (TriangulationPoint p in t.Points)
                        {
                            meshIndicies.Add(vertexCodeToIndex[p.VertexCode]);
                            foreach (TriangulationPoint pp in t.Points)
                            {
                                if (pp.VertexCode == p.VertexCode)
                                    continue;
                            }
                            if (processedVertices.Contains(p.VertexCode))
                                continue;

                            processedVertices.Add(p.VertexCode);
                        }
                    }
                }



                Vector3[] finalVerticies = meshVerticies.Values.ToArray();
                int[] finalIndicies = meshIndicies.ToArray();

                Vector3[] normals = new Vector3[meshVerticies.Count];
                Vector2[] uvs = new Vector2[meshVerticies.Count];
                for (int i = 0; i < normals.Length; i++)
                {
                    uvs[i] = new Vector2(finalVerticies[i].x, finalVerticies[i].y) / 10;
                    normals[i] = -Vector3.forward;
                }

                allVerticies.AddRange(finalVerticies);
                allTriangles.AddRange(finalIndicies);
                allNormals.AddRange(normals);
                allUvs.AddRange(uvs);

            }

            Mesh generatedMesh = new Mesh
                                     {
                                         vertices = allVerticies.ToArray(),
                                         triangles = allTriangles.ToArray().Reverse().ToArray(),
                                         normals = allNormals.ToArray(),
                                         uv = allUvs.ToArray()
                                     };
            return generatedMesh;

        }

        private void RecursiveDrawPolytree(PolyNode tree, Color drawColor,Matrix4x4 matrix)
        {
            if (tree == null)
                return;

            DrawPointListLine(tree.Contour, drawColor, matrix, true);

            foreach (PolyNode cTree in tree.Childs)
            {
                RecursiveDrawPolytree(cTree, drawColor, matrix);
            }
        }

        private void DrawPointListLine(List<IntPoint> contour, Color drawColor, Matrix4x4 matrix, bool close)
        {
            Handles.matrix = matrix;

            if (contour == null || contour.Count == 0)
                return;

            Handles.color = drawColor;

            for (int j = 0; j < contour.Count; j++)
            {
                if (close && j == contour.Count - 1)
                {
                    Handles.DrawLine(
                    new Vector3(contour[j].X / GenerationInformation.CalculationScaleFactor, contour[j].Y / GenerationInformation.CalculationScaleFactor, 0),
                    new Vector3(contour[0].X / GenerationInformation.CalculationScaleFactor, contour[0].Y / GenerationInformation.CalculationScaleFactor, 0)
                    );
                }
                else
                {
                    Handles.DrawLine(
                    new Vector3(contour[j].X / GenerationInformation.CalculationScaleFactor, contour[j].Y / GenerationInformation.CalculationScaleFactor, 0),
                    new Vector3(contour[j + 1].X / GenerationInformation.CalculationScaleFactor, contour[j + 1].Y / GenerationInformation.CalculationScaleFactor, 0)
                    );
                }

            }
        }

        private void DrawPointListLine(Vector2[] contour, Color color, Matrix4x4 matrix, bool close)
        {
            Handles.matrix = matrix;

            if (contour == null || contour.Length == 0)
                return;

            Handles.color = color;

            for (int j = 0; j < (close ? contour.Length : contour.Length-1); j++)
            {
                if (close && j == contour.Length - 1)
                {
                    Handles.DrawLine(
                    new Vector3(contour[j].x, contour[j].y, 0),
                    new Vector3(contour[0].x, contour[0].y, 0)
                    );
                }
                else
                {
                    Handles.DrawLine(
                    new Vector3(contour[j].x, contour[j].y, 0),
                    new Vector3(contour[j + 1].x, contour[j + 1].y, 0)
                    );
                }

            }
        }

        private void DrawQuadTreeNodeRecursive(NodeIndexQuadTreeNode n)
        {
            if(n == null)
                return;

            Handles.color = NavMeshBaseSelectedColor*new Color(1, 1, 1, 0.25f);
            Handles.DrawLine(new Vector3(n.NodeBounds.xMin, n.NodeBounds.yMin), new Vector3(n.NodeBounds.xMax, n.NodeBounds.yMin));
            Handles.DrawLine(new Vector3(n.NodeBounds.xMax, n.NodeBounds.yMin), new Vector3(n.NodeBounds.xMax, n.NodeBounds.yMax));
            Handles.DrawLine(new Vector3(n.NodeBounds.xMax, n.NodeBounds.yMax), new Vector3(n.NodeBounds.xMin, n.NodeBounds.yMax));
            Handles.DrawLine(new Vector3(n.NodeBounds.xMin, n.NodeBounds.yMax), new Vector3(n.NodeBounds.xMin, n.NodeBounds.yMin));

            if (n.ChildNodes != null)
            {
                foreach (NodeIndexQuadTreeNode childNode in n.ChildNodes)
                {
                    DrawQuadTreeNodeRecursive(childNode);
                }
            }
        }

        private class PolyTreeEdgesRetained
        {
            public PolyTree Tree;
            public List<List<IntPoint>> Edges;
        }

        #region Overrides of VersionedEditorWindow

        public override VersionInformation VersionInformation()
        {
            return new VersionInformation("NavMesh2D", 1, 5, 1);
        }

        #endregion
    }
}
