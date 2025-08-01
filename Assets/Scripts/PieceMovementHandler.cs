using Chess.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Chess.Scripts.Core.ChessPlayerPlacementHandler;

public class PieceMovementHandler:MonoBehaviour
{
    public static PieceMovementHandler Instance;

    private void Start()
    {
        Instance = this;
    }

    public void GetValidMoves(ChessPieceType pieceType, Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        ChessBoardPlacementHandler.Instance.ClearHighlights(); // Clear previous highlights

        switch (pieceType)
        {
            case ChessPieceType.Pawn: PawnMoves(pos, piece); break;
            case ChessPieceType.Rook: RookMoves(pos, piece); break;
            case ChessPieceType.Knight: KnightMoves(pos, piece); break;
            case ChessPieceType.Bishop: BishopMoves(pos, piece); break;
            case ChessPieceType.Queen: QueenMoves(pos, piece); break;
            case ChessPieceType.King: KingMoves(pos, piece); break;
            default: Debug.LogWarning("Unknown piece type"); break;
        }
    }

    #region Movement Logic
    void PawnMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        int row = pos.x;
        int col = pos.y;
        bool isWhite = piece.isWhite;

        int direction = isWhite ? -1 : 1;
        int startRow = isWhite ? 6 : 1;

        // One step forward
        int forwardRow = row + direction;
        if (InBounds(forwardRow, col))
        {
            var forwardTile = ChessBoardPlacementHandler.Instance.GetTile(forwardRow, col);
            if (forwardTile != null && forwardTile.transform.childCount == 0)
            {
                piece.TryHighlight(forwardRow, col);

                // Two steps forward (only if first move and 1-step is empty)
                int twoStepRow = row + 2 * direction;
                var twoStepTile = ChessBoardPlacementHandler.Instance.GetTile(twoStepRow, col);
                if (row == startRow && InBounds(twoStepRow, col) &&
                    twoStepTile != null && twoStepTile.transform.childCount == 0)
                {
                    piece.TryHighlight(twoStepRow, col);
                }
            }
        }

        // Diagonal captures
        Vector2Int[] diagonalOffsets = new Vector2Int[]
        {
        new Vector2Int(direction, -1),
        new Vector2Int(direction, 1)
        };

        foreach (var offset in diagonalOffsets)
        {
            int r = row + offset.x;
            int c = col + offset.y;

            if (InBounds(r, c))
            {
                var tile = ChessBoardPlacementHandler.Instance.GetTile(r, c)?.transform;
                if (tile != null && tile.childCount > 0)
                {
                    var otherPiece = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                    if (otherPiece != null && otherPiece.isWhite != isWhite)
                    {
                        otherPiece.GetComponent<SpriteRenderer>().color = Color.red;
                        ChessBoardPlacementHandler.Instance.enemyHighlights.Add(otherPiece.transform);
                    }
                }
            }
        }
    }

    void RookMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        int row = pos.x;
        int col = pos.y;

        for (int r = row + 1; r < 8; r++) if (!piece.TryHighlight(r, col)) break;
        for (int r = row - 1; r >= 0; r--) if (!piece.TryHighlight(r, col)) break;
        for (int c = col + 1; c < 8; c++) if (!piece.TryHighlight(row, c)) break;
        for (int c = col - 1; c >= 0; c--) if (!piece.TryHighlight(row, c)) break;
    }

    void BishopMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        int row = pos.x;
        int col = pos.y;

        // Top-Right ↗
        for (int r = row + 1, c = col + 1; r < 8 && c < 8; r++, c++)
            if (!piece.TryHighlight(r, c)) break;

        // Top-Left ↖
        for (int r = row + 1, c = col - 1; r < 8 && c >= 0; r++, c--)
            if (!piece.TryHighlight(r, c)) break;

        // Bottom-Right ↘
        for (int r = row - 1, c = col + 1; r >= 0 && c < 8; r--, c++)
            if (!piece.TryHighlight(r, c)) break;

        // Bottom-Left ↙
        for (int r = row - 1, c = col - 1; r >= 0 && c >= 0; r--, c--)
            if (!piece.TryHighlight(r, c)) break;
    }

    void KnightMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        int row = pos.x;
        int col = pos.y;

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
            if (InBounds(r, c))
            {
                piece.TryHighlight(r, c);
            }
        }
    }

    void QueenMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        RookMoves(pos, piece);   // Straight lines
        BishopMoves(pos, piece); // Diagonals
    }

    void KingMoves(Vector2Int pos, ChessPlayerPlacementHandler piece)
    {
        int row = pos.x;
        int col = pos.y;

        Vector2Int[] kingOffsets = new Vector2Int[]
        {
        new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1),
        new Vector2Int(0, -1),                      new Vector2Int(0, 1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1)
        };

        foreach (var offset in kingOffsets)
        {
            int r = row + offset.x;
            int c = col + offset.y;
            if (InBounds(r, c))
            {
                piece.TryHighlight(r, c);
            }
        }
    }

    #endregion


    bool InBounds(int r, int c) => r >= 0 && r < 8 && c >= 0 && c < 8;
}
