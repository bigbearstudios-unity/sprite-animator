using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBUnity.Editor {

    [CustomEditor(typeof(SpriteAnimator))]
    public class listExampleInspector : UnityEditor.Editor {

    private ReorderableList _reorderableList;

    private SpriteAnimator Target {
        get {
            return target as SpriteAnimator;
        }
    }

    private void OnEnable() {
        _reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("_frames"), true, true, true, true);

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
            Sprite item = Target.Frames[index];

            EditorGUI.BeginChangeCheck();
            Target.Frames[index] = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, 75.0f, 50.0f), item, typeof(Sprite), allowSceneObjects: true);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }
        }

    private void AddItem(ReorderableList list) {
        Target.AddFrame(null);
        EditorUtility.SetDirty(target);
    }

    private void RemoveItem(ReorderableList list) {
        Target.RemoveFrame(list.index);
        EditorUtility.SetDirty(target);
    }

    private void ReorderCallbackDelegateWithDetails(ReorderableList list, int oldIndex, int newIndex) {
        
    }

    public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _reorderableList.DoLayoutList();
        }
    }
}