using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBUnity.Editor {

    [CustomEditor(typeof(SpriteAnimator))]
    public class listExampleInspector : UnityEditor.Editor {

        private ReorderableList _reorderableList;
        private SerializedProperty _seralizedFrames = null;

        private SpriteAnimator Target {
            get {
                return target as SpriteAnimator;
            }
        }

        private void OnEnable() {
            _seralizedFrames = serializedObject.FindProperty("_frames");

            _reorderableList = new ReorderableList(serializedObject, _seralizedFrames, true, true, true, true);

            _reorderableList.drawHeaderCallback += DrawHeader;
            _reorderableList.drawElementCallback += DrawElement;
            _reorderableList.onAddCallback += AddItem;
            _reorderableList.onRemoveCallback += RemoveItem;
            _reorderableList.onReorderCallbackWithDetails += ReorderCallbackDelegateWithDetails;

            _reorderableList.elementHeight = 50.0f;
        }

        private void OnDisable() {
            _reorderableList.drawHeaderCallback -= DrawHeader;
            _reorderableList.drawElementCallback -= DrawElement;

            _reorderableList.onAddCallback -= AddItem;
            _reorderableList.onRemoveCallback -= RemoveItem;
            _reorderableList.onReorderCallbackWithDetails -= ReorderCallbackDelegateWithDetails;
        }

        private void DrawHeader(Rect rect) {
            GUI.Label(rect, "Animation Frames");
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused) {
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(rect, _seralizedFrames.GetArrayElementAtIndex(index), typeof(Sprite));
            if (EditorGUI.EndChangeCheck()) {
                SaveSerializedObject();
            }
        }

        private void AddItem(ReorderableList list) {
            _seralizedFrames.InsertArrayElementAtIndex(_seralizedFrames.arraySize);
            SaveSerializedObject();
        }

        private void RemoveItem(ReorderableList list) {
            _seralizedFrames.DeleteArrayElementAtIndex(list.index);
            SaveSerializedObject();
        }

        private void ReorderCallbackDelegateWithDetails(ReorderableList list, int oldIndex, int newIndex) {
            _seralizedFrames.MoveArrayElement(oldIndex, newIndex);
            SaveSerializedObject();
        }

        private void SaveSerializedObject() {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _reorderableList.DoLayoutList();
        }
    }
}
