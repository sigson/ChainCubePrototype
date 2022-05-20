using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    public List<DictionaryNode<TKey, TValue>> dictionary = new List<DictionaryNode<TKey, TValue>>();

    public void AddEx(TKey key, TValue value)
    {
        base.Add(key, value);
        DirectUpdate();
    }

    public void RemoveEx(TKey key)
    {
        base.Remove(key);
        DirectUpdate();
    }

    public void ClearEx()
    {
        base.Clear();
        DirectUpdate();
    }

    private void DirectUpdate()
    {
        dictionary.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            dictionary.Add(new DictionaryNode<TKey, TValue>(pair.Key, pair.Value));
        }
    }
    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        if (dictionary.Count == this.Count)
        {
            dictionary.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                dictionary.Add(new DictionaryNode<TKey, TValue>(pair.Key, pair.Value));
            }
        }
        else
        {
            Debug.Log("You have a double key elements in data array. Remove or rename key on this data");
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        //while(keys.Count > values.Count)
        //    values.Add(default(TValue));
        //while (keys.Count < values.Count)
        //    keys.Add(default(TKey));
        //throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
        for (int i = 0; i < dictionary.Count; i++)
        {
            try
            {

                this.Add(dictionary[i].key, dictionary[i].value);
            }
            catch
            {

            }
        }
    }
}
[System.Serializable]
public class DictionaryNode<TKey, TValue>
{
    public TKey key;
    public TValue value;
    public DictionaryNode(TKey Key, TValue Value)
    {
        key = Key;
        value = Value;
    }
}

public static class ListReverse
{
    public static List<T> ReverseEx<T>(this List<T> list)
    {
        list.Reverse();
        return list;
    }
}