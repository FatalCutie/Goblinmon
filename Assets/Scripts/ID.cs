using System.Linq;
using UnityEngine;
using System;

//Thank you u/Instinctx for making my job WAY easier
public class MonoBehaviourID : MonoBehaviour
{
    [SerializeField]
    private UniqueID _id;

    public string ID
    {
        get { return _id.Value; }
    }

    [ContextMenu("Force reset ID")]
    public void ResetId()
    {
        _id.Value = Guid.NewGuid().ToString();
        Debug.Log("Setting new ID on object: " + gameObject.name, gameObject);
    }

    //Need to check for duplicates when copying a gameobject/component
    public static bool IsUnique(string ID)
    {
        return Resources.FindObjectsOfTypeAll<MonoBehaviourID>().Count(x => x.ID == ID) == 1;
    }

    public void CloneIdFrom(MonoBehaviourID source)
    {
        if (source != null)
        {
            _id.Value = source.ID;
            Debug.Log("Cloned ID from " + source.gameObject.name + " to " + gameObject.name);
        }
        else
        {
            Debug.LogError("Source MonoBehaviourID is null.");
        }
    }

    protected void OnValidate()
    {
        //If scene is not valid, the gameobject is most likely not instantiated (ex. prefabs)
        if (!gameObject.scene.IsValid())
        {
            _id.Value = string.Empty;
            return;
        }

        if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
        {
            ResetId();
        }
    }

    [Serializable]
    private struct UniqueID
    {
        public string Value;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
    private class UniqueIdDrawer : UnityEditor.PropertyDrawer
    {
        private const float buttonWidth = 120;
        private const float padding = 2;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUI.enabled = false;
            Rect valueRect = position;
            valueRect.width -= buttonWidth + padding;
            UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("Value");
            UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

            GUI.enabled = true;

            Rect buttonRect = position;
            buttonRect.x += position.width - buttonWidth;
            buttonRect.width = buttonWidth;
            if (GUI.Button(buttonRect, "Copy to clipboard"))
            {
                UnityEditor.EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
            }

            UnityEditor.EditorGUI.EndProperty();
        }
    }
#endif
}