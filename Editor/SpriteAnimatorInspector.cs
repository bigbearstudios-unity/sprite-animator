using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBUnity.SpriteAnimation.Editor {

    [CustomEditor(typeof(SpriteAnimator))]
    public class SpriteAnimatorEditor : UnityEditor.Editor {

        private ReorderableList _reorderableList;
        private SerializedProperty _serializedFrames = null;

        private SpriteAnimator Target {
            get { return target as SpriteAnimator; }
        }

        private void OnEnable() {
            _serializedFrames = serializedObject.FindProperty("_frames");

            _reorderableList = new ReorderableList(serializedObject, _serializedFrames, true, true, true, true);

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
            EditorGUI.ObjectField(rect, _serializedFrames.GetArrayElementAtIndex(index), typeof(Sprite));
            if(EditorGUI.EndChangeCheck()) {
                SaveSerializedObject();
            }
        }

        private void AddItem(ReorderableList list) {
            _serializedFrames.InsertArrayElementAtIndex(_serializedFrames.arraySize);
            SaveSerializedObject();
        }

        private void RemoveItem(ReorderableList list) {
            _serializedFrames.DeleteArrayElementAtIndex(list.index);
            SaveSerializedObject();
        }

        private void ReorderCallbackDelegateWithDetails(ReorderableList list, int oldIndex, int newIndex) {
            _serializedFrames.MoveArrayElement(oldIndex, newIndex);
            SaveSerializedObject();
        }

        private void SaveSerializedObject() {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "_frames");
            _reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
