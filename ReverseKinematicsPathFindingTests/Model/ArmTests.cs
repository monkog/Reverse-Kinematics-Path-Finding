using System.Windows;
using ReverseKinematicsPathFinding.Model;
using Xunit;

namespace ReverseKinematicsPathFindingTests.Model
{
	public class ArmTests
	{
		private readonly Arm _unitUnderTest;
		private readonly Point _startPosition;

		public ArmTests()
		{
			_startPosition = new Point(10, 5);
			_unitUnderTest = new Arm(_startPosition);
		}

		[Fact]
		public void Constructor_StartPosition_PropertiesSet()
		{
			var unitUnderTest = new Arm(_startPosition);

			Assert.Equal(_startPosition, unitUnderTest.Start);
			Assert.Equal(_startPosition, unitUnderTest.Joint);
			Assert.Equal(_startPosition, unitUnderTest.AlternativeJoint);
			Assert.Equal(_startPosition, unitUnderTest.End);
		}

		[Fact]
		public void SetJointPosition_Position_JointPositionSet()
		{
			var jointPosition = new Point(4, 20);

			_unitUnderTest.SetJointPosition(jointPosition);

			Assert.Equal(jointPosition, _unitUnderTest.Joint);
		}

		[Fact]
		public void SetEndPosition_Position_EndPositionSet()
		{
			var endPosition = new Point(4, 20);

			_unitUnderTest.SetEndPosition(endPosition);

			Assert.Equal(endPosition, _unitUnderTest.End);
		}

		[Fact]
		public void FirstPartLength_PositionsSet_FirstPartLength()
		{
			var jointPosition = new Point(11, 5);
			_unitUnderTest.SetJointPosition(jointPosition);

			Assert.Equal(1, _unitUnderTest.FirstPartLength);
		}

		[Fact]
		public void SecondPartLength_PositionsSet_SecondPartLength()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetJointPosition(jointPosition);
			_unitUnderTest.SetEndPosition(endPosition);

			Assert.Equal(3, _unitUnderTest.SecondPartLength);
		}

		[Fact]
		public void MinRange_PositionsSet_MinRange()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetJointPosition(jointPosition);
			_unitUnderTest.SetEndPosition(endPosition);

			Assert.Equal(4, _unitUnderTest.MinRange);
		}

		[Fact]
		public void MaxRange_PositionsSet_MaxRange()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetJointPosition(jointPosition);
			_unitUnderTest.SetEndPosition(endPosition);

			Assert.Equal(8, _unitUnderTest.MaxRange);
		}
	}
}