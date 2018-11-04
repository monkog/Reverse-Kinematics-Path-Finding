using System.Windows;
using ReverseKinematicsPathFinding.Model;
using Xunit;

namespace ReverseKinematicsPathFindingTests.Model
{
	public class ObstacleTests
	{
		private readonly Point _position;

		private readonly Point _size;

		private readonly Obstacle _unitUnderTest;

		public ObstacleTests()
		{
			_position = new Point(-5, 2);
			_size = new Point(10, 12);
			_unitUnderTest = new Obstacle(_position, _size);
		}

		[Fact]
		public void Constructor_NoParams_PropertiesInitialized()
		{
			var unitUnderTest = new Obstacle(_position, _size);

			Assert.False(unitUnderTest.IsSelected);
			Assert.Equal(_position.X, unitUnderTest.Position.X);
			Assert.Equal(_position.Y, unitUnderTest.Position.Y);
			Assert.Equal(_size.X, unitUnderTest.Size.X);
			Assert.Equal(_size.Y, unitUnderTest.Size.Y);
		}

		[Theory]
		[InlineData(-5, 2)]
		[InlineData(0, 5)]
		[InlineData(5, 14)]
		public void Contains_PointInBounds_True(double x, double y)
		{
			var point = new Point(x, y);

			Assert.True(_unitUnderTest.Contains(point));
		}

		[Fact]
		public void Contains_PointOutsideBounds_False()
		{
			var point = new Point(11, 10);

			Assert.False(_unitUnderTest.Contains(point));
		}

		[Fact]
		public void Move_Delta_ObstacleMoved()
		{
			var delta = new Vector(2, 3);

			_unitUnderTest.Move(delta);

			Assert.Equal(_position.X + delta.X, _unitUnderTest.Position.X);
			Assert.Equal(_position.Y + delta.Y, _unitUnderTest.Position.Y);
		}

		[Fact]
		public void Resize_NewSize_SizeSet()
		{
			_unitUnderTest.Resize(4, 6);

			Assert.Equal(4, _unitUnderTest.Size.X);
			Assert.Equal(6, _unitUnderTest.Size.Y);
		}
	}
}