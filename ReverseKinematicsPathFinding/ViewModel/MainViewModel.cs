using System;
using System.Collections.Generic;
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
        private ICommand _startAnimationCommand;

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
        private List<Tuple<int, int>> _configurations;

        private DispatcherTimer _timer;
        private DateTime _timerStart;

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

        public ICommand StartAnimationCommand { get { return _startAnimationCommand ?? (_startAnimationCommand = new DelegateCommand(StartAnimation)); } }

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

            ClearConfigurationData();
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500) };
            _timer.Tick += _timer_Tick;
        }

        #endregion Constructors

        #region Private Methods

        private void ClearConfigurationData()
        {
            ConfigurationSpaceImage = new Bitmap(360, 360);
            ReachableSpaceImage = new Bitmap(360, 360);
            _floodConfigurationSpace = new int[360, 360];
        }

        private List<Tuple<int, int>> ReversePathFinding(Tuple<int, int> startConfiguration, Tuple<int, int> endConfiguration)
        {
            var configurations = new List<Tuple<int, int>>();
            var currentConfiguration = endConfiguration;
            configurations.Add(currentConfiguration);

            while (_floodConfigurationSpace[currentConfiguration.Item1, currentConfiguration.Item2] != 1 &&
                (currentConfiguration.Item1 != startConfiguration.Item1 || currentConfiguration.Item2 != startConfiguration.Item2))
            {
                currentConfiguration = (new List<Tuple<int, int>>
                {
                    new Tuple<int, int>((currentConfiguration.Item1 + 1 + 360) % 360, currentConfiguration.Item2),
                    new Tuple<int, int>(currentConfiguration.Item1, (currentConfiguration.Item2 + 1 + 360) % 360),
                    new Tuple<int, int>((currentConfiguration.Item1 - 1 + 360) % 360, currentConfiguration.Item2),
                    new Tuple<int, int>(currentConfiguration.Item1, (currentConfiguration.Item2 - 1 + 360) % 360)
                }).First(i => _floodConfigurationSpace[i.Item1, i.Item2] == _floodConfigurationSpace[currentConfiguration.Item1, currentConfiguration.Item2] - 1);
                configurations.Add(currentConfiguration);
            }

            configurations.Reverse();
            return configurations;
        }

        private bool CalculateReachableConfiguration(out Tuple<int, int> startConfiguration, out Tuple<int, int> endConfiguration)
        {
            bool result;
            if (!(result = FindConfigurationForSingleArm(out startConfiguration, out endConfiguration, isFirstArm: true)))
                result = FindConfigurationForSingleArm(out startConfiguration, out endConfiguration, isFirstArm: false);

            OnPropertyChanged("ConfigurationSpaceImage");
            return result;
        }

        private bool FindConfigurationForSingleArm(out Tuple<int, int> startConfiguration, out Tuple<int, int> endConfiguration, bool isFirstArm)
        {
            var startAngles = isFirstArm ? Robot.CalculateReverseKinematicsFirstPosition(Robot.SecondPosition.X - Robot.ZeroPosition.X, Robot.SecondPosition.Y - Robot.ZeroPosition.Y)
                : Robot.CalculateReverseKinematicsSecondPosition(Robot.SecondPosition.X - Robot.ZeroPosition.X, Robot.SecondPosition.Y - Robot.ZeroPosition.Y);

            var endAngles = isFirstArm ? Robot.CalculateReverseKinematicsFirstPosition(Robot.DestinationPosition.X - Robot.ZeroPosition.X, Robot.DestinationPosition.Y - Robot.ZeroPosition.Y)
                : Robot.CalculateReverseKinematicsSecondPosition(Robot.DestinationPosition.X - Robot.ZeroPosition.X, Robot.DestinationPosition.Y - Robot.ZeroPosition.Y);

            if (double.IsNaN(startAngles.X) || double.IsNaN(startAngles.Y) || double.IsNaN(endAngles.X) ||
                double.IsNaN(endAngles.Y))
            {
                MessageBox.Show(string.Format("Cannot calculate {0} angles configuration", isFirstArm ? "first" : "second"));
                startConfiguration = endConfiguration = null;
                return false;
            }

            startConfiguration = new Tuple<int, int>((int)((startAngles.X * 180 / Math.PI) + 360) % 360, (int)((startAngles.Y * 180 / Math.PI) + 360) % 360);
            endConfiguration = new Tuple<int, int>((int)((endAngles.X * 180 / Math.PI) + 360) % 360, (int)((endAngles.Y * 180 / Math.PI) + 360) % 360);
            bool foundPath = FloodFill(startConfiguration.Item1, startConfiguration.Item2, endConfiguration.Item1, endConfiguration.Item2, 0, isFirstArm);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    ConfigurationSpaceImage.SetPixel(startConfiguration.Item1 + i, startConfiguration.Item2 + j, isFirstArm ? Color.Lime : Color.Magenta);
                    ConfigurationSpaceImage.SetPixel(endConfiguration.Item1 + i, endConfiguration.Item2 + j, isFirstArm ? Color.Lime : Color.Magenta);
                }

            return foundPath;
        }

        private bool FloodFill(int startX, int startY, int endX, int endY, int sequenceNumber, bool isFirstArm)
        {
            var neighbours = new List<Tuple<int, int>>();
            neighbours.Add(new Tuple<int, int>(startX, startY));

            do
            {
                sequenceNumber++;
                var color = Math.Min(255, sequenceNumber + 1);

                var newNeighbours = new List<Tuple<int, int>>();
                foreach (var neighbour in neighbours)
                {
                    startX = neighbour.Item1;
                    startY = neighbour.Item2;
                    if (_floodConfigurationSpace[(startX - 1 + 360) % 360, startY] == 0)
                    {
                        _floodConfigurationSpace[(startX - 1 + 360) % 360, startY] = sequenceNumber;
                        ConfigurationSpaceImage.SetPixel((startX - 1 + 360) % 360, startY, Color.FromArgb(color, color, color));
                        newNeighbours.Add(new Tuple<int, int>((startX - 1 + 360) % 360, startY));
                        if ((startX - 1 + 360) % 360 == endX && startY == endY) return true;
                    }
                    if (_floodConfigurationSpace[startX, (startY + 1) % 360] == 0)
                    {
                        _floodConfigurationSpace[startX, (startY + 1) % 360] = sequenceNumber;
                        ConfigurationSpaceImage.SetPixel(startX, (startY + 1) % 360, Color.FromArgb(color, color, color));
                        newNeighbours.Add(new Tuple<int, int>(startX, (startY + 1) % 360));
                        if (startX == endX && (startY + 1) % 360 == endY) return true;
                    }
                    if (_floodConfigurationSpace[startX, (startY - 1 + 360) % 360] == 0)
                    {
                        _floodConfigurationSpace[startX, (startY - 1 + 360) % 360] = sequenceNumber;
                        ConfigurationSpaceImage.SetPixel(startX, (startY - 1 + 360) % 360, Color.FromArgb(color, color, color));
                        newNeighbours.Add(new Tuple<int, int>(startX, (startY - 1 + 360) % 360));
                        if (startX == endX && (startY - 1 + 360) % 360 == endY) return true;
                    }
                    if (_floodConfigurationSpace[(startX + 1) % 360, startY] == 0)
                    {
                        _floodConfigurationSpace[(startX + 1) % 360, startY] = sequenceNumber;
                        ConfigurationSpaceImage.SetPixel((startX + 1) % 360, startY, Color.FromArgb(color, color, color));
                        newNeighbours.Add(new Tuple<int, int>((startX + 1) % 360, startY));
                        if ((startX + 1) % 360 == endX && startY == endY) return true;
                    }
                }
                neighbours = newNeighbours;
            } while (neighbours.Any());

            MessageBox.Show(string.Format("Cannot calculate path for {0} arm", isFirstArm ? "first" : "second"));
            return false;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
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
                    }
                    else
                        ReachableSpaceImage.SetPixel(i, j, Color.RoyalBlue);
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
                Robot.ThirdPosition = Robot.CalculateFirstPosition(angles.X);
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
                var delta = _mouseDownPosition - new Point(Width / 2.0, Height / 2.0);
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

                Obstacles.Add(new Obstacle { Position = _mouseDownPosition, Size = new Point(10, 10) });
            }
            else if (Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                var position = Mouse.GetPosition((IInputElement)obj);
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

        private void CalculatePath(object obj)
        {
            ClearConfigurationData();
            CalculateConfiguration();

            Tuple<int, int> startConfiguration, endConfiguration;
            if (CalculateReachableConfiguration(out startConfiguration, out endConfiguration))
            {
                _configurations = ReversePathFinding(startConfiguration, endConfiguration);
                foreach (var configuration in _configurations)
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            ConfigurationSpaceImage.SetPixel((configuration.Item1 + i + 360) % 360, (configuration.Item2 + j + 360) % 360, Color.Goldenrod);
                }
                OnPropertyChanged("ConfigurationSpaceImage");
            }
        }

        private void StartAnimation(object obj)
        {
            if (_configurations == null) return;

            _timerStart = DateTime.Now;
            _timer.Start();
        }

        #endregion Private Methods
    }
}