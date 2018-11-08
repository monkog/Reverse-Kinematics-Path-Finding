using System;
using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding.Model
{
	public class Arm : ViewModelBase
	{
		private Point _start;

		private Point _joint;

		private Point _alternativeJoint;

		private Point _end;

		/// <summary>
		/// Gets the arm's start position.
		/// </summary>
		public Point Start
		{
			get { return _start; }
			private set
			{
				if (_start == value) return;
				_start = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the arm's joint position.
		/// </summary>
		public Point Joint
		{
			get { return _joint; }
			private set
			{
				if (_joint == value) return;
				_joint = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the arm's joint position.
		/// </summary>
		public Point AlternativeJoint
		{
			get { return _alternativeJoint; }
			private set
			{
				if (_alternativeJoint == value) return;
				_alternativeJoint = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the arm's end position.
		/// </summary>
		public Point End
		{
			get { return _end; }
			private set
			{
				if (_end == value) return;
				_end = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the first part length.
		/// </summary>
		public double FirstPartLength { get; private set; }

		/// <summary>
		/// Gets the second part length.
		/// </summary>
		public double SecondPartLength { get; private set; }

		/// <summary>
		/// Gets the minimum range of arms.
		/// </summary>
		public double MinRange { get; private set; }

		/// <summary>
		/// Gets the maximum range of arms.
		/// </summary>
		public double MaxRange { get; private set; }

		public Arm(Point start)
		{
			_start = start;
			Joint = start;
			AlternativeJoint = start;
			End = start;
		}

		/// <summary>
		/// Sets the arm position.
		/// </summary>
		/// <param name="position">New end position.</param>
		public void SetArmPosition(Point position)
		{
			if (MaxRange > 0 && (position - Start).Length > MaxRange * 0.5)
				return;

			if (FirstPartLength == 0.0)
			{
				Joint = position;
				End = position;
				FirstPartLength = (Joint - Start).Length;
				return;
			}

			if (SecondPartLength == 0.0)
			{
				End = position;
				SecondPartLength = (End - Joint).Length;
				MaxRange = (FirstPartLength + SecondPartLength) * 2;
				MinRange = Math.Abs(FirstPartLength - SecondPartLength) * 2;
			}

			RecalculateJoints(position);
			NotifyArmsChanged();
		}

		private void RecalculateJoints(Point position)
		{
			var destination = position - Start;
			var firstSolution = CalculateReverseKinematics(destination.X, destination.Y, false);

			var p1 = CalculateFirstPosition(firstSolution.X);
			var p2 = CalculateSecondPosition(p1, firstSolution.X, firstSolution.Y);

			var secondSolution = CalculateReverseKinematics(destination.X, destination.Y, true);

			var p11 = CalculateFirstPosition(secondSolution.X);
			var p12 = CalculateSecondPosition(p11, secondSolution.X, secondSolution.Y);

			if ((p2 - p12).Length > 0.00001 || double.IsNaN(p2.X) || double.IsNaN(p2.Y)) return;

			Joint = p1;
			End = p2;
			AlternativeJoint = p11;
		}

		private void NotifyArmsChanged()
		{
			OnPropertyChanged(nameof(MaxRange));
			OnPropertyChanged(nameof(MinRange));
		}

		private Point CalculateFirstPosition(double alpha)
		{
			return new Point(Start.X + (FirstPartLength * Math.Cos(alpha)), Start.Y + (FirstPartLength * Math.Sin(alpha)));
		}

		private Point CalculateSecondPosition(Point firstPosition, double alpha, double beta)
		{
			return new Point(firstPosition.X + (SecondPartLength * (((Math.Cos(beta) * Math.Cos(alpha))) + (Math.Sin(beta) * Math.Sin(alpha)))),
				firstPosition.Y + (SecondPartLength * (-(Math.Sin(beta) * Math.Cos(alpha)) + (Math.Cos(beta) * Math.Sin(alpha)))));
		}

		private Point CalculateReverseKinematics(double x, double y, bool isAlternative)
		{
			var coefficient = isAlternative ? 1 : -1;
			var beta = -1 * coefficient * Math.Acos((x * x + y * y - FirstPartLength * FirstPartLength - SecondPartLength * SecondPartLength) / (2 * FirstPartLength * SecondPartLength));
			var alpha = coefficient * Math.Asin((SecondPartLength * Math.Sin(coefficient * beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
			return new Point(alpha, beta);
		}
	}
}