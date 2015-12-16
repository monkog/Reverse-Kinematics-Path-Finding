using System.Collections.ObjectModel;
using System.Linq;
using ReverseKinematicsPathFinding.Model;

namespace ReverseKinematicsPathFinding.ViewModel
{
    using System.Collections.Generic;
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

        private double _viewportWidth;
        private double _viewportHeight;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public double ViewportWidth
        {
            get { return _viewportWidth; }
            set
            {
                if (_viewportWidth == value) return;
                _viewportWidth = value;
                OnPropertyChanged("ViewportWidth");
            }
        }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public double ViewportHeight
        {
            get { return _viewportHeight; }
            set
            {
                if (_viewportHeight == value) return;
                _viewportHeight = value;
                OnPropertyChanged("ViewportHeight");
            }
        }

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
        }

        #endregion Constructors

        #region Constructors

        private void CalculatePath(object obj)
        {

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

        #endregion Constructors
    }
}