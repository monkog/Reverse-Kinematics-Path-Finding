namespace ReverseKinematicsPathFinding.Model
{
    using System.Windows;

    using ReverseKinematicsPathFinding.ViewModel;

    public class Obstacle : ViewModelBase
    {
        #region Private Members

        private Point _position;

        private Point _size;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Position of the obstacle.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Size of the obstacle.
        /// </summary>
        public Point Size
        {
            get { return _size; }
            set
            {
                if (_size == value) return;
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        #endregion Public Members

        #region Constructors

        public Obstacle()
        {
            Position = new Point();
            Size = new Point();
        }

        #endregion Constructors
    }
}