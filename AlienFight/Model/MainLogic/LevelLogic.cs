using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Логика игрового уровня.
    /// </summary>
    public class LevelLogic : BaseModelLogic
    {
        /// <summary>
        /// Период задержки для цикла вычислений состояния модели.
        /// </summary>
        private static readonly int THREAD_SLEEP_MS = 5;
        /// <summary>
        /// Необходимый для выигрыша наезд игрока на цель (в клетках).
        /// </summary>
        private static readonly float PLAYER_TO_DOOR_WIN_OFFSET = 0.4f;
        /// <summary>
        /// Радиус активации движения камеры к игроку по горизонтали (доля ширины камеры).
        /// </summary>
        private static readonly float CAMERA_ACTIVATE_RANGE_X = 0.1f;
        /// <summary>
        /// Радиус активации движения камеры к игроку по вертикали (доля высоты камеры).
        /// </summary>
        private static readonly float CAMERA_ACTIVATE_RANGE_Y = 0.15f;
        /// <summary>
        /// Радиус деактивации движения камеры к игроку по горизонтали (доля ширины камеры).
        /// </summary>
        private static readonly float CAMERA_DEACTIVATE_RANGE_X = 0.03f;
        /// <summary>
        /// Радиус деактивации движения камеры к игроку по вертикали (доля высоты камеры).
        /// </summary>
        private static readonly float CAMERA_DEACTIVATE_RANGE_Y = 0.03f;
        
        /// <summary>
        /// Модель.
        /// </summary>
        private GameModel _model;
        /// <summary>
        /// Таймер для нахождения дельты времени в потоковом цикле логики.
        /// </summary>
        private DateTime _timer;
        /// <summary>
        /// Флаг движения камеры к игроку по горизонтали.
        /// </summary>
        private bool _cameraMovingX;
        /// <summary>
        /// Флаг движения камеры к игроку по вертикали.
        /// </summary>
        private bool _cameraMovingY;
        /// <summary>
        /// Флаг остановки потокового цикла логики.
        /// </summary>
        private bool _stopThread;
        /// <summary>
        /// Событие для блокировки потока во время паузы.
        /// </summary>
        private ManualResetEventSlim _manualResetEventSlim;

        /// <summary>
        /// Таймер времени прохождения уровня.
        /// </summary>
        public TimeSpan LevelTimer { get; private set; }

        /// <summary>
        /// Инициализирует логику модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        public LevelLogic(GameModel parModel) : base(parModel)
        {
            _stateMachine = new LevelMenuStateMachine(parModel);
            MenuHeader = _stateMachine.MenuHeader;
            ShadowLevel = _stateMachine.ShadowLevel;
            _model = parModel;
            LevelTimer = new TimeSpan(0);
            _manualResetEventSlim = new ManualResetEventSlim(true);
        }

        /// <summary>
        /// Запуск работы логики уровня и всех логик объектов.
        /// </summary>
        public void Start()
        {
            _manualResetEventSlim.Set();

            _model.PlayerLogics.Start(_manualResetEventSlim);
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Start(_manualResetEventSlim);
                }
            }

            _stopThread = false;
            Thread logicThread = new Thread(IterativeAction)
            {
                IsBackground = true
            };
            logicThread.Start();
        }

        /// <summary>
        /// Остановка работы логики уровня и всех логик объектов.
        /// </summary>
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
            _manualResetEventSlim.Set();
        }

        /// <summary>
        /// Пауза работы логики уровня и всех логик объектов.
        /// </summary>
        public void Pause()
        {
            _manualResetEventSlim.Reset();
        }

        /// <summary>
        /// Возобновление работы логики уровня и всех логик объектов.
        /// </summary>
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
            _manualResetEventSlim.Set();
        }

        /// <summary>
        /// Получение команды от контроллера.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        /// <param name="parBeginCommand">Флаг начала команды (true, если начата).</param>
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

        /// <summary>
        /// Обработка команды контроллера внутри логики модели.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
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

        /// <summary>
        /// Потоковый цикл логики уровня.
        /// </summary>
        private void IterativeAction()
        {
            _timer = DateTime.UtcNow;
            DateTime newTime;
            TimeSpan timerDelta;

            while (!_stopThread)
            {
                _manualResetEventSlim.Wait();
                newTime = DateTime.UtcNow;
                timerDelta = newTime - _timer;
                LevelTimer += timerDelta;
                _timer = newTime;

                _stopThread = CheckPlayerHP() || CheckPlayerPosition();
                if (!_stopThread)
                {
                    MoveCamera((float)timerDelta.TotalSeconds);
                    Thread.Sleep(THREAD_SLEEP_MS);
                }
            }
        }

        /// <summary>
        /// Проверка очков жизни игрока. При необходимости изменяет состояние автомата.
        /// </summary>
        /// <returns>True, если очков 0 и нужно остановить поток.</returns>
        private bool CheckPlayerHP()
        {
            bool stopThread = false;
            if (_model.Player.Health == 0)
            {
                this.Stop();
                ((LevelMenuStateMachine)_stateMachine).EnterToMenu(UIObjectType.Restart);
                SelectedMenuItem = _stateMachine.SelectedMenuItem;
                MenuHeader = _stateMachine.MenuHeader;
                ShadowLevel = _stateMachine.ShadowLevel;
                stopThread = true;
            }

            return stopThread;
        }

        /// <summary>
        /// Проверка позиции игрока. При необходимости изменяет состояние автомата и сохраняет рекорд.
        /// </summary>
        /// <returns>True, если игрок у цели и нужно остановить поток.</returns>
        private bool CheckPlayerPosition()
        {
            bool stopThread = false;
            if (IsPlayerInGoalPoint())
            {
                this.Stop();
                ((LevelMenuStateMachine)_stateMachine).EnterToMenu(UIObjectType.Next_level);
                SelectedMenuItem = _stateMachine.SelectedMenuItem;
                MenuHeader = _stateMachine.MenuHeader;
                ShadowLevel = _stateMachine.ShadowLevel;
                if (_model.Player.Health == _model.Player.HealthMax)
                {
                    SaveFile.GetInstance().CheckAndSetRecord(_model.LevelID, LevelTimer);
                }
                List<int> nextLevels = LevelLoader.CheckAvailableLevels().OrderBy(x => x).SkipWhile(x => x <= _model.LevelID).ToList();
                if (nextLevels.Count > 0)
                {
                    int nextLevel = nextLevels.First();
                    SaveFile.GetInstance().LevelToLoad = nextLevel;
                    if (nextLevel > SaveFile.GetInstance().OpenedLevel)
                    {
                        SaveFile.GetInstance().OpenedLevel = nextLevel;
                    }
                }
                stopThread = true;
            }

            return stopThread;
        }

        /// <summary>
        /// Проверка достижения игроком цели.
        /// </summary>
        /// <returns>True, если игрок у цели.</returns>
        private bool IsPlayerInGoalPoint()
        {
            bool result = false;
            foreach (LevelObject elDoor in _model.Doors)
            {
                if (elDoor.State == 1)
                {
                    result |= IsIntersected(_model.Player.X + _model.Player.SizeX * PLAYER_TO_DOOR_WIN_OFFSET,
                            _model.Player.X + _model.Player.SizeX * (1 - PLAYER_TO_DOOR_WIN_OFFSET),
                            elDoor.X,
                            elDoor.X + elDoor.SizeX)
                        && IsIntersected(_model.Player.Y + _model.Player.SizeY * PLAYER_TO_DOOR_WIN_OFFSET,
                            _model.Player.Y + _model.Player.SizeY * (1 - PLAYER_TO_DOOR_WIN_OFFSET),
                            elDoor.Y,
                            elDoor.Y + elDoor.SizeY);
                }
            }

            return result;
        }

        /// <summary>
        /// Проверка пересечения двух отрезков.
        /// </summary>
        /// <param name="parMin1">Левая граница отрезка 1.</param>
        /// <param name="parMax1">Правая граница отрезка 1.</param>
        /// <param name="parMin2">Левая граница отрезка 2.</param>
        /// <param name="parMax2">Правая граница отрезка 2.</param>
        /// <returns>True, если отрезки пересекаются.</returns>
        private bool IsIntersected(float parMin1, float parMax1, float parMin2, float parMax2)
        {
            return ((parMax2 >= parMin1) && (parMax2 <= parMax1))
                || ((parMax2 > parMax1) && (parMin2 <= parMax1));
        }

        /// <summary>
        /// Перемещение камеры за игроком.
        /// </summary>
        /// <param name="parDeltaSeconds">Прошедшее с предыдуего шага время.</param>
        private void MoveCamera(float parDeltaSeconds)
        {
            float dX = (_model.Player.X + _model.Player.SizeX * 0.5f) - (_model.CameraX + _model.CameraSizeX * 0.5f);
            _cameraMovingX = (_cameraMovingX && (Math.Abs(dX) > (_model.CameraSizeX * CAMERA_DEACTIVATE_RANGE_X)))
                    || (Math.Abs(dX) > (_model.CameraSizeX * CAMERA_ACTIVATE_RANGE_X));
            if (_cameraMovingX)
            {
                if (dX > 0)
                {
                    float freeSpace = _model.SizeX - (_model.CameraX + _model.CameraSizeX);
                    if (dX > freeSpace)
                    {
                        dX = freeSpace;
                    }
                }
                else
                {
                    float freeSpace = -_model.CameraX;
                    if (dX < freeSpace)
                    {
                        dX = freeSpace;
                    }
                }
                float moveX = PlayerLogic.HORISONTAL_SPEED * parDeltaSeconds * Math.Sign(dX);
                if (Math.Abs(moveX) > Math.Abs(dX))
                {
                    moveX = dX;
                }
                _model.CameraX += moveX;
            }

            float dY = (_model.Player.Y + _model.Player.SizeY * 0.5f) - (_model.CameraY + _model.CameraSizeY * 0.5f);
            _cameraMovingY = (_cameraMovingY && (Math.Abs(dY) > (_model.CameraSizeY * CAMERA_DEACTIVATE_RANGE_Y)))
                    || (Math.Abs(dY) > (_model.CameraSizeY * CAMERA_ACTIVATE_RANGE_Y));
            if (_cameraMovingY)
            {
                if (dY > 0)
                {
                    float freeSpace = _model.SizeY - (_model.CameraY + _model.CameraSizeY);
                    if (dY > freeSpace)
                    {
                        dY = freeSpace;
                    }
                }
                else
                {
                    float freeSpace = -_model.CameraY;
                    if (dY < freeSpace)
                    {
                        dY = freeSpace;
                    }
                }
                float speedY = PlayerLogic.MAX_SPEED * (0.1f + Math.Abs(dY) / _model.CameraSizeY * 3f);
                float moveY = speedY * parDeltaSeconds * Math.Sign(dY);
                if (Math.Abs(moveY) > Math.Abs(dY))
                {
                    moveY = dY;
                }
                _model.CameraY += moveY;
            }
        }
    }
}
