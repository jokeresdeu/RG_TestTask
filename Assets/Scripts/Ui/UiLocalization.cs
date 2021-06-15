using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName  = "Localiztion", menuName = ("UiLocaliztion/Localization"), order = 2)]
public class UiLocalization : ScriptableObject
{
    [SerializeField] private UiElementData[] _uiDatas;
    public UiElementData[] UiDatas => _uiDatas;
}

public enum Languages
{
    English,
}

public enum UiElement
{
    Lvl,
    SwipeToStart,
    LvlCompleted,
    NextLvl,
    Score, 
    TotalScore,
    Cleared,

}

[Serializable]
public class UiElementData
{
    [SerializeField] private UiElement _uiElement;
    [SerializeField] private string _text;

    public UiElement UiElement => _uiElement;
    public string Text => _text;
}
