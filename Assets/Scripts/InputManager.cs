using Chess.Scripts.Core;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    ChessPlayerPlacementHandler selectedPiece;
    ChessPlayerPlacementHandler newPiece;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                ClearHighlights(); // clearing previous highlights

                // Object was clicked
                ChessPlayerPlacementHandler piece = hit.collider.GetComponent<ChessPlayerPlacementHandler>();
                newPiece = piece;

                if (selectedPiece != newPiece)
                {
                    selectedPiece = newPiece;
                    StartCoroutine(Delay(piece));
                }
                else selectedPiece = newPiece = null;

                return;
            }

            // No object clicked — clicked elsewhere
            selectedPiece = newPiece = null;
            ClearHighlights();
        }
    }

    private void ClearHighlights()
    {
        ChessBoardPlacementHandler.Instance.ClearHighlights();
    }
    private IEnumerator Delay(ChessPlayerPlacementHandler piece)
    {
        yield return null;
        piece.OnPieceClicked();
    }
}
