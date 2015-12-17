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
        private Point _destinationPosition;

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
                RecalculateRobot();
            }
        }

        /// <summary>
        /// Destination position of the robot's second arm.
        /// </summary>
        public Point DestinationPosition
        {
            get { return _destinationPosition; }
            set
            {
                if (_destinationPosition == value) return;
                _destinationPosition = value;
                OnPropertyChanged("DestinationPosition");
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
            SecondPosition = FirstPosition = ZeroPosition = new Point(1200 / 2.0, 837 / 2.0);
            L1 = L2 = double.NaN;
        }

        #endregion Constructors

        #region Private Methods

        private void RecalculateRobot()
        {
            if (!double.IsNaN(L1))
                L2 = (SecondPosition - FirstPosition).Length;
            L1 = (FirstPosition - ZeroPosition).Length;
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            double q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            double d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            double r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            double s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        #endregion Private Methods

        #region Public Methods

        public bool IntersectsRectangle(Point p1, Point p2, Obstacle r)
        {
            var l1=LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y));
            var l2=LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ;
            var l3=LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y), new Point(r.Position.X, r.Position.Y + r.Size.Y));
            var l4=LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y + r.Size.Y), new Point(r.Position.X, r.Position.Y)) ;
            var l5 =r.Contains(p1);
            var l6 = r.Contains(p2);

            return LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y), new Point(r.Position.X, r.Position.Y + r.Size.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y + r.Size.Y), new Point(r.Position.X, r.Position.Y)) ||
                   (r.Contains(p1) || r.Contains(p2));
        }

        public Point CalculateFirstPosition(double alpha, double beta)
        {
            return new Point(L1 * Math.Cos(alpha + beta), L1 * Math.Sin(alpha + beta));
        }

        public Point CalculateSecondPosition(double alpha, double beta)
        {
            return new Point(L1 * Math.Cos(alpha) + L2 * Math.Cos(alpha + beta), L1 * Math.Sin(alpha) + L2 * Math.Sin(alpha + beta));
        }

        #endregion Public Methods
    }
}
