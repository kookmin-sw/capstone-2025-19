using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableArray<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    [NonSerialized] private List<KeyValuePair<TKey, TValue>> keyValuePairs = new List<KeyValuePair<TKey, TValue>>();

    [SerializeField] private bool foldout;

    public void OnBeforeSerialize()
    {
        keys.Clear(); values.Clear();

        foreach(var kv in keyValuePairs)
        {
            keys.Add(kv.Key);
            values.Add(kv.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        keyValuePairs.Clear();
        for(int i =0; i < keys.Count; i++)
        {
            if (keys[i] != null) { keyValuePairs.Add(new KeyValuePair<TKey, TValue>(keys[i], values[i])); }
            else { Debug.LogWarning($"Null key fount at index {i}. It will be skipped."); }
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (!ContainsKey(key)) { keyValuePairs.Add(new KeyValuePair<TKey,TValue>(key, value)); }
        else { Debug.LogWarning($"Attempted to add duplicate key: {key}. The entry was not added."); }
    }

    public bool ContainsKey(TKey key)
    {
        foreach (var kv in keyValuePairs)
        {
            if (EqualityComparer<TKey>.Default.Equals(kv.Key, key))
            {
                return true;
            }
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        foreach (var kvp in keyValuePairs)
        {
            if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
            {
                value = kvp.Value;
                return true;
            }
        }
        value = default(TValue);
        return false;
    }



    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return keyValuePairs.GetEnumerator();
    }

    

    

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Foldout
    {
        get { return foldout; }
        set { foldout = value; }
    }
}
