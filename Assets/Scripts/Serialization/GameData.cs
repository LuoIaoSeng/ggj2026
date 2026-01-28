using System;

[Serializable]
public class GameData
{
    public int level;
    public int checkpoint;

    public int resolutionIndex;
    public bool fullscreen;
    public int globalVolumn;
    public int gameVolumn;
    public int musicVolumn;
    public string lastSaveTime;
    public string saveVersion;
}