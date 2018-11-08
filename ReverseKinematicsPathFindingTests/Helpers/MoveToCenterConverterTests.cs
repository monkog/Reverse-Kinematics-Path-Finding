using ReverseKinematicsPathFinding.Helpers;
using Xunit;

namespace ReverseKinematicsPathFindingTests.Helpers
{
	public class MoveToCenterConverterTests
	{
		private readonly MoveToCenterConverter _unitUnderTest;

		public MoveToCenterConverterTests()
		{
			_unitUnderTest = new MoveToCenterConverter();
		}

		[Theory]
		[InlineData(10, 100, 45)]
		[InlineData(2, 11, 4.5)]
		public void MoveToCenter_WidthAndOffset_StartPosition(double objectSize, double screenSize, double expected)
		{
			var result = _unitUnderTest.Convert(new object[] { objectSize, screenSize }, null, null, null);

			Assert.Equal(expected, result);
		}
	}
}