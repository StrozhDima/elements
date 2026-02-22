using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    public sealed class NormalizationSystem : INormalizationSystem
    {
        private readonly INormalizationView _view;

        public NormalizationSystem(INormalizationView view) => _view = view;

        async UniTask INormalizationSystem.NormalizeAsync(ILevelModel level, CancellationToken cancellationToken)
        {
            while (true)
            {
                var falls = ApplyGravity(level);

                if (falls.Count > 0)
                {
                    foreach (var fall in falls)
                    {
                        for (var row = fall.to.y; row <= fall.from.y; row++)
                        {
                            level.SetState(fall.to.x, row, BlockState.Falling);
                        }
                    }

                    var fallTasks = new UniTask[falls.Count];

                    for (var i = 0; i < falls.Count; i++)
                    {
                        fallTasks[i] = _view.PlayFallAsync(falls[i].from, falls[i].to, cancellationToken);
                    }

                    await UniTask.WhenAll(fallTasks);

                    foreach (var fall in falls)
                    {
                        for (var row = fall.to.y; row <= fall.from.y; row++)
                        {
                            level.SetState(fall.to.x, row, BlockState.Idle);
                        }
                    }
                }

                var destroyCells = FindDestroyableRegions(level);

                if (destroyCells.Count == 0)
                {
                    break;
                }

                foreach (var cell in destroyCells)
                {
                    level.SetState(cell.x, cell.y, BlockState.Destroying);
                }

                await _view.PlayDestroyAsync(destroyCells, cancellationToken);

                foreach (var cell in destroyCells)
                {
                    level.SetCell(cell.x, cell.y, null);
                }
            }
        }

        public List<(Vector2Int from, Vector2Int to)> ApplyGravity(ILevelModel level)
        {
            var falls = new List<(Vector2Int from, Vector2Int to)>();

            for (var col = 0; col < level.Width; col++)
            {
                var writeRow = 0;

                for (var row = 0; row < level.Height; row++)
                {
                    if (!level.GetBlockType(col, row).HasValue)
                    {
                        continue;
                    }

                    if (row != writeRow)
                    {
                        level.SetCell(col, writeRow, level.GetBlockType(col, row));
                        level.SetCell(col, row, null);
                        falls.Add((new Vector2Int(col, row), new Vector2Int(col, writeRow)));
                    }

                    writeRow++;
                }
            }

            return falls;
        }

        public List<Vector2Int> FindDestroyableRegions(ILevelModel level)
        {
            var visited = new bool[level.Width, level.Height];
            var result = new List<Vector2Int>();

            for (var col = 0; col < level.Width; col++)
            {
                for (var row = 0; row < level.Height; row++)
                {
                    if (visited[col, row] || !level.GetBlockType(col, row).HasValue)
                    {
                        continue;
                    }

                    var region = FloodFill(level, col, row, visited);

                    if (HasLineOfThree(region))
                    {
                        result.AddRange(region);
                    }
                }
            }

            return result;
        }

        private List<Vector2Int> FloodFill(ILevelModel level, int startCol, int startRow, bool[,] visited)
        {
            var region = new List<Vector2Int>();
            var type = level.GetBlockType(startCol, startRow);
            var queue = new Queue<Vector2Int>();

            queue.Enqueue(new Vector2Int(startCol, startRow));
            visited[startCol, startRow] = true;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                region.Add(current);
                TryEnqueue(level, queue, visited, type, current.x + 1, current.y);
                TryEnqueue(level, queue, visited, type, current.x - 1, current.y);
                TryEnqueue(level, queue, visited, type, current.x, current.y + 1);
                TryEnqueue(level, queue, visited, type, current.x, current.y - 1);
            }

            return region;
        }

        private void TryEnqueue(ILevelModel level, Queue<Vector2Int> queue, bool[,] visited, BlockType? type, int col, int row)
        {
            if (col < 0 || col >= level.Width || row < 0 || row >= level.Height)
            {
                return;
            }
            if (visited[col, row])
            {
                return;
            }
            if (level.GetBlockType(col, row) != type)
            {
                return;
            }

            visited[col, row] = true;
            queue.Enqueue(new Vector2Int(col, row));
        }

        private bool HasLineOfThree(List<Vector2Int> region)
        {
            var cellSet = new HashSet<Vector2Int>(region);

            foreach (var cell in region)
            {
                var hCount = 1;
                var x = cell.x + 1;

                while (cellSet.Contains(new Vector2Int(x, cell.y)))
                {
                    hCount++;
                    x++;
                }

                if (hCount >= 3)
                {
                    return true;
                }

                var vCount = 1;
                var y = cell.y + 1;

                while (cellSet.Contains(new Vector2Int(cell.x, y)))
                {
                    vCount++;
                    y++;
                }

                if (vCount >= 3)
                {
                    return true;
                }
            }

            return false;
        }
    }
}