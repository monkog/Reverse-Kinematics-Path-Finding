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
        private Point _thirdPosition;
        private Point _fourthPosition;

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
            }
        }

        /// <summary>
        /// First position of the robot's first arm.
        /// </summary>
        public Point FirstPosition
        {
            get { return _firstPosition; }
            set
            {
                if (_firstPosition == value) return;
                _firstPosition = value;
                OnPropertyChanged("FirstPosition");
            }
        }

        /// <summary>
        /// Second position of the robot's first arm.
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
        /// First position of the robot's second arm.
        /// </summary>
        public Point ThirdPosition
        {
            get { return _thirdPosition; }
            set
            {
                if (_thirdPosition == value) return;
                _thirdPosition = value;
                OnPropertyChanged("ThirdPosition");
            }
        }

        /// <summary>
        /// Second position of the robot's second arm.
        /// </summary>
        public Point FourthPosition
        {
            get { return _fourthPosition; }
            set
            {
                if (_fourthPosition == value) return;
                _fourthPosition = value;
                OnPropertyChanged("FourthPosition");
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
                OnPropertyChanged("ArmsMaxRange");
                OnPropertyChanged("ArmsMinRange");
            }
        }

        /// <summary>
        /// Gets the maximum range of arms.
        /// </summary>
        public double ArmsMaxRange
        {
            get { return (L1 + L2) * 2; }
        }

        /// <summary>
        /// Gets the minimum range of arms.
        /// </summary>
        public double ArmsMinRange
        {
            get { return 2 * Math.Abs(L2 - L1); }
        }

        #endregion Public Members

        #region Constructors

        public Robot(double width, double height)
        {
            FourthPosition = ThirdPosition = SecondPosition = FirstPosition = ZeroPosition = new Point(width / 2.0, height / 2.0);
            DestinationPosition = ZeroPosition;
            L1 = L2 = double.NaN;
        }

        #endregion Constructors

        #region Private Methods

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

        public void RecalculateRobot()
        {
            if (!double.IsNaN(L1))
                L2 = (SecondPosition - FirstPosition).Length;
            L1 = (FirstPosition - ZeroPosition).Length;

            if (!double.IsNaN(L2))
            {
                var delta = SecondPosition - ZeroPosition;
                var angles = CalculateReverseKinematicsSecondPosition(delta.X, delta.Y);

                ThirdPosition = CalculateFirstPosition(angles.X);
                FourthPosition = CalculateSecondPosition(ThirdPosition, angles.X, angles.Y);
            }
        }

        public bool IntersectsRectangle(Point p1, Point p2, Obstacle r)
        {
            var a = LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y));
            var b = LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y));
            var c = LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y + r.Size.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y));
            var d = LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X, r.Position.Y + r.Size.Y));
            var e = r.Contains(p1);
            var f = r.Contains(p2);

            return LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X + r.Size.X, r.Position.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y + r.Size.Y), new Point(r.Position.X + r.Size.X, r.Position.Y + r.Size.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.Position.X, r.Position.Y), new Point(r.Position.X, r.Position.Y + r.Size.Y)) ||
                   (r.Contains(p1) || r.Contains(p2));
        }

        public Point CalculateFirstPosition(double alpha)
        {
            return new Point(ZeroPosition.X + (L1 * Math.Cos(alpha)), ZeroPosition.Y + (L1 * Math.Sin(alpha)));
        }

        public Point CalculateSecondPosition(Point firstPosition, double alpha, double beta)
        {
            return new Point(firstPosition.X + (L2 * (((Math.Cos(beta) * Math.Cos(alpha))) + (Math.Sin(beta) * Math.Sin(alpha)))),
                firstPosition.Y + (L2 * (-(Math.Sin(beta) * Math.Cos(alpha)) + (Math.Cos(beta) * Math.Sin(alpha)))));
        }

        public Point CalculateReverseKinematicsFirstPosition(double x, double y)
        {
            var beta = -Math.Acos((x * x + y * y - L1 * L1 - L2 * L2) / (2 * L1 * L2));
            var alpha = Math.Asin((L2 * Math.Sin(beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
            return new Point(alpha, beta);
        }

        public Point CalculateReverseKinematicsSecondPosition(double x, double y)
        {
            var beta = Math.Acos((x * x + y * y - L1 * L1 - L2 * L2) / (2 * L1 * L2));
            var alpha = -Math.Asin((L2 * Math.Sin(-beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
            return new Point(alpha, beta);
        }

        #endregion Public Methods
    }
}
