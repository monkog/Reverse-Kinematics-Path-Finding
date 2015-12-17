using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ReverseKinematicsPathFinding.Model;
using Color = System.Drawing.Color;

namespace ReverseKinematicsPathFinding.ViewModel
{
    using System.Windows.Input;
    using System.Windows;

    public class MainViewModel : ViewModelBase
    {
        #region Private Members

        private ObservableCollection<Obstacle> _obstacles;

        private ICommand _calculatePathCommand;

        private ICommand _mouseDownCommand;
        private ICommand _mouseMoveCommand;
        private ICommand _mouseUpCommand;

        private bool _isMouseDown;
        private Point _lastMousePosition;
        private Point _mouseDownPosition;

        private Bitmap _configurationSpaceImage;

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

        public ICommand CalculatePathCommand { get { return _calculatePathCommand ?? (_calculatePathCommand = new DelegateCommand(CalculatePath)); } }

        public ICommand MouseDownCommand { get { return _mouseDownCommand ?? (_mouseDownCommand = new DelegateCommand(MouseDown)); } }

        public ICommand MouseMoveCommand { get { return _mouseMoveCommand ?? (_mouseMoveCommand = new DelegateCommand(MouseMove)); } }

        public ICommand MouseUpCommand { get { return _mouseUpCommand ?? (_mouseUpCommand = new DelegateCommand(MouseUp)); } }

        #endregion Public Members

        #region Constructors

        public MainViewModel()
        {
            Obstacles = new ObservableCollection<Obstacle>();
            Robot = new Robot();

            ConfigurationSpaceImage = new Bitmap(360, 360);
        }

        #endregion Constructors

        #region Public Methods

        private void CalculatePath(object obj)
        {
            CalculateConfiguration();

        }

        private void CalculateConfiguration()
        {
            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    bool intersectsObstacle = false;
                    foreach (var obstacle in Obstacles)
                    {
                        if (Robot.IntersectsRectangle(Robot.ZeroPosition, Robot.CalculateFirstPosition(i, j), obstacle))
                            intersectsObstacle = true;
                        if (Robot.IntersectsRectangle(Robot.CalculateFirstPosition(i, j), Robot.CalculateSecondPosition(i, j), obstacle))
                            intersectsObstacle = true;
                    }

                    if (intersectsObstacle) ConfigurationSpaceImage.SetPixel(i, j, Color.Red);
                    else ConfigurationSpaceImage.SetPixel(i, j, Color.Aqua);
                }
            }
            OnPropertyChanged("ConfigurationSpaceImage");
        }

        private void MouseDown(object obj)
        {
            _mouseDownPosition = _lastMousePosition = Mouse.GetPosition((IInputElement)obj);
            _isMouseDown = true;

            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (double.IsNaN(Robot.L1))
                    Robot.FirstPosition = _mouseDownPosition;
                else if (double.IsNaN(Robot.L2))
                    Robot.SecondPosition = _mouseDownPosition;
                else 
                    Robot.DestinationPosition = _mouseDownPosition;
            }
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Obstacles.Add(new Obstacle { Position = _mouseDownPosition, Size = new Point(10, 10) });
            }
        }

        private void MouseMove(object obj)
        {
            if (!_isMouseDown) return;

            if (Mouse.RightButton == MouseButtonState.Pressed)
            {

            }
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var size = Mouse.GetPosition((IInputElement)obj) - _mouseDownPosition;
                Obstacles.Last().Size = new Point(size.X, size.Y);
            }

            _lastMousePosition = Mouse.GetPosition((IInputElement)obj);
        }

        private void MouseUp(object obj)
        {
            _isMouseDown = false;
        }

        #endregion Public Methods
    }
}