using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRAssetPathObject : ScriptableObject
{
    public static readonly string assetName = "AssetManager.asset";

    public string projectName = "";
    [SerializeField]
    string bg = "bg";
    [SerializeField]
    string stand = "fgimage";
    [SerializeField]
    string uiImage = "image";
    [SerializeField]
    string ruleImage = "rule";
    [SerializeField]
    string bgm = "bgm";
    [SerializeField]
    string voice = "voice";
    [SerializeField]
    string se = "sound";
    [SerializeField]
    string other = "other";
}
