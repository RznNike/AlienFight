using System.Threading;

namespace AlienExplorer.Model
{
    public interface ILogic
    {
        void Start(ManualResetEventSlim parManualResetEventSlim);

        void Stop();

        void Resume();
    }
}
