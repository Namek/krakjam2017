using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event channels.
/// per aggiungere un canale, aggiungere prima un campo NOMECANALE in eventChannels, 
/// poi un enum chiamato NOMECANALEChannelEvents, quindi un
/// enumChannelEventList.Add(eventChannels.inGame, System.Enum.GetValues(typeof(NOMECANALEChannelEvents)));
/// dentro getChannelEnumList in ChannelEnums
/// poi in eventPickerva aggiunto il campo per il canale e il nome della variabile come const secondo il template:
/// public const string NOMECANALEVarName = "NOMECANALE";
/// public NOMECANALEChannelEvents NOMECANALE;
/// quindi un caso nello switch di Selected:
/// case eventChannels.NOMECANALE: return NOMECANALE;
/// seguito da un caso nella serie di if dell EventPickerDrawer
///     if (chosen.Equals(eventChannels.NOMECANALE))
///         EditorGUI.PropertyField(tempPos, property.FindPropertyRelative(EventPicker.NOMECANALEVarName), GUIContent.none);
/// </summary>

public enum eventChannels
{
    menu = 0,
    inGame = 1,
    model = 2,
}
public enum menuChannelEvents
{
    closeMainMenu = 1,
    openMainMenu = 2,
    closeGameUI = 3,
    openGameUI = 4,
    closeEndMenu = 5,
    openEndMenu = 6,
}
public enum inGameChannelEvents
{
    gameStart = 1,
    gameOver = 2,
    playerHitByWave =3,
    baseHitByWave=4,
    gameReset=5
}
public enum modelChannelEvents
{
    modelReady = 1,
}

public struct introOverEventData
{
    public int Id;
}
public struct vector3EventData
{
    public Vector3 vec;
}
public struct GOEventArg
{
    public GameObject gameObjectArg;
}


public class ChannelEnums
{
    public static Dictionary<eventChannels, System.Array> getChannelEnumList()
    {

        Dictionary<eventChannels, System.Array> enumChannelEventList = new Dictionary<eventChannels, System.Array>();
        enumChannelEventList.Add(eventChannels.menu, System.Enum.GetValues(typeof(menuChannelEvents)));
        enumChannelEventList.Add(eventChannels.inGame, System.Enum.GetValues(typeof(inGameChannelEvents)));
        enumChannelEventList.Add(eventChannels.model, System.Enum.GetValues(typeof(modelChannelEvents)));
        return enumChannelEventList;
    }
}