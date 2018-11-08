using System.Windows;
using ReverseKinematicsPathFinding.ViewModel;

namespace ReverseKinematicsPathFinding
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void AreaSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (DataContext != null) return;
			DataContext = new MainViewModel(e.NewSize.Width, e.NewSize.Height);
		}
	}
}