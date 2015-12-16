using System;

namespace ReverseKinematicsPathFinding.Model
{
    using System.Windows;

    using ViewModel;

    public class Robot : ViewModelBase
    {
        #region Private Members

        private Point _firstPosition;
        private Point _secondPosition;
        private Point _zeroPosition;

        private double _l1;
        private double _l2;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Position of the beginning of the robot's first arm.
        /// </summary>
        public Point ZeroPosition
        {
            get { return _zeroPosition; }
            set
            {
                if (_zeroPosition == value) return;
                _zeroPosition = value;
                OnPropertyChanged("ZeroPosition");
                RecalculateRobot();
            }
        }

        /// <summary>
        /// Position of the robot's first arm.
        /// </summary>
        public Point FirstPosition
        {
            get { return _firstPosition; }
            set
            {
                if (_firstPosition == value) return;
                _firstPosition = value;
                OnPropertyChanged("FirstPosition");
                RecalculateRobot();
            }
        }

        /// <summary>
        /// Position of the robot's second arm.
        /// </summary>
        public Point SecondPosition
        {
            get { return _secondPosition; }
            set
            {
                if (_secondPosition == value) return;
                _secondPosition = value;
                OnPropertyChanged("SecondPosition");
            }
        }

        /// <summary>
        /// Length of the robot's first arm.
        /// </summary>
        public double L1
        {
            get { return _l1; }
            set
            {
                if (_l1 == value) return;
                _l1 = value;
                OnPropertyChanged("L1");
            }
        }

        /// <summary>
        /// Length of the robot's second arm.
        /// </summary>
        public double L2
        {
            get { return _l2; }
            set
            {
                if (_l2 == value) return;
                _l2 = value;
                OnPropertyChanged("L2");
            }
        }

        #endregion Public Members

        #region Constructors

        public Robot()
        {
            SecondPosition = FirstPosition = ZeroPosition = new Point(1300 / 2.0, 837 / 2.0);
            L1 = L2 = double.NaN;
        }

        #endregion Constructors

        private void RecalculateRobot()
        {
            L1 = (FirstPosition - ZeroPosition).Length;
            if (!double.IsNaN(L2))
                L2 = (SecondPosition - FirstPosition).Length;
        }
    }
}
