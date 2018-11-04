using System;
using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding.Model
{
	public class Robot : ViewModelBase
	{
		private Arm _arm;

		private Arm _animationArm;

		private Point _position;
		private Point _destination;
		
		/// <summary>
		/// The robot's position.
		/// </summary>
		public Point Position
		{
			get { return _position; }
			set
			{
				if (_position == value) return;
				_position = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The robot's first arm.
		/// </summary>
		public Arm Arm
		{
			get { return _arm; }
			set
			{
				if (_arm == value) return;
				_arm = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Animation arm.
		/// </summary>
		public Arm AnimationArm
		{
			get { return _animationArm; }
			set
			{
				if (_animationArm == value) return;
				_animationArm = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Destination position of the robot's second arm.
		/// </summary>
		public Point Destination
		{
			get { return _destination; }
			set
			{
				if (_destination == value) return;
				_destination = value;
				OnPropertyChanged();
			}
		}
		
		#region Constructors

		public Robot(double width, double height)
		{
			_position = new Point(width / 2.0, height / 2.0);
			Arm = new Arm(_position);
			Destination = _position;
		}

		#endregion Constructors

		#region Public Methods

		public bool IntersectsRectangle(Point p1, Point p2, Obstacle r)
		{
			return LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y)) ||
				   LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ||
				   LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y + r.Size.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ||
				   LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X, r.Position.Y + r.Size.Y)) ||
				   (r.Contains(p1) || r.Contains(p2));
		}

		public Point CalculateFirstPosition(double alpha)
		{
			return new Point(Position.X + (Arm.FirstPartLength * Math.Cos(alpha)), Position.Y + (Arm.FirstPartLength * Math.Sin(alpha)));
		}

		public Point CalculateSecondPosition(Point firstPosition, double alpha, double beta)
		{
			return new Point(firstPosition.X + (Arm.SecondPartLength * (((Math.Cos(beta) * Math.Cos(alpha))) + (Math.Sin(beta) * Math.Sin(alpha)))),
				firstPosition.Y + (Arm.SecondPartLength * (-(Math.Sin(beta) * Math.Cos(alpha)) + (Math.Cos(beta) * Math.Sin(alpha)))));
		}

		public Point CalculateReverseKinematicsFirstPosition(double x, double y)
		{
			var beta = -Math.Acos((x * x + y * y - Arm.FirstPartLength * Arm.FirstPartLength - Arm.SecondPartLength * Arm.SecondPartLength) / (2 * Arm.FirstPartLength * Arm.SecondPartLength));
			var alpha = Math.Asin((Arm.SecondPartLength * Math.Sin(beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
			return new Point(alpha, beta);
		}

		public Point CalculateReverseKinematicsSecondPosition(double x, double y)
		{
			var beta = Math.Acos((x * x + y * y - Arm.FirstPartLength * Arm.FirstPartLength - Arm.SecondPartLength * Arm.SecondPartLength) / (2 * Arm.FirstPartLength * Arm.SecondPartLength));
			var alpha = -Math.Asin((Arm.SecondPartLength * Math.Sin(-beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
			return new Point(alpha, beta);
		}

		#endregion Public Methods

		#region Private Methods

		private static bool LineIntersectsLine(Point l1P1, Point l1P2, Point l2P1, Point l2P2)
		{
			double q = (l1P1.Y - l2P1.Y) * (l2P2.X - l2P1.X) - (l1P1.X - l2P1.X) * (l2P2.Y - l2P1.Y);
			double d = (l1P2.X - l1P1.X) * (l2P2.Y - l2P1.Y) - (l1P2.Y - l1P1.Y) * (l2P2.X - l2P1.X);

			if (d == 0) return false;

			double r = q / d;

			q = (l1P1.Y - l2P1.Y) * (l1P2.X - l1P1.X) - (l1P1.X - l2P1.X) * (l1P2.Y - l1P1.Y);
			double s = q / d;

			if (r < 0 || r > 1 || s < 0 || s > 1) return false;

			return true;
		}

		#endregion Private Methods
	}
}