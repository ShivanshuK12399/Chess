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

    public void GetValidMoves(ChessPieceType pieceType, Vector2Int pos, bool isWhite)
    {
        switch (pieceType)
        {
            case ChessPieceType.Pawn: 
                PawnMoves(pos, isWhite);
                break;
            case ChessPieceType.Rook: 
                RookMoves(pos, isWhite);
                break;
            case ChessPieceType.Knight: 
                KnightMoves(pos, isWhite);
                break;
            case ChessPieceType.Bishop: 
                BishopMoves(pos, isWhite);
                break;
            case ChessPieceType.Queen: 
                QueenMoves(pos, isWhite);
                break;
            case ChessPieceType.King: 
                KingMoves(pos, isWhite);
                break;
            default: new List<Vector2Int>();
                break;
        }
    }


    #region Movement Logic
    void PawnMoves(Vector2Int pos, bool isWhite)
    {
        int row = pos.x;
        int col = pos.y;

        // Up 
        if (row < 8)
        {
            ChessBoardPlacementHandler.Instance.Highlight(row + 1, col);
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

            if (InBounds(r, c)) // Avoiding edges
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

    void RookMoves(Vector2Int pos, bool isWhite)
    {
        int row = pos.x;
        int col = pos.y;

        // Up
        for (int r = row + 1; r < 8; r++)
        {
            if (!TryHighlight(r, col,isWhite)) break;
        }

        // Down
        for (int r = row - 1; r >= 0; r--)
        {
            if (!TryHighlight(r, col, isWhite)) break;
        }

        // Right
        for (int c = col + 1; c < 8; c++)
        {
            if (!TryHighlight(row, c, isWhite)) break;
        }

        // Left
        for (int c = col - 1; c >= 0; c--)
        {
            if (!TryHighlight(row, c, isWhite)) break;
        }
    }

    void BishopMoves(Vector2Int pos, bool isWhite)
    {
        int row = pos.x;
        int col = pos.y;

        // Top-Right
        for (int r = row + 1, c = col + 1; r < 8 && c < 8; r++, c++)
        {
            if (!TryHighlight(r, c, isWhite)) break;
        }

        // Top-Left
        for (int r = row + 1, c = col - 1; r < 8 && c >= 0; r++, c--)
        {
            if (!TryHighlight(r, c, isWhite)) break;
        }

        // Bottom-Right
        for (int r = row - 1, c = col + 1; r >= 0 && c < 8; r--, c++)
        {
            if (!TryHighlight(r, c, isWhite)) break;
        }

        // Bottom-Left
        for (int r = row - 1, c = col - 1; r >= 0 && c >= 0; r--, c--)
        {
            if (!TryHighlight(r, c, isWhite)) break;
        }
    }

    void KnightMoves(Vector2Int pos, bool isWhite)
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
            if (InBounds(r, c)) // Avoiding edges
                TryHighlight(r, c, isWhite);
        }
    }

    void QueenMoves(Vector2Int pos, bool isWhite)
    {
        RookMoves(pos,isWhite);   // Queen = Rook + Bishop
        BishopMoves(pos,isWhite);
    }

    void KingMoves(Vector2Int pos, bool isWhite)
    {
        int row = pos.x;
        int col = pos.y;

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
            if (InBounds(r,c)) // Avoiding edges
                TryHighlight(r, c, isWhite);
        }
    }

#endregion


    bool TryHighlight(int row, int col,bool isWhite)
    {
        var tile = ChessBoardPlacementHandler.Instance.GetTile(row, col)?.transform;

        if (tile == null)
            return false;

        if (tile.childCount > 0)
        {
            var otherPiece = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
            if (otherPiece != null && otherPiece.isWhite != isWhite)
            {
                otherPiece.GetComponent<SpriteRenderer>().color = Color.red;
                ChessBoardPlacementHandler.Instance.enemyHighlights.Add(otherPiece.transform);
            }
            return false; // Occupied — don’t highlight and stop further movement
        }

        ChessBoardPlacementHandler.Instance.Highlight(row, col);
        return true; // Continue checking next tile
    }

    bool InBounds(int r, int c) => r >= 0 && r < 8 && c >= 0 && c < 8;
}
