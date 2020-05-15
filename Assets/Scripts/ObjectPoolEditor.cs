//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(ObjectPool))]
//[CanEditMultipleObjects]
//public class ObjectPoolEditor : Editor
//{
//    ObjectPool objectPool;

//    private void OnEnable()
//    {
//        objectPool = (ObjectPool)target;
//    }
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        if (GUILayout.Button("Generate Sprite Masks"))
//        {
//            objectPool.GenerateMasks();
//        }

//        if (GUILayout.Button("Delete All Sprite Masks"))
//        {
//            objectPool.DestroyAllMasks();
//        }

//        if (GUILayout.Button("Generate Bullets"))
//        {
//            objectPool.GenerateBullets(null);
//        }

//        if (GUILayout.Button("Delete All Bullets"))
//        {
//            objectPool.DestroyAllBullets();
//        }
//    }
//}
