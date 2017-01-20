using UnityEngine;

[System.Serializable] //ha un drawer dedicato!!!
public class EventPicker
{
    public const string channelVarName = "channel";
    public eventChannels channel;

    //il nome di questi campi (il nome della variabile qui nel codice) deve corrispondere a un eventchannel se no errore da EventPickerDrawer
    public inGameChannelEvents inGame;
    public menuChannelEvents menu;
    public modelChannelEvents model;
    public int Selected
    {
        get
        {
            switch (channel)
            {
                case eventChannels.menu: return (int)menu;
                case eventChannels.inGame: return (int)inGame;
                case eventChannels.model: return (int)model;
                default:
                    Debug.LogError("EventPicker was passed an unimplemented channel!!! " + channel.ToString());
                    break;
            }
            return (int)channel;
        }
    }


}
