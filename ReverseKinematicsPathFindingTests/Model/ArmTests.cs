using System;
using System.Windows;
using ReverseKinematicsPathFinding.Model;
using Xunit;

namespace ReverseKinematicsPathFindingTests.Model
{
	public class ArmTests
	{
		private const double Epsilon = 0.00001;
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
		public void SetArmPosition_Position_JointPositionSet()
		{
			var jointPosition = new Point(4, 20);

			_unitUnderTest.SetArmPosition(jointPosition);

			Assert.Equal(jointPosition, _unitUnderTest.Joint);
		}

		[Fact]
		public void SetArmPosition_Position_EndPositionSet()
		{
			var endPosition = new Point(4, 20);

			_unitUnderTest.SetArmPosition(endPosition);

			Assert.Equal(endPosition, _unitUnderTest.End);
		}

		[Fact]
		public void SetArmPosition_TwoPositions_AlternativeJointPositionSet()
		{
			var jointPosition = new Point(4, 10);
			var endPosition = new Point(4, 20);

			_unitUnderTest.SetArmPosition(jointPosition);
			_unitUnderTest.SetArmPosition(endPosition);

			Assert.NotEqual(_unitUnderTest.Start, _unitUnderTest.AlternativeJoint);
		}

		[Fact]
		public void SetArmPosition_TwoPositions_JointsCalculated()
		{
			var jointPosition = new Point(10, 0);
			var endPosition = new Point(15, 0);

			_unitUnderTest.SetArmPosition(jointPosition);
			_unitUnderTest.SetArmPosition(endPosition);

			Assert.True(Math.Abs(_unitUnderTest.AlternativeJoint.X - 10) < Epsilon);
			Assert.True(Math.Abs(_unitUnderTest.AlternativeJoint.Y) < Epsilon);
			Assert.True(Math.Abs(_unitUnderTest.Joint.X - 15) < Epsilon);
			Assert.True(Math.Abs(_unitUnderTest.Joint.Y - 5) < Epsilon);
		}

		[Fact]
		public void FirstPartLength_PositionsSet_FirstPartLength()
		{
			var jointPosition = new Point(11, 5);
			_unitUnderTest.SetArmPosition(jointPosition);

			Assert.Equal(1, _unitUnderTest.FirstPartLength);
		}

		[Fact]
		public void SecondPartLength_PositionsSet_SecondPartLength()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetArmPosition(jointPosition);
			_unitUnderTest.SetArmPosition(endPosition);

			Assert.Equal(3, _unitUnderTest.SecondPartLength);
		}

		[Fact]
		public void MinRange_PositionsSet_MinRange()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetArmPosition(jointPosition);
			_unitUnderTest.SetArmPosition(endPosition);

			Assert.Equal(4, _unitUnderTest.MinRange);
		}

		[Fact]
		public void MaxRange_PositionsSet_MaxRange()
		{
			var jointPosition = new Point(11, 5);
			var endPosition = new Point(14, 5);
			_unitUnderTest.SetArmPosition(jointPosition);
			_unitUnderTest.SetArmPosition(endPosition);

			Assert.Equal(8, _unitUnderTest.MaxRange);
		}
	}
}