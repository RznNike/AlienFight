using System;
using System.Collections.Generic;
using System.Threading;

namespace AlienExplorer.Model
{
    public class LevelLogic : BaseModelLogic
    {
        public TimeSpan LevelTimer { get; private set; }
        private bool _stopThread;

        public LevelLogic(GameModel parModel) : base(parModel)
        {
            _model.UIItems = new List<UIObject>();
            for (UIObjectType i = UIObjectType.Health; i <= UIObjectType.Timer; i++)
            {
                UIObject item = new UIObject()
                {
                    Type = i,
                    State = 0
                };
                _model.UIItems.Add(item);
            }
            MenuHeader = "";
            _currentMenu = UIObjectType.OK;
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
            Thread timerTick = new Thread(TimerTick)
            {
                IsBackground = true
            };
            timerTick.Start();
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
            if ((logic != null)
                && ((int)parCommand <= 3))
            {
                logic.ReceiveCommand(parCommand, parBeginCommand);
            }
        }

        public override void HandleCommand(ModelCommand parCommand)
        {
            switch (parCommand)
            {
                /*case ModelCommand.Up:
                    SelectPrevMenuItem();
                    break;
                case ModelCommand.Down:
                    SelectNextMenuItem();
                    break;*/
                case ModelCommand.OK:
                    AcceptAction();
                    break;
                case ModelCommand.Escape:
                    CancelAction();
                    break;
            }
        }

        protected override void AcceptAction()
        {
        }

        protected override void CancelAction()
        {
            LoadAnotherModel(GameModelType.Menu);
        }
    }
}
