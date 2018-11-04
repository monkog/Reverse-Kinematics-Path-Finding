using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding.Model
{
    public class Obstacle : ViewModelBase
    {
        private Point _position;

        private Point _size;

        private bool _isSelected;
		
        /// <summary>
        /// Position of the obstacle.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            private set
            {
                if (_position == value) return;
                _position = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Size of the obstacle.
        /// </summary>
        public Point Size
        {
            get { return _size; }
            private set
            {
                if (_size == value) return;
                _size = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
		
        public Obstacle(Point position, Point size)
        {
            Position = position;
            Size = size;
            IsSelected = false;
        }

        /// <summary>
        /// Checks if the given point is inside the obstacle.
        /// </summary>
        /// <param name="p">Point to check.</param>
        /// <returns>True if the obstacle contains the given point, otherwise false.</returns>
        public bool Contains(Point p)
        {
            return Position.X <= p.X && Position.X + Size.X >= p.X &&
                Position.Y <= p.Y && Position.Y + Size.Y >= p.Y;
        }

		/// <summary>
		/// Moves the obstacle by the given delta.
		/// </summary>
		/// <param name="delta"></param>
	    public void Move(Vector delta)
	    {
		    Position = Position + delta;
	    }

		/// <summary>
		/// Changes the size of the obstacle.
		/// </summary>
		/// <param name="width">Width of the obstacle.</param>
		/// <param name="height">Height of the obstacle.</param>
		public void Resize(double width, double height)
	    {
		    Size = new Point(width, height);
	    }
    }
}