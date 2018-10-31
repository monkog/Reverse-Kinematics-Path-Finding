using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReverseKinematicsPathFinding.ViewModel
{
	/// <summary>
	/// Base class for View Models
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Called when [property changed].
		/// </summary>
		/// <param name="property">The property.</param>
		protected void OnPropertyChanged([CallerMemberName] string property = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}