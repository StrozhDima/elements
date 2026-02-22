using JetBrains.Annotations;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class MoveValidator : IMoveValidator
    {
        bool IMoveValidator.IsValidMove(ILevelModel level, int col, int row, Vector2Int direction)
        {
            if (!level.GetBlockType(col, row).HasValue)
            {
                return false;
            }

            var state = level.GetBlockState(col, row);

            if (state is not BlockState.Idle)
            {
                return false;
            }

            var targetCol = col + direction.x;
            var targetRow = row + direction.y;

            if (targetCol < 0 || targetCol >= level.Width)
            {
                return false;
            }

            if (targetRow < 0 || targetRow >= level.Height)
            {
                return false;
            }

            if (direction == Vector2Int.up && !level.GetBlockType(targetCol, targetRow).HasValue)
            {
                return false;
            }

            if (level.GetBlockState(targetCol, targetRow) is not BlockState.Idle)
            {
                return false;
            }

            return true;
        }
    }
}