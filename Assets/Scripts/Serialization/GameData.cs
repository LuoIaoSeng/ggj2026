using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public string[] resolutions = { "1920x1080", "2560x1440" };
    public int level;
    public int checkpoint;

    public string resolution;
    public bool fullscreen;
    public int globalVolumn;
    public int gameVolumn;
    public int musicVolumn;
    public string lastSaveTime;
    public string saveVersion;
}