using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elements.Common;
using Elements.Level;
using NUnit.Framework;
using UnityEngine;

namespace Elements.Tests
{
    [TestFixture]
    public sealed class NormalizationSystemTests
    {
        private INormalizationSystem _system;
        private ILevelModel _level;

        [SetUp]
        public void SetUp()
        {
            _system = new NormalizationSystem(new NullNormalizationView());
            _level = new LevelModel();
        }

        [Test]
        public void FindDestroyableRegions_WithVerticalLine3_ReturnsRegion()
        {
            // 1 column, 3 rows, all Water (vertical line of 3)
            SetupBoard(1, 3, new BlockType?[]
            {
                BlockType.Water,
                BlockType.Water,
                BlockType.Water
            });

            var regions = _system.FindDestroyableRegions(_level);

            Assert.AreEqual(3, regions.Count);
        }

        [Test]
        public void FindDestroyableRegions_WithHorizontalLine3_ReturnsRegion()
        {
            // 3 columns, 1 row, all Fire (horizontal line of 3)
            SetupBoard(3, 1, new BlockType?[]
            {
                BlockType.Fire, BlockType.Fire, BlockType.Fire
            });

            var regions = _system.FindDestroyableRegions(_level);

            Assert.AreEqual(3, regions.Count);
        }

        [Test]
        public void FindDestroyableRegions_WithCrossShape_ReturnsEntireRegion()
        {
            // 3x3, cross of Water: center, top, bottom, left, right
            // Layout (row 0=bottom):
            // row2: _, W, _
            // row1: W, W, W
            // row0: _, W, _
            SetupBoard(3, 3, new BlockType?[]
            {
                null, BlockType.Water, null, // row 0
                BlockType.Water, BlockType.Water, BlockType.Water, // row 1
                null, BlockType.Water, null // row 2
            });

            var regions = _system.FindDestroyableRegions(_level);

            Assert.AreEqual(5, regions.Count);
        }

        [Test]
        public void FindDestroyableRegions_WithTShape_ReturnsEntireRegion()
        {
            // T-shape of Fire:
            // row2: F, F, F
            // row1: _, F, _
            // row0: _, F, _
            SetupBoard(3, 3, new BlockType?[]
            {
                null, BlockType.Fire, null, // row 0
                null, BlockType.Fire, null, // row 1
                BlockType.Fire, BlockType.Fire, BlockType.Fire // row 2
            });

            var regions = _system.FindDestroyableRegions(_level);

            Assert.AreEqual(5, regions.Count);
        }

        [Test]
        public void FindDestroyableRegions_WithIsolatedPairs_ReturnsEmpty()
        {
            // 2x2, all Water — no line of 3
            SetupBoard(2, 2, new BlockType?[]
            {
                BlockType.Water, BlockType.Water,
                BlockType.Water, BlockType.Water
            });

            var regions = _system.FindDestroyableRegions(_level);

            Assert.AreEqual(0, regions.Count);
        }

        [Test]
        public void ApplyGravity_WithHangingBlocks_FallsDown()
        {
            // 1 column, 3 rows: empty at row0, Water at row1 and row2
            SetupBoard(1, 3, new BlockType?[]
            {
                null, // row 0
                BlockType.Water, // row 1
                BlockType.Water // row 2
            });

            var falls = _system.ApplyGravity(_level);

            // Water at row1 falls to row0; Water at row2 falls to row1
            Assert.AreEqual(2, falls.Count);
            Assert.AreEqual(BlockType.Water, _level.GetBlockType(0, 0));
            Assert.AreEqual(BlockType.Water, _level.GetBlockType(0, 1));
            Assert.IsNull(_level.GetBlockType(0, 2));
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

        private sealed class NullNormalizationView : INormalizationView
        {
            public UniTask PlayDestroyAsync(IReadOnlyList<Vector2Int> cells, CancellationToken cancellationToken)
                => UniTask.CompletedTask;

            public UniTask PlayFallAsync(Vector2Int from, Vector2Int to, CancellationToken cancellationToken)
                => UniTask.CompletedTask;
        }
    }
}