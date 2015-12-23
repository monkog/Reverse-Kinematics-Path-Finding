namespace ReverseKinematicsPathFinding.Model
{
    using System.Windows;

    using ReverseKinematicsPathFinding.ViewModel;

    public class Obstacle : ViewModelBase
    {
        #region Private Members

        private Point _position;
        private Point _size;

        private bool _isSelected;

        #endregion Private Members

        #region Public Properties

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

        /// <summary>
        /// Is the obstacle selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        #endregion Public Properties

        #region Constructors

        public Obstacle()
        {
            Position = new Point();
            Size = new Point();
            IsSelected = false;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Checks if the given point is inside the obstacle.
        /// </summary>
        /// <param name="p">Point to check.</param>
        /// <returns>True if the obstacle contains the given point, otherwise false.</returns>
        public bool Contains(Point p)
        {
            return Position.X <= p.X && Position.X + Size.X <= p.X &&
                Position.Y <= p.Y && Position.Y + Size.Y <= p.Y;
        }

        #endregion Public Methods
    }
}