using System.Globalization;
using ReverseKinematicsPathFinding.Helpers;
using Xunit;

namespace ReverseKinematicsPathFindingTests.Helpers
{
	public class AdditionConverterTests
	{
		private readonly AdditionConverter _unitUnderTest = new AdditionConverter();

		[Theory]
		[InlineData(6, 1, 2, 3)]
		[InlineData(0, 5, -5)]
		[InlineData(6.5, 1.1, 2.2, 3.2)]
		public void Convert_Doubles_Sum(double sum, params object[] values)
		{
			var result = _unitUnderTest.Convert(values, null, null, CultureInfo.InvariantCulture);

			Assert.Equal(sum, result);
		}
	}
}