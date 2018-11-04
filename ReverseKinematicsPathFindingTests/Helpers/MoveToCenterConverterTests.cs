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
		[InlineData(10, 0, -5)]
		[InlineData(10, 2, -3)]
		[InlineData(11, 0, -5.5)]
		public void MoveToCenter_WidthAndOffset_StartPosition(double width, double offset, double expected)
		{
			var result = _unitUnderTest.Convert(width, null, offset, null);

			Assert.Equal(expected, result);
		}
	}
}