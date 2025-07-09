using Godot;
using Godot.Collections;
using System.Linq;
// using System;

public partial class SaveSystem : Node
{
    public static SaveSystem Instance { get; private set; }
    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    private Array<Variant> PopulateDataToSave()
    {
        return null;
    }

    /// <summary>
    /// Stores all data in the appropriate data list into a json object and writes it into a file, using the listName for the fileName
    /// </summary>
    public void Save(string fileName)
    {
        Array<Variant> dataToSave = PopulateDataToSave();
        using var saveFile = FileAccess.Open("user://" + fileName + ".save", FileAccess.ModeFlags.Write);

        Array dataToSaveArray = new Array(dataToSave.ToArray());

        var jsonData = Json.Stringify(dataToSaveArray);
        saveFile.StoreLine(jsonData);
        dataToSave.Clear();
    }

    /// <summary>
    /// Takes in a specific file and attempts to load in the data of said file. Deletes relevant data from the application prior to loading in new data.
    /// </summary>
    /// <param name="specificFile"></param>
    public void Load(string specificFile)
    {
        using var saveFile = FileAccess.Open("user://" + specificFile + ".save", FileAccess.ModeFlags.Read);

        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            var jsonString = saveFile.GetLine();

            var json = new Json();
            var parseResult = json.Parse(jsonString, true);

            if (parseResult != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
                continue;
            }

            Array data = new Godot.Collections.Array((Godot.Collections.Array)json.Data);

            // Handle the data
        }
    }
}
