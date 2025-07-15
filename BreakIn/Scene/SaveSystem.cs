using Godot;
using System;
using System.Collections.Generic;

public static class SaveSystem
{
    private const string root = "user://";
    private const string fileExtension = ".saved";

    public static Dictionary<string, Godot.Collections.Dictionary<string, Variant>> dataCache = new Dictionary<string, Godot.Collections.Dictionary<string, Variant>>();

    public static void UnloadAllDataFromCache(bool saveFirst = false)
    {
        if(saveFirst)
        {
            foreach(string path in dataCache.Keys)
            {
                SaveData(path);
            }
        }
        dataCache.Clear();
    }

    public static void UnloadFromCache(string path, bool saveFirst = false)
    {
        if(saveFirst)
        {
            SaveData(path);
        }
        dataCache.Remove(path);
    }

    // --------------------------------
    //			READING DATA	
    // --------------------------------
    #region Reading Data
    public static Variant GetDataItemVariant(string dataSetPath, string dataItemName, Variant defaultValue, bool cacheData = true)
    {
        var data = GetRawData(dataSetPath, cacheData);
        if(data.TryGetValue(dataItemName, out Variant variant))
        {
            return variant;
        }
        else
        {
            return defaultValue;
        }
    }

    public static T GetDataItem<[MustBeVariant] T>(string dataSetPath, string dataItemName, T defaultValue = default(T), bool cacheData = true)
    {
        return GetDataItemVariant(dataSetPath, dataItemName, Variant.From(defaultValue), cacheData).As<T>();
    }

    public static Godot.Collections.Dictionary<string, Variant> GetRawData(string path, bool cacheData = true)
    {
        if (dataCache.ContainsKey(path))
        {
            return dataCache[path];
        }
        else
        {
            return LoadDataFromStorage(path, cacheData);
        }
    }

    private static Godot.Collections.Dictionary<string, Variant> LoadDataFromStorage(string path, bool cacheData = true)
    {
        string loadedStringData = "";
        string filePath = root + path + fileExtension;

        Godot.Collections.Dictionary<string, Variant> resultData = null;

        if (FileAccess.FileExists(filePath))
        {
            using (FileAccess saveFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
            {
                loadedStringData = saveFile.GetAsText();
            }

            Json parsedJsonData = new Json();
            Error parseStatus = parsedJsonData.Parse(loadedStringData, keepText: false);
            if(parseStatus != Error.Ok)
            {
                return new Godot.Collections.Dictionary<string, Variant>();
            }

            try
            {
                resultData = (Godot.Collections.Dictionary<string, Variant>)parsedJsonData.Data;
                if(resultData == null)
                {
                    resultData = new Godot.Collections.Dictionary<string, Variant>();
                }
            }
            catch
            {
                return new Godot.Collections.Dictionary<string, Variant>();
            }

        }
        else
        {
            resultData = new Godot.Collections.Dictionary<string, Variant>();
        }

        if(cacheData)
        {
            if(dataCache.ContainsKey(path))
            {
                dataCache[path] = resultData;
            }
            else
            {
                dataCache.Add(path, resultData);
            }
        }

        return resultData;
    }

    #endregion

    // --------------------------------
    //			WRITING DATA	
    // --------------------------------
    #region Writing Data

    public static void AddDataItemVariant(string path, string dataItemName, Variant dataItem)
    {
        Godot.Collections.Dictionary<string, Variant> data = GetRawData(path, true);
        if(data.ContainsKey(dataItemName))
        {
            data[dataItemName] = dataItem;
        }
        else
        {
            data.Add(dataItemName, dataItem);
        }
    }

    public static void AddDataItem<[MustBeVariant] T>(string path, string dataItemName, T dataItem)
    {
        AddDataItemVariant(path, dataItemName, Variant.From(dataItem));
    }

    public static void AddRawData(string path, Godot.Collections.Dictionary<string, Variant> data, bool saveImmediate = false)
    {
        if (dataCache.ContainsKey(path))
        {
            dataCache[path] = data;
        }
        else
        {
            dataCache.Add(path, data);
        }
        if(saveImmediate)
        {
            SaveData(path);
        }

    }

    public static void SaveData(string path)
    {
        if(!dataCache.TryGetValue(path, out Godot.Collections.Dictionary<string, Variant> data)) { return; }
        string jsonString = Json.Stringify(data);

        using (FileAccess saveFile = FileAccess.Open(root + path + fileExtension, FileAccess.ModeFlags.Write))
        {
            saveFile.StoreString(jsonString);
        }
    }

    #endregion
}
