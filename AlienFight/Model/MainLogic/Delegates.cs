namespace AlienExplorer.Model
{
    public delegate void dLoadAnotherModel(GameModelType parModelType, int parLevelID = 1);
    public delegate void dCloseApplication();
    public delegate void dSetCameraSize(float parSizeX, float parSizeY);
}
