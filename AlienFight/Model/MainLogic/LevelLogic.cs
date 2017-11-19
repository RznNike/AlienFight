using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AlienExplorer.Model
{
    public class LevelLogic : BaseModelLogic
    {
        public TimeSpan LevelTimer { get; private set; }
        private DateTime _timer;
        private GameModel _model;
        private bool _stopThread;
        private Mutex mutex;

        public LevelLogic(GameModel parModel) : base(parModel)
        {
            _stateMachine = new LevelMenuStateMachine(parModel);
            MenuHeader = _stateMachine.MenuHeader;
            ShadowLevel = _stateMachine.ShadowLevel;
            _model = parModel;
            LevelTimer = new TimeSpan(0);
            mutex = new Mutex(true, "AlienExplorerLogicMutex");
        }

        public void Start()
        {
            mutex.ReleaseMutex();
            _model.PlayerLogics.Start();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Start();
                }
            }

            _stopThread = false;
            Thread timerTickThread = new Thread(TimerTick)
            {
                IsBackground = true
            };
            timerTickThread.Start();
        }

        public void Stop()
        {
            _stopThread = true;
            _model.PlayerLogics.Stop();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Stop();
                }
            }
            mutex.ReleaseMutex();
        }

        public void Pause()
        {
            mutex.WaitOne();
        }

        public void Resume()
        {
            _timer = DateTime.UtcNow;
            _model.PlayerLogics.Resume();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Resume();
                }
            }
            mutex.ReleaseMutex();
        }

        private void TimerTick()
        {
            _timer = DateTime.UtcNow;
            while (!_stopThread)
            {
                mutex.WaitOne();
                mutex.ReleaseMutex();
                DateTime newTime = DateTime.UtcNow;
                LevelTimer += newTime - _timer;
                _timer = newTime;
                Thread.Sleep(50);
            }
        }

        public override void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                HandleCommand(parCommand);
            }
            PlayerLogic logic = _model.PlayerLogics;
            if ((_stateMachine.MenuHeader.Equals(""))
                && (logic != null)
                && (parCommand <= ModelCommand.Down))
            {
                logic.ReceiveCommand(parCommand, parBeginCommand);
            }
        }

        protected override void HandleCommand(ModelCommand parCommand)
        {
            _stateMachine.ChangeState(parCommand);
            SelectedMenuItem = _stateMachine.SelectedMenuItem;
            MenuHeader = _stateMachine.MenuHeader;
            ShadowLevel = _stateMachine.ShadowLevel;
            switch (_stateMachine.CurrentCommand)
            {
                case ModelStateMachineCommand.Pause:
                    this.Pause();
                    break;
                case ModelStateMachineCommand.Resume:
                    this.Resume();
                    break;
                case ModelStateMachineCommand.LoadMenu:
                    this.Stop();
                    LoadAnotherModel?.Invoke(GameModelType.Menu);
                    break;
                case ModelStateMachineCommand.LoadLevel:
                    this.Stop();
                    LoadAnotherModel?.Invoke(GameModelType.Level, _model.LevelID);
                    break;
                case ModelStateMachineCommand.LoadNextLevel:
                    this.Stop();
                    try
                    {
                        List<int> levels = LevelLoader.CheckAvailableLevels().OrderBy(x => x).ToList();
                        int currentNumber = levels.FindIndex(x => x == _model.LevelID);
                        int nextLevelID = levels[currentNumber + 1];
                        LoadAnotherModel?.Invoke(GameModelType.Level, nextLevelID);
                    }
                    catch
                    {
                        LoadAnotherModel?.Invoke(GameModelType.Menu);
                    }
                    break;
            }
        }
    }
}
