using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ReverseKinematicsPathFinding.Model;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace ReverseKinematicsPathFinding.ViewModel
{
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

		private readonly DispatcherTimer _timer;
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
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The robot.
		/// </summary>
		public Robot Robot { get; }

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
				OnPropertyChanged();
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
				OnPropertyChanged();
			}
		}

		public ICommand StartAnimationCommand { get { return _startAnimationCommand ?? (_startAnimationCommand = new DelegateCommand(StartAnimation)); } }

		public ICommand CalculatePathCommand { get { return _calculatePathCommand ?? (_calculatePathCommand = new DelegateCommand(CalculatePath)); } }

		public ICommand MouseDownCommand { get { return _mouseDownCommand ?? (_mouseDownCommand = new DelegateCommand(MouseDown)); } }

		public ICommand MouseMoveCommand { get { return _mouseMoveCommand ?? (_mouseMoveCommand = new DelegateCommand(MouseMove)); } }

		public ICommand MouseUpCommand { get { return _mouseUpCommand ?? (_mouseUpCommand = new DelegateCommand(MouseUp)); } }

		public ICommand DeleteCommand { get { return _deleteCommand ?? (_deleteCommand = new DelegateCommand(RemoveObstacle)); } }

		#endregion Public Members

		#region Constructors

		public MainViewModel()
		{
			Obstacles = new ObservableCollection<Obstacle>();
			Robot = new Robot(Width, Height);

			_timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50) };
			_timer.Tick += TimerTick;
			ClearConfigurationData();
		}

		#endregion Constructors

		#region Private Methods

		private void ClearConfigurationData()
		{
			ConfigurationSpaceImage = new Bitmap(360, 360);
			ReachableSpaceImage = new Bitmap(360, 360);
			_floodConfigurationSpace = new int[360, 360];

			_timer.Stop();
			_configurations = null;
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

			OnPropertyChanged(nameof(ConfigurationSpaceImage));
			return result;
		}

		private bool FindConfigurationForSingleArm(out Tuple<int, int> startConfiguration, out Tuple<int, int> endConfiguration, bool isFirstArm)
		{
			var startAngles = isFirstArm ? Robot.CalculateReverseKinematicsFirstPosition(Robot.FirstArm.EndPosition.X - Robot.Position.X, Robot.FirstArm.EndPosition.Y - Robot.Position.Y)
				: Robot.CalculateReverseKinematicsSecondPosition(Robot.FirstArm.EndPosition.X - Robot.Position.X, Robot.FirstArm.EndPosition.Y - Robot.Position.Y);

			var endAngles = isFirstArm ? Robot.CalculateReverseKinematicsFirstPosition(Robot.Destination.X - Robot.Position.X, Robot.Destination.Y - Robot.Position.Y)
				: Robot.CalculateReverseKinematicsSecondPosition(Robot.Destination.X - Robot.Position.X, Robot.Destination.Y - Robot.Position.Y);

			if (double.IsNaN(startAngles.X) || double.IsNaN(startAngles.Y) || double.IsNaN(endAngles.X) ||
				double.IsNaN(endAngles.Y))
			{
				MessageBox.Show($"Cannot calculate {(isFirstArm ? "first" : "second")} angles configuration");
				startConfiguration = endConfiguration = null;
				return false;
			}

			startConfiguration = new Tuple<int, int>((int)((startAngles.X * 180 / Math.PI) + 360) % 360, (int)((startAngles.Y * 180 / Math.PI) + 360) % 360);
			endConfiguration = new Tuple<int, int>((int)((endAngles.X * 180 / Math.PI) + 360) % 360, (int)((endAngles.Y * 180 / Math.PI) + 360) % 360);
			bool foundPath = FloodFill(startConfiguration.Item1, startConfiguration.Item2, endConfiguration.Item1, endConfiguration.Item2, 0, isFirstArm);

			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 2; j++)
				{
					ConfigurationSpaceImage.SetPixel((startConfiguration.Item1 + i + 360) % 360, (startConfiguration.Item2 + j + 360) % 360, isFirstArm ? Color.Lime : Color.Magenta);
					ConfigurationSpaceImage.SetPixel((endConfiguration.Item1 + i + 360) % 360, (endConfiguration.Item2 + j + 360) % 360, isFirstArm ? Color.Lime : Color.Magenta);
				}

			return foundPath;
		}

		private bool FloodFill(int startX, int startY, int endX, int endY, int sequenceNumber, bool isFirstArm)
		{
			var neighbors = new List<Tuple<int, int>> { new Tuple<int, int>(startX, startY) };

			do
			{
				sequenceNumber++;
				var color = Math.Min(255, sequenceNumber + 1);

				var newNeighbors = new List<Tuple<int, int>>();
				foreach (var neighbor in neighbors)
				{
					startX = neighbor.Item1;
					startY = neighbor.Item2;
					if (_floodConfigurationSpace[(startX - 1 + 360) % 360, startY] == 0)
					{
						_floodConfigurationSpace[(startX - 1 + 360) % 360, startY] = sequenceNumber;
						ConfigurationSpaceImage.SetPixel((startX - 1 + 360) % 360, startY, Color.FromArgb(color, color, color));
						newNeighbors.Add(new Tuple<int, int>((startX - 1 + 360) % 360, startY));
						if ((startX - 1 + 360) % 360 == endX && startY == endY) return true;
					}
					if (_floodConfigurationSpace[startX, (startY + 1) % 360] == 0)
					{
						_floodConfigurationSpace[startX, (startY + 1) % 360] = sequenceNumber;
						ConfigurationSpaceImage.SetPixel(startX, (startY + 1) % 360, Color.FromArgb(color, color, color));
						newNeighbors.Add(new Tuple<int, int>(startX, (startY + 1) % 360));
						if (startX == endX && (startY + 1) % 360 == endY) return true;
					}
					if (_floodConfigurationSpace[startX, (startY - 1 + 360) % 360] == 0)
					{
						_floodConfigurationSpace[startX, (startY - 1 + 360) % 360] = sequenceNumber;
						ConfigurationSpaceImage.SetPixel(startX, (startY - 1 + 360) % 360, Color.FromArgb(color, color, color));
						newNeighbors.Add(new Tuple<int, int>(startX, (startY - 1 + 360) % 360));
						if (startX == endX && (startY - 1 + 360) % 360 == endY) return true;
					}
					if (_floodConfigurationSpace[(startX + 1) % 360, startY] == 0)
					{
						_floodConfigurationSpace[(startX + 1) % 360, startY] = sequenceNumber;
						ConfigurationSpaceImage.SetPixel((startX + 1) % 360, startY, Color.FromArgb(color, color, color));
						newNeighbors.Add(new Tuple<int, int>((startX + 1) % 360, startY));
						if ((startX + 1) % 360 == endX && startY == endY) return true;
					}
				}
				neighbors = newNeighbors;
			} while (neighbors.Any());

			MessageBox.Show($"Cannot calculate path for {(isFirstArm ? "first" : "second")} arm");
			return false;
		}

		private void TimerTick(object sender, EventArgs e)
		{
			var timeDelta = (int)((DateTime.Now - _timerStart).TotalMilliseconds / 50);
			if (timeDelta >= _configurations.Count) return;

			var currentPosition = _configurations.ElementAt(timeDelta);
			var p1 = Robot.CalculateFirstPosition(currentPosition.Item1 * Math.PI / 180.0);
			var p2 = Robot.CalculateSecondPosition(p1, currentPosition.Item1 * Math.PI / 180.0, currentPosition.Item2 * Math.PI / 180.0);

			Robot.AnimationArm.SetJointPosition(p1);
			Robot.AnimationArm.SetEndPosition(p2);
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
						if (Robot.IntersectsRectangle(Robot.Position, p1, obstacle))
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
			OnPropertyChanged(nameof(ConfigurationSpaceImage));
			OnPropertyChanged(nameof(ReachableSpaceImage));
		}

		private void FindSolution(Point destination, bool isFirstSolution)
		{
			Point angles;
			if (isFirstSolution) angles = Robot.CalculateReverseKinematicsFirstPosition(destination.X, destination.Y);
			else angles = Robot.CalculateReverseKinematicsSecondPosition(destination.X, destination.Y);

			if (double.IsNaN(angles.X) || double.IsNaN(angles.Y)) return;

			var p1 = Robot.CalculateFirstPosition(angles.X);
			var p2 = Robot.CalculateSecondPosition(p1, angles.X, angles.Y);

			if (isFirstSolution)
			{
				Robot.FirstArm.SetJointPosition(p1);
				Robot.FirstArm.SetEndPosition(p2);
			}
			else
			{
				Robot.SecondArm.SetJointPosition(p1);
				Robot.SecondArm.SetEndPosition(p2);
			}
		}

		private void SetRobotArms()
		{
			if (_currentObstacle != null) return;
			if (Keyboard.IsKeyDown(Key.LeftShift))
			{
				Robot.Destination = _mouseDownPosition;
				return;
			}

			if (Robot.FirstArm.JointPosition == Robot.FirstArm.StartPosition)
			{
				Robot.FirstArm.SetJointPosition(_mouseDownPosition);
			}
			else if (Robot.FirstArm.EndPosition == Robot.FirstArm.StartPosition)
			{
				Robot.FirstArm.SetEndPosition(_mouseDownPosition);
				Robot.SecondArm.SetEndPosition(_mouseDownPosition);
				var delta = Robot.FirstArm.JointPosition - Robot.FirstArm.StartPosition;
				var angles = Robot.CalculateReverseKinematicsSecondPosition(delta.X, delta.Y);
				var p1 = Robot.CalculateFirstPosition(angles.X);
				var p2 = Robot.CalculateSecondPosition(p1, angles.X, angles.Y);
				Robot.SecondArm.SetJointPosition(p2);
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

				Obstacles.Add(new Obstacle(_mouseDownPosition, new Point(10, 10)));
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
					_currentObstacle.Resize(_currentObstacle.Size.X + size.X, _currentObstacle.Size.Y + size.Y);
				}
				else
				{
					var size = Mouse.GetPosition((IInputElement)obj) - _mouseDownPosition;
					Obstacles.Last().Resize(size.X, size.Y);
				}
			}
			if (Mouse.RightButton == MouseButtonState.Pressed)
			{
				if (_currentObstacle != null)
				{
					var delta = Mouse.GetPosition((IInputElement)obj) - _lastMousePosition;
					_currentObstacle.Move(delta);
				}
			}

			_lastMousePosition = Mouse.GetPosition((IInputElement)obj);
		}

		private void MouseUp(object obj)
		{
			_isMouseDown = false;
		}

		private void RemoveObstacle(object obj)
		{
			if (_currentObstacle == null) return;

			Obstacles.Remove(_currentObstacle);
			_currentObstacle = null;
		}

		private void CalculatePath(object obj)
		{
			ClearConfigurationData();
			CalculateConfiguration();

			if (!CalculateReachableConfiguration(out var startConfiguration, out var endConfiguration)) return;
			_configurations = ReversePathFinding(startConfiguration, endConfiguration);
			foreach (var configuration in _configurations)
			{
				for (int i = 0; i < 2; i++)
					for (int j = 0; j < 2; j++)
						ConfigurationSpaceImage.SetPixel((configuration.Item1 + i + 360) % 360, (configuration.Item2 + j + 360) % 360, Color.Goldenrod);
			}
			OnPropertyChanged(nameof(ConfigurationSpaceImage));
		}

		private void StartAnimation(object obj)
		{
			if (_configurations == null) return;

			_timerStart = DateTime.Now;
			Robot.AnimationArm = new Arm(Robot.Position);
			_timer.Start();
		}

		#endregion Private Methods
	}
}