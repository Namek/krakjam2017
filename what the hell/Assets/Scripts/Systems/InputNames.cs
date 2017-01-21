using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputNames
{
    Horizontal,
    Vertical,
    Fire1,
    Fire2,
    Fire3,
    Jump,
    Submit,
    Cancel
}

public static class ExtendInputNames
{
    public static string P(this InputNames toComplete, int playerId)
    {
        return toComplete.ToString() + "_P" + playerId;
    }
}