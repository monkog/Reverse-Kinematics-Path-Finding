using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Threading;
using ReverseKinematicsPathFinding.Model;
using Color = System.Drawing.Color;

namespace ReverseKinematicsPathFinding.ViewModel
{
    using System.Windows.Input;
    using System.Windows;

    public class MainViewModel : ViewModelBase
    {
        #region Private Members

        private const double Width = 1200;
        private const double Height = 837;

        private ObservableCollection<Obstacle> _obstacles;

        private ICommand _calculatePathCommand;

        private ICommand _mouseDownCommand;
        private ICommand _mouseMoveCommand;
        private ICommand _mouseUpCommand;
        private ICommand _deleteCommand;

        private bool _isMouseDown;
        private Point _lastMousePosition;
        private Point _mouseDownPosition;

        private Bitmap _configurationSpaceImage;
        private Bitmap _reachableSpaceImage;

        private int[,] _floodConfigurationSpace;

        private DispatcherTimer _timer;
        private int _ticks;

        private Obstacle _currentObstacle;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Collection of the obstacles.
        /// </summary>
        public ObservableCollection<Obstacle> Obstacles
        {
            get { return _obstacles; }
            set
            {
                if (_obstacles == value) return;
                _obstacles = value;
                OnPropertyChanged("Obstacles");
            }
        }

        /// <summary>
        /// The robot.
        /// </summary>
        public Robot Robot { get; private set; }

        /// <summary>
        /// Gets the configuration space image.
        /// </summary>
        public Bitmap ConfigurationSpaceImage
        {
            get { return _configurationSpaceImage; }
            private set
            {
                if (_configurationSpaceImage == value) return;
                _configurationSpaceImage = value;
                OnPropertyChanged("ConfigurationSpaceImage");
            }
        }

        /// <summary>
        /// Gets the reachable space image.
        /// </summary>
        public Bitmap ReachableSpaceImage
        {
            get { return _reachableSpaceImage; }
            private set
            {
                if (_reachableSpaceImage == value) return;
                _reachableSpaceImage = value;
                OnPropertyChanged("ReachableSpaceImage");
            }
        }

        public ICommand CalculatePathCommand { get { return _calculatePathCommand ?? (_calculatePathCommand = new DelegateCommand(CalculatePath)); } }

        public ICommand MouseDownCommand { get { return _mouseDownCommand ?? (_mouseDownCommand = new DelegateCommand(MouseDown)); } }

        public ICommand MouseMoveCommand { get { return _mouseMoveCommand ?? (_mouseMoveCommand = new DelegateCommand(MouseMove)); } }

        public ICommand MouseUpCommand { get { return _mouseUpCommand ?? (_mouseUpCommand = new DelegateCommand(MouseUp)); } }

        public ICommand DeleteCommand { get { return _deleteCommand ?? (_deleteCommand = new DelegateCommand(Delete)); } }

        #endregion Public Members

        #region Constructors

        public MainViewModel()
        {
            Obstacles = new ObservableCollection<Obstacle>();
            Robot = new Robot(Width, Height);

            ConfigurationSpaceImage = new Bitmap(360, 360);
            ReachableSpaceImage = new Bitmap(360, 360);
            _floodConfigurationSpace = new int[360, 360];
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1, 500) };
            _timer.Tick += _timer_Tick;
            _ticks = 0;
        }

        #endregion Constructors

        #region Private Methods

        private void CalculatePath(object obj)
        {
            CalculateConfiguration();
            //CalculateReachableConfiguration();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CalculateConfiguration()
        {
            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    var alpha = i * Math.PI / 180.0;
                    var beta = j * Math.PI / 180.0;
                    bool intersectsObstacle = false;
                    var p1 = Robot.CalculateFirstPosition(alpha);
                    var p2 = Robot.CalculateSecondPosition(p1, alpha, beta);
                    foreach (var obstacle in Obstacles)
                    {
                        if (Robot.IntersectsRectangle(Robot.ZeroPosition, p1, obstacle))
                            intersectsObstacle = true;
                        if (Robot.IntersectsRectangle(p1, p2, obstacle))
                            intersectsObstacle = true;
                    }

                    if (intersectsObstacle)
                    {
                        ReachableSpaceImage.SetPixel(i, j, Color.DarkRed);
                        _floodConfigurationSpace[i, j] = -1;
                        ConfigurationSpaceImage.SetPixel(i, j, Color.DarkRed);
                        //if (p2.X < 0 || p2.Y < 0 || p2.X >= Width || p2.Y >= Height) continue;
                        //ConfigurationSpaceImage.SetPixel((int)(p2.X * 360 / Width), (int)(p2.Y * 360 / Height), Color.DarkRed);
                    }
                    else
                    {
                        ReachableSpaceImage.SetPixel(i, j, Color.RoyalBlue);
                        if (p2.X < 0 || p2.Y < 0 || p2.X >= Width || p2.Y >= Height) continue;
                        //ConfigurationSpaceImage.SetPixel((int)(p2.X * 360 / Width), (int)(p2.Y * 360 / Height), Color.RoyalBlue);
                    }
                }
            }
            OnPropertyChanged("ConfigurationSpaceImage");
            OnPropertyChanged("ReachableSpaceImage");
        }

        private void FindSolution(Point destination, bool isFirstSolution)
        {
            Point angles;
            if (isFirstSolution) angles = Robot.CalculateReverseKinematicsFirstPosition(destination.X, destination.Y);
            else angles = Robot.CalculateReverseKinematicsSecondPosition(destination.X, destination.Y);

            if (double.IsNaN(angles.X) || double.IsNaN(angles.Y)) return;

            if (isFirstSolution)
            {
                var p1 = Robot.CalculateFirstPosition(angles.X);
                var p2 = Robot.CalculateSecondPosition(p1, angles.X, angles.Y);

                Robot.FirstPosition = p1;
                Robot.SecondPosition = p2;
            }
            else
            {
                var p1 = Robot.CalculateFirstPosition(angles.X);
                var p2 = Robot.CalculateSecondPosition(p1, angles.X, angles.Y);

                Robot.ThirdPosition = p1;
                Robot.FourthPosition = p2;
            }
        }

        private void SetRobotArms()
        {
            if (_currentObstacle != null) return;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Robot.DestinationPosition = _mouseDownPosition;
                return;
            }

            if (double.IsNaN(Robot.L1))
            {
                Robot.FirstPosition = _mouseDownPosition;
                Robot.RecalculateRobot();
            }
            else if (double.IsNaN(Robot.L2))
            {
                Robot.SecondPosition = _mouseDownPosition;
                Robot.RecalculateRobot();
            }
            else
            {
                var delta = _mouseDownPosition - new Point(Width/2.0, Height/2.0);
                FindSolution(new Point(delta.X, delta.Y), isFirstSolution: true);
                FindSolution(new Point(delta.X, delta.Y), isFirstSolution: false);
            }
        }

        private void MouseDown(object obj)
        {
            _mouseDownPosition = _lastMousePosition = Mouse.GetPosition((IInputElement)obj);
            _isMouseDown = true;

            if (Mouse.RightButton == MouseButtonState.Pressed)
                SetRobotArms();
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentObstacle != null) return;

                Obstacles.Add(new Obstacle {Position = _mouseDownPosition, Size = new Point(10, 10)});
            }
            else if (Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                var position = Mouse.GetPosition((IInputElement) obj);
                foreach (var obstacle in Obstacles)
                    obstacle.IsSelected = false;

                _currentObstacle = Obstacles.FirstOrDefault(o => o.Position.X < position.X && o.Position.Y < position.Y
                                                                 && o.Position.X + o.Size.X > position.X &&
                                                                 o.Position.Y + o.Size.Y > position.Y);
                if (_currentObstacle != null) _currentObstacle.IsSelected = true;
            }
        }

        private void MouseMove(object obj)
        {
            if (!_isMouseDown) return;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentObstacle != null)
                {
                    var size = Mouse.GetPosition((IInputElement)obj) - _lastMousePosition;
                    _currentObstacle.Size = new Point(_currentObstacle.Size.X + size.X, _currentObstacle.Size.Y + size.Y);
                }
                else
                {
                    var size = Mouse.GetPosition((IInputElement)obj) - _mouseDownPosition;
                    Obstacles.Last().Size = new Point(size.X, size.Y);
                }
            }
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (_currentObstacle != null)
                {
                    var delta = Mouse.GetPosition((IInputElement)obj) - _lastMousePosition;
                    _currentObstacle.Position = _currentObstacle.Position + delta;
                }
            }

            _lastMousePosition = Mouse.GetPosition((IInputElement)obj);
        }

        private void MouseUp(object obj)
        {
            _isMouseDown = false;
        }

        private void Delete(object obj)
        {
            if (_currentObstacle != null)
            {
                Obstacles.Remove(_currentObstacle);
                _currentObstacle = null;
            }
        }

        #endregion Private Methods

        #region Public Methods

        #endregion Public Methods
    }
}