using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrefComponent<T>
{
    protected T DefaultValue;
    protected T Value;
    protected string Key;
    
    public PrefComponent(string key, T value)
    {
        DefaultValue = value;
        Key = key;
        if(!PlayerPrefs.HasKey(key))
            SetValue(DefaultValue);
    }

    public virtual void SetValue(T value)
    {
        Value = value;
    }

    public abstract T GetValue();

    public void ClearValue()
    {
        PlayerPrefs.DeleteKey(Key);
    }
}

public class BoolPref : PrefComponent<bool>
{
    public BoolPref(string key, bool value) : base(key, value)
    {

    }

    public override bool GetValue()
    {
        if (!PlayerPrefs.HasKey(Key))
            PlayerPrefs.SetInt(Key, Value ? 1 : 0);
        return PlayerPrefs.GetInt(Key) == 1;
    }

    public override void SetValue(bool value)
    {
        base.SetValue(value);
        PlayerPrefs.SetInt(Key, value ? 1 : 0);
    }
}

public class IntPref : PrefComponent<int>
{
    public IntPref(string key, int value) : base(key, value)
    {

    }

    public override int GetValue()
    {
        if (!PlayerPrefs.HasKey(Key))
            PlayerPrefs.SetInt(Key, Value);
        return PlayerPrefs.GetInt(Key);
    }

    public override void SetValue(int value)
    {
        base.SetValue(value);
        PlayerPrefs.SetInt(Key, value);
    }

    public void AddValue(int value)
    {
        Value = PlayerPrefs.GetInt(Key) + value;
        SetValue(Value);
    }
}
