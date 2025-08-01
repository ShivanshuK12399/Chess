using System;
using UnityEngine;

namespace Chess.Scripts.Core 
{
    public class ChessPlayerPlacementHandler : MonoBehaviour 
    {

        [SerializeField] private ChessPieceType piece;
        [SerializeField] private int row, column;

        public Vector2Int currentTileIndex;
        public bool isWhite;

        public enum ChessPieceType 
        {
            None, Pawn, Knight, Bishop, Rook, Queen, King
        }


        void Start()
        {
            ChangePosition();
        }

        // This function is used to change the position of the piece while testing in the editor.
        // By "ContextMenu" we can call this function by right cliking on header
        [ContextMenu("Change Position")]
        public void ChangePosition()
        {
            Transform tileTransform = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform;
            transform.SetParent(tileTransform);
            transform.localPosition = Vector3.zero; // To align with tile's origin

            currentTileIndex = new Vector2Int(row, column);
        }

        public void OnPieceClicked()
        {
            PieceMovementHandler.Instance.GetValidMoves(piece, currentTileIndex, this);
        }

        public bool TryHighlight(int row, int col)
        {
            var tile = ChessBoardPlacementHandler.Instance.GetTile(row, col)?.transform;

            if (tile == null) return false;

            if (tile.childCount > 0)
            {
                var otherPiece = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                if (otherPiece != null && otherPiece.isWhite != this.isWhite)
                {
                    otherPiece.GetComponent<SpriteRenderer>().color = Color.red;
                    ChessBoardPlacementHandler.Instance.enemyHighlights.Add(otherPiece.transform);
                }
                return false; // Blocked by another piece
            }

            ChessBoardPlacementHandler.Instance.Highlight(row, col);
            return true;
        }


    }
}