using AlienExplorer.Model;

namespace AlienExplorer.View
{
    public interface IViewable
    {
        dSetCameraSize SetCameraSize { get; set; }

        void SendCameraSizeToModel();
        void ViewModel(GameModel parLevel);
    }
}
