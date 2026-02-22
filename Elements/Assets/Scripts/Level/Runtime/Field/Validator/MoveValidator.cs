using UnityEngine;

namespace Elements.Level
{
    public sealed class MoveValidator : IMoveValidator
    {
        private readonly ILevelModel _levelModel;

        public MoveValidator(ILevelModel levelModel) => _levelModel = levelModel;

        bool IMoveValidator.IsValidMove(int col, int row, Vector2Int direction)
        {
            if (!_levelModel.GetBlockType(col, row).HasValue)
            {
                return false;
            }

            var state = _levelModel.GetBlockState(col, row);

            if (state is not BlockState.Idle)
            {
                return false;
            }

            var targetCol = col + direction.x;
            var targetRow = row + direction.y;

            if (targetCol < 0 || targetCol >= _levelModel.Width)
            {
                return false;
            }

            if (targetRow < 0 || targetRow >= _levelModel.Height)
            {
                return false;
            }

            if (direction == Vector2Int.up && !_levelModel.GetBlockType(targetCol, targetRow).HasValue)
            {
                return false;
            }

            var blockState = _levelModel.GetBlockState(targetCol, targetRow);
            return blockState is BlockState.Idle;
        }
    }
}