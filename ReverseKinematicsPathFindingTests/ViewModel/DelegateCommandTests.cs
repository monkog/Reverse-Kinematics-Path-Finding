using ReverseKinematicsPathFinding.ViewModel;
using Xunit;

namespace ReverseKinematicsPathFindingTests.ViewModel
{
	public class DelegateCommandTests
	{
		[Fact]
		public void Constructor_ActionOnly_CanExecuteAlwaysTrue()
		{
			var unitUnderTest = new DelegateCommand(_ => { });

			Assert.True(unitUnderTest.CanExecute());
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Constructor_ActionAndPredicate_CanExecuteIfPredicateMet(bool canExecute)
		{
			var unitUnderTest = new DelegateCommand(_ => { }, can => canExecute);

			Assert.Equal(canExecute, unitUnderTest.CanExecute());
		}

		[Fact]
		public void Execute_Always_Executed()
		{
			var value = "ʕ•ᴥ•ʔ";
			var result = string.Empty;
			var unitUnderTest = new DelegateCommand(_ => result = value);

			unitUnderTest.Execute();

			Assert.Equal(value, result);
		}
	}
}