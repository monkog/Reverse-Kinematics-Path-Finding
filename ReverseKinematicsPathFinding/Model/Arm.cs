using System;
using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding.Model
{
	public class Arm:ViewModelBase
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
		public double FirstPartLength
		{
			get { return (Joint - Start).Length; }
		}

		/// <summary>
		/// Gets the second part length.
		/// </summary>
		public double SecondPartLength
		{
			get { return (End - Joint).Length; }
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

		public Arm(Point start)
		{
			_start = start;
			Joint = start;
			AlternativeJoint = start;
			End = start;
		}

		/// <summary>
		/// Sets the joint position.
		/// </summary>
		/// <param name="position">New joint position.</param>
		public void SetJointPosition(Point position)
		{
			Joint = position;
		}

		/// <summary>
		/// Sets the alternative joint position.
		/// </summary>
		/// <param name="position">New joint position.</param>
		public void SetAlternativeJointPosition(Point position)
		{
			AlternativeJoint = position;
		}

		/// <summary>
		/// Sets the end arm position.
		/// </summary>
		/// <param name="position">New end position.</param>
		public void SetEndPosition(Point position)
		{
			End = position;
			NotifyArmsChanged();
		}

		private void NotifyArmsChanged()
		{
			OnPropertyChanged(nameof(MaxRange));
			OnPropertyChanged(nameof(MinRange));
		}
	}
}