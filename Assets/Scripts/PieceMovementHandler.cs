using Chess.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;
using static Chess.Scripts.Core.ChessPlayerPlacementHandler;

public static class PieceMovementHandler
{
    public static List<Vector2Int> GetValidMoves(ChessPieceType pieceType, Vector2Int pos, bool isWhite)
    {
        switch (pieceType)
        {
            case ChessPieceType.Pawn: return PawnMoves(pos, isWhite);
            case ChessPieceType.Rook: return RookMoves(pos, isWhite);
            case ChessPieceType.Knight: return KnightMoves(pos, isWhite);
            case ChessPieceType.Bishop: return BishopMoves(pos, isWhite);
            case ChessPieceType.Queen: return QueenMoves(pos, isWhite);
            case ChessPieceType.King: return KingMoves(pos, isWhite);
            default: return new List<Vector2Int>();
        }
    }

    private static List<Vector2Int> PawnMoves(Vector2Int pos, bool isWhite)
    {
        var moves = new List<Vector2Int>();
        int dir = isWhite ? -1 : 1;

        int forwardRow = pos.x + dir;
        int col = pos.y;

        // Move forward if empty
        if (InBounds(forwardRow, col))
        {
            var forwardTile = ChessBoardPlacementHandler.Instance.GetTile(forwardRow, col)?.transform;
            if (forwardTile != null && forwardTile.childCount == 0)
                moves.Add(new Vector2Int(forwardRow, col));
        }

        // Diagonal captures
        foreach (int offset in new int[] { -1, 1 })
        {
            int diagCol = col + offset;
            if (!InBounds(forwardRow, diagCol)) continue;

            var tile = ChessBoardPlacementHandler.Instance.GetTile(forwardRow, diagCol)?.transform;
            if (tile != null && tile.childCount > 0)
            {
                var other = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                if (other != null && other.isWhite != isWhite)
                {
                    other.GetComponent<SpriteRenderer>().color = Color.red;
                    ChessBoardPlacementHandler.Instance.enemyHighlights.Add(other.transform);
                }
            }
        }

        return moves;
    }

    private static List<Vector2Int> RookMoves(Vector2Int pos, bool isWhite)
    {
        return GetMovesInDirections(pos, isWhite, new Vector2Int[] {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
        });
    }

    private static List<Vector2Int> BishopMoves(Vector2Int pos, bool isWhite)
    {
        return GetMovesInDirections(pos, isWhite, new Vector2Int[] {
            new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
        });
    }

    private static List<Vector2Int> QueenMoves(Vector2Int pos, bool isWhite)
    {
        return GetMovesInDirections(pos, isWhite, new Vector2Int[] {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
            new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
        });
    }

    private static List<Vector2Int> KnightMoves(Vector2Int pos, bool isWhite)
    {
        var moves = new List<Vector2Int>();
        Vector2Int[] offsets = new Vector2Int[]
        {
            new(2, 1), new(2, -1), new(-2, 1), new(-2, -1),
            new(1, 2), new(1, -2), new(-1, 2), new(-1, -2)
        };

        foreach (var offset in offsets)
        {
            int r = pos.x + offset.x;
            int c = pos.y + offset.y;
            if (!InBounds(r, c)) continue;

            var tile = ChessBoardPlacementHandler.Instance.GetTile(r, c)?.transform;
            if (tile == null) continue;

            if (tile.childCount == 0)
            {
                moves.Add(new Vector2Int(r, c));
            }
            else
            {
                var other = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                if (other != null && other.isWhite != isWhite)
                {
                    other.GetComponent<SpriteRenderer>().color = Color.red;
                    ChessBoardPlacementHandler.Instance.enemyHighlights.Add(other.transform);
                }
            }
        }

        return moves;
    }

    private static List<Vector2Int> KingMoves(Vector2Int pos, bool isWhite)
    {
        var moves = new List<Vector2Int>();
        Vector2Int[] offsets = new Vector2Int[]
        {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
            new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
        };

        foreach (var offset in offsets)
        {
            int r = pos.x + offset.x;
            int c = pos.y + offset.y;
            if (!InBounds(r, c)) continue;

            var tile = ChessBoardPlacementHandler.Instance.GetTile(r, c)?.transform;
            if (tile == null) continue;

            if (tile.childCount == 0)
            {
                moves.Add(new Vector2Int(r, c));
            }
            else
            {
                var other = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                if (other != null && other.isWhite != isWhite)
                {
                    other.GetComponent<SpriteRenderer>().color = Color.red;
                    ChessBoardPlacementHandler.Instance.enemyHighlights.Add(other.transform);
                }
            }
        }

        return moves;
    }

    private static List<Vector2Int> GetMovesInDirections(Vector2Int pos, bool isWhite, Vector2Int[] directions)
    {
        var moves = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            for (int i = 1; i < 8; i++)
            {
                int r = pos.x + dir.x * i;
                int c = pos.y + dir.y * i;
                if (!InBounds(r, c)) break;

                var tile = ChessBoardPlacementHandler.Instance.GetTile(r, c)?.transform;
                if (tile == null) break;

                if (tile.childCount == 0)
                {
                    moves.Add(new Vector2Int(r, c));
                }
                else
                {
                    var other = tile.GetComponentInChildren<ChessPlayerPlacementHandler>();
                    if (other != null && other.isWhite != isWhite)
                    {
                        other.GetComponent<SpriteRenderer>().color = Color.red;
                        ChessBoardPlacementHandler.Instance.enemyHighlights.Add(other.transform);
                    }
                    break;
                }
            }
        }

        return moves;
    }

    private static bool InBounds(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;
}
