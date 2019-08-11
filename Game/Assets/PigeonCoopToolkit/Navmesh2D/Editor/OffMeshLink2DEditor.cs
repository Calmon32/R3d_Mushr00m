using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.Navmesh2D.Editor
{
    [CustomEditor(typeof(OffMeshLink2D))]
    public class OffMeshLink2DEditor : UnityEditor.Editor {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
 

        public void OnSceneGUI()
        {
            OffMeshLink2D link2D = (OffMeshLink2D)target;
            Handles.color = Color.green;
            link2D.PointAPos = link2D.transform.InverseTransformPoint(Handles.FreeMoveHandle(link2D.transform.TransformPoint(link2D.PointAPos), Quaternion.identity, 0.2f, Vector3.zero, Handles.CircleCap));
            Handles.color = Color.red;
            link2D.PointBPos = link2D.transform.InverseTransformPoint(Handles.FreeMoveHandle(link2D.transform.TransformPoint(link2D.PointBPos), Quaternion.identity, 0.2f, Vector3.zero, Handles.CircleCap));

            EditorUtility.SetDirty(link2D);
        }
    }
}
