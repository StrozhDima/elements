using Elements.Common;
using Elements.Level;
using NUnit.Framework;
using UnityEngine;

namespace Elements.Tests
{
    [TestFixture]
    public sealed class LevelModelTests
    {
        private ILevelModel _level;
        private IMoveValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _level = new LevelModel();
            _validator = new MoveValidator(_level);
        }

        [Test]
        public void TryMove_WithEmptyTarget_MovesBlock()
        {
            // 2x1: Fire at (0,0), empty at (1,0)
            SetupBoard(2, 1, new BlockType?[] { BlockType.Fire, null });

            _level.TryMove(0, 0, Vector2Int.right);

            Assert.IsNull(_level.GetBlockType(0, 0));
            Assert.AreEqual(BlockType.Fire, _level.GetBlockType(1, 0));
        }

        [Test]
        public void TryMove_WithOccupiedTarget_SwapsBlocks()
        {
            // 2x1: Fire at (0,0), Water at (1,0)
            SetupBoard(2, 1, new BlockType?[] { BlockType.Fire, BlockType.Water });

            _level.TryMove(0, 0, Vector2Int.right);

            Assert.AreEqual(BlockType.Water, _level.GetBlockType(0, 0));
            Assert.AreEqual(BlockType.Fire, _level.GetBlockType(1, 0));
        }

        [Test]
        public void TryMove_UpDirection_WithEmptyAbove_ReturnsFalse()
        {
            // 1x2: Water at (0,0), empty at (0,1)
            SetupBoard(1, 2, new BlockType?[] { BlockType.Water, null });

            var valid = _validator.IsValidMove(0, 0, Vector2Int.up);

            Assert.IsFalse(valid);
        }

        [Test]
        public void TryMove_UpDirection_WithOccupiedAbove_ReturnsTrue()
        {
            // 1x2: Water at (0,0), Fire at (0,1)
            SetupBoard(1, 2, new BlockType?[] { BlockType.Water, BlockType.Fire });

            var valid = _validator.IsValidMove(0, 0, Vector2Int.up);

            Assert.IsTrue(valid);
        }

        [Test]
        public void TryMove_FallingBlock_ReturnsFalse()
        {
            SetupBoard(2, 1, new BlockType?[] { BlockType.Fire, BlockType.Water });
            _level.SetState(0, 0, BlockState.Falling);

            var valid = _validator.IsValidMove(0, 0, Vector2Int.right);

            Assert.IsFalse(valid);
        }

        [Test]
        public void TryMove_DestroyingBlock_ReturnsFalse()
        {
            SetupBoard(2, 1, new BlockType?[] { BlockType.Fire, BlockType.Water });
            _level.SetState(0, 0, BlockState.Destroying);

            var valid = _validator.IsValidMove(0, 0, Vector2Int.right);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsEmpty_AfterClearingAllCells_ReturnsTrue()
        {
            SetupBoard(2, 2, new BlockType?[] { BlockType.Fire, BlockType.Water, BlockType.Water, BlockType.Fire });

            for (var col = 0; col < _level.Width; col++)
            {
                for (var row = 0; row < _level.Height; row++)
                {
                    _level.SetCell(col, row, null);
                }
            }

            Assert.IsTrue(_level.IsEmpty());
        }

        // cells[col + row * width] = value at (col, row), row 0 = bottom
        private void SetupBoard(int width, int height, BlockType?[] cells)
        {
            var cellInts = new int[width * height];

            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    var type = cells[row * width + col];
                    cellInts[row * width + col] = type.HasValue ? (int)type.Value : -1;
                }
            }

            _level.LoadFromSave(new GameSaveData
            {
                Width = width,
                Height = height,
                Cells = cellInts,
                LevelIndex = 0
            });
        }
    }
}