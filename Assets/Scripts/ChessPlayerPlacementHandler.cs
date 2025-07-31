using System;
using UnityEngine;

namespace Chess.Scripts.Core 
{
    public class ChessPlayerPlacementHandler : MonoBehaviour 
    {
        [SerializeField] private chessPieceType piece;
        [SerializeField] private int row, column;
        
        public Vector2Int currentTileIndex;
        public bool isWhite;

        enum chessPieceType 
        {
            None, Pawn, Knight, Bishop, Rook, Queen, King
        }

        void Start()
        {
            ChangePosition();
        }

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
            switch (piece)
            {
                case chessPieceType.None:
                    break;

                case chessPieceType.Pawn:
                    PawnMoves();
                    break;

                case chessPieceType.Knight:
                    KnightMoves();
                    break;

                case chessPieceType.Bishop:
                    BishopMoves();
                    break;

                case chessPieceType.Rook:
                    RookMoves();
                    break;

                case chessPieceType.Queen:
                    QueenMoves();
                    break;

                case chessPieceType.King:
                    KingMoves();
                    break;
            }
        }

        #region Moves

        void PawnMoves()
        {
            int row = currentTileIndex.x;
            int col = currentTileIndex.y;

            // Up 
            if (row < 8)
            {
                ChessBoardPlacementHandler.Instance.Highlight(row+1, col);
            }

            // Sideways only if enemy present
            Vector2Int[] pawnOffsets = new Vector2Int[]
            {
                new Vector2Int(1, -1), new Vector2Int(1, 1)
            };

            foreach (var offset in pawnOffsets)
            {
                int r = row + offset.x;
                int c = col + offset.y;

                if (r >= 0 && r < 8 && c >= 0 && c < 8)
                {
                    var tile = ChessBoardPlacementHandler.Instance.GetTile(r, c)?.transform;
                    if (tile != null && tile.childCount > 0)
                    {
                        var otherPiece = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                        if (otherPiece != null && otherPiece.isWhite != this.isWhite)
                        {
                            otherPiece.GetComponent<SpriteRenderer>().color = Color.red;
                            ChessBoardPlacementHandler.Instance.enemyHighlights.Add(otherPiece.transform);
                        }
                    }
                }
            }
        }

        void RookMoves()
        {
            int row = currentTileIndex.x;
            int col = currentTileIndex.y;

            // Up
            for (int r = row + 1; r < 8; r++)
            {
                if (!TryHighlight(r, col)) break;
            }

            // Down
            for (int r = row - 1; r >= 0; r--)
            {
                if (!TryHighlight(r, col)) break;
            }

            // Right
            for (int c = col + 1; c < 8; c++)
            {
                if (!TryHighlight(row, c)) break;
            }

            // Left
            for (int c = col - 1; c >= 0; c--)
            {
                if (!TryHighlight(row, c)) break;
            }
        }

        void BishopMoves()
        {
            int row = currentTileIndex.x;
            int col = currentTileIndex.y;

            // Top-Right
            for (int r = row + 1, c = col + 1; r < 8 && c < 8; r++, c++)
            {
                if (!TryHighlight(r, c)) break;
            }

            // Top-Left
            for (int r = row + 1, c = col - 1; r < 8 && c >= 0; r++, c--)
            {
                if (!TryHighlight(r, c)) break;
            }

            // Bottom-Right
            for (int r = row - 1, c = col + 1; r >= 0 && c < 8; r--, c++)
            {
                if (!TryHighlight(r, c)) break;
            }

            // Bottom-Left
            for (int r = row - 1, c = col - 1; r >= 0 && c >= 0; r--, c--)
            {
                if (!TryHighlight(r, c)) break;
            }
        }

        void KnightMoves()
        {
            int row = currentTileIndex.x;
            int col = currentTileIndex.y;

            Vector2Int[] knightOffsets = new Vector2Int[]
            {
                new Vector2Int(2, 1), new Vector2Int(2, -1),
                new Vector2Int(-2, 1), new Vector2Int(-2, -1),
                new Vector2Int(1, 2), new Vector2Int(1, -2),
                new Vector2Int(-1, 2), new Vector2Int(-1, -2)
            };

            foreach (var offset in knightOffsets)
            {
                int r = row + offset.x;
                int c = col + offset.y;
                if (r >= 0 && r < 8 && c >= 0 && c < 8) // Avoiding edges
                    TryHighlight(r, c);
            }
        }

        void QueenMoves()
        {
            RookMoves();   // Queen = Rook + Bishop
            BishopMoves();
        }

        void KingMoves()
        {
            int row = currentTileIndex.x;
            int col = currentTileIndex.y;

            Vector2Int[] kingOffsets = new Vector2Int[]
            {
                new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1),
                new Vector2Int(0, -1), new Vector2Int(0, 1),
                new Vector2Int(-1, -1),new Vector2Int(-1, 0), new Vector2Int(-1, 1)
            };

            foreach (var offset in kingOffsets)
            {
                int r = row + offset.x;
                int c = col + offset.y;
                if (r >= 0 && r < 8 && c >= 0 && c < 8) // Avoiding edges
                    TryHighlight(r, c);
            }
        }

        #endregion

        bool TryHighlight(int row, int col)
        {
            var tile = ChessBoardPlacementHandler.Instance.GetTile(row, col)?.transform;

            if (tile == null)
                return false;

            if (tile.childCount > 0)
            {
                var otherPiece = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                if (otherPiece != null && otherPiece.isWhite != this.isWhite)
                {
                    otherPiece.GetComponent<SpriteRenderer>().color = Color.red;
                    ChessBoardPlacementHandler.Instance.enemyHighlights.Add(otherPiece.transform);
                }
                return false; // Occupied — don’t highlight and stop further movement
            }

            ChessBoardPlacementHandler.Instance.Highlight(row, col);
            return true; // Continue checking next tile
        }
    }
}