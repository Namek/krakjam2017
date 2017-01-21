using UnityEditor;
using UnityEngine;

/// <summary>
/// drawer per EventPicker
/// </summary>
[CustomPropertyDrawer(typeof(EventPicker))]
public class EventPickerDrawer : PropertyDrawer
{
    protected virtual float ROWHEIGHT
    {
        get
        { return 18f; }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //inserisce label
        EditorGUI.PrefixLabel(position, label);
        //dopo il label iniziano i dati, dichiara Rect per indicare la posizione
        Rect tempPos = position;
        //imposta l'altezza
        tempPos.height = ROWHEIGHT;
        //lo fa partire una riga sotto il label
        tempPos.y += ROWHEIGHT;
        //imposta dimensioni orizzontali dei picker
        tempPos.x += position.width * .4f;
        tempPos.width *= .6f;

        //recupera contenuto della property
        SerializedProperty channel = property.FindPropertyRelative(EventPicker.channelVarName);
        //disegna il primo picker per enum channel
        EditorGUI.PropertyField(tempPos, channel, GUIContent.none);
        //incremento riga
        tempPos.y += ROWHEIGHT;
        //disegna secondo picker per enum evento, scegliendo quale in base al channel selezionato
        eventChannels chosen = (eventChannels)channel.enumValueIndex;
        SerializedProperty selection = property.FindPropertyRelative(chosen.ToString());
        EditorGUI.PropertyField(tempPos, selection, GUIContent.none);

    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 3f + 5f;
    }
}
