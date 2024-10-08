using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys count is not equal to values count");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            if (this.ContainsKey(keys[i]))
            {
                Debug.LogWarning($"Duplicate key found: {keys[i]}. Updating value.");
                this[keys[i]] = values[i]; // Mevcut de�eri g�ncelle
            }
            else
            {
                this.Add(keys[i], values[i]); // Yeni anahtar-de�er �ifti ekle
            }
        }
    }
}
