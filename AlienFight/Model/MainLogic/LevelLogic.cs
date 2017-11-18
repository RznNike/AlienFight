using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AlienExplorer.Model
{
    public class LevelLogic : BaseModelLogic
    {
        public TimeSpan LevelTimer { get; private set; }
        public bool ShadowLevel { get; private set; }
        private GameModel _model;
        private bool _stopThread;
        private Thread _timerTick;

        public LevelLogic(GameModel parModel) : base(parModel)
        {
            _stateMachine = new LevelMenuStateMachine(parModel);
            ShadowLevel = false;
            MenuHeader = _stateMachine.MenuHeader;
            _model = parModel;
            LevelTimer = new TimeSpan(0);
        }

        public void Start()
        {
            _model.PlayerLogics.Start();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Start();
                }
            }

            _stopThread = false;
            _timerTick = new Thread(TimerTick)
            {
                IsBackground = true
            };
            _timerTick.Start();
        }

        public void Stop()
        {
            _stopThread = true;
            if (_timerTick != null)
            {
                _timerTick.Resume();
            }
            _model.PlayerLogics.Stop();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Stop();
                }
            }
        }

        public void Pause()
        {
            if (_timerTick != null)
            {
                _timerTick.Suspend();
            }
            _model.PlayerLogics.Pause();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Pause();
                }
            }
        }

        public void Resume()
        {
            if (_timerTick != null)
            {
                _timerTick.Resume();
            }
            _model.PlayerLogics.Resume();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Resume();
                }
            }
        }

        private void TimerTick()
        {
            DateTime lastTime = DateTime.UtcNow;
            while (!_stopThread)
            {
                DateTime newTime = DateTime.UtcNow;
                LevelTimer += newTime - lastTime;
                lastTime = newTime;
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
