using Godot;
using Godot.Collections;
using System.Linq;

public partial class SaveManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private Array<Variant> dataToSave = new Array<Variant>();
    [Export]
    private string defaultSaveFileName = "breakoutGameData";

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------
    public static SaveManager Instance { get; private set; }
    public Array<Variant> DataToSave { get => dataToSave; set => dataToSave = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    // --------------------------------
    //		    FILE LOGIC	
    // --------------------------------

    public void SaveToFile()
    {
        SaveToFile(defaultSaveFileName);
    }

    /// <summary>
    /// Stores all data in the appropriate data list into a json object and writes it into a file, using the listName for the fileName
    /// FILES ARE SAVED TO C:\Users\[UserName]\AppData\Roaming\Godot\app_userdata\[Projectname]
    /// </summary>
    public void SaveToFile(string fileName)
    {
        using var saveFile = FileAccess.Open("user://" + fileName + ".save", FileAccess.ModeFlags.Write);

        Array dataToSaveArray = new Array(dataToSave.ToArray());

        var jsonData = Json.Stringify(dataToSaveArray);
        saveFile.StoreLine(jsonData);
        dataToSave.Clear();
    }

    public Array LoadFromFile()
    {
        return LoadFromFile(defaultSaveFileName);
    }

    /// <summary>
    /// Takes in a specific file and attempts to load in the data of said file.
    /// </summary>
    /// <param name="specificFile"></param>
    public Array LoadFromFile(string specificFile)
    {
        using var saveFile = FileAccess.Open("user://" + specificFile + ".save", FileAccess.ModeFlags.Read);
        Array loadedData = new Array();

        if (saveFile == null) return null;

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

            loadedData += (Array)json.Data;
        }

        return loadedData;
    }
}
