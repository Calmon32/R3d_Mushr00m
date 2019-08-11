using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.Navmesh2D.Editor
{
    [CustomEditor(typeof(NavMesh2DBehaviour))]
    public class NavMesh2DBehaviourInspector : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            NavMesh2DBehaviour _sceneNavmesh = target as NavMesh2DBehaviour;

            if(_sceneNavmesh == null)
                return;

            GUIStyle informationPanel = new GUIStyle(EditorStyles.miniLabel);
            informationPanel.normal.background = EditorGUIUtility.whiteTexture;
            informationPanel.padding = new RectOffset(10,10,10,10);
            informationPanel.margin = new RectOffset(10, 15, 20, 20);


            Color restoreColor = GUI.color;

            GUI.color = new Color(0, 0, 0, 0.25f);
            GUILayout.BeginVertical(informationPanel);
            GUI.color = restoreColor;

            EditorGUILayout.BeginHorizontal(EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("NavMesh2D Properties", EditorStyles.whiteMiniLabel);
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

            GUILayout.EndVertical();
        }
    }
}
