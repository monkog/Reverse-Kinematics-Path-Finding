using System;
using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding.Model
{
	public class Arm:ViewModelBase
	{
		private Point _startPosition;

		private Point _jointPosition;

		private Point _endPosition;

		/// <summary>
		/// Gets the arm's start position.
		/// </summary>
		public Point StartPosition
		{
			get { return _startPosition; }
			private set
			{
				if (_startPosition == value) return;
				_startPosition = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the arm's joint position.
		/// </summary>
		public Point JointPosition
		{
			get { return _jointPosition; }
			private set
			{
				if (_jointPosition == value) return;
				_jointPosition = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the arm's end position.
		/// </summary>
		public Point EndPosition
		{
			get { return _endPosition; }
			private set
			{
				if (_endPosition == value) return;
				_endPosition = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the first part length.
		/// </summary>
		public double FirstPartLength
		{
			get { return (JointPosition - StartPosition).Length; }
		}

		/// <summary>
		/// Gets the second part length.
		/// </summary>
		public double SecondPartLength
		{
			get { return (EndPosition - JointPosition).Length; }
		}

		/// <summary>
		/// Gets the minimum range of arms.
		/// </summary>
		public double MinRange
		{
			get { return 2 * Math.Abs(SecondPartLength - FirstPartLength); }
		}

		/// <summary>
		/// Gets the maximum range of arms.
		/// </summary>
		public double MaxRange
		{
			get { return (FirstPartLength + SecondPartLength) * 2; }
		}

		public Arm(Point startPosition)
		{
			_startPosition = startPosition;
			JointPosition = startPosition;
			EndPosition = startPosition;
		}

		/// <summary>
		/// Sets the joint position.
		/// </summary>
		/// <param name="position">New joint position.</param>
		public void SetJointPosition(Point position)
		{
			JointPosition = position;
		}

		/// <summary>
		/// Sets the end arm position.
		/// </summary>
		/// <param name="position">New end position.</param>
		public void SetEndPosition(Point position)
		{
			EndPosition = position;
			NotifyArmsChanged();
		}

		private void NotifyArmsChanged()
		{
			OnPropertyChanged(nameof(MaxRange));
			OnPropertyChanged(nameof(MinRange));
		}
	}
}