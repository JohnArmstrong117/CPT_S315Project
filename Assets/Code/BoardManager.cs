using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Board Aspects
    public GameObject tilePrefab;
    public int boardSize;
    public float tileOffset;

    //Info Trackers
    public int moveNum;
    public int turnNum;
    public string moveList;
    private List<Opening> mathingOpenings;

    // Peices
    public GameObject whitePawnPrefab;
    public GameObject blackPawnPrefab;
    public GameObject whiteRookPrefab;
    public GameObject blackRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject blackKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject blackBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject blackQueenPrefab;
    public GameObject whiteKingPrefab;
    public GameObject blackKingPrefab;

    //Input Vars
    private bool pSelected;
    private GameObject piece;
    private Vector3 offset;

    //Other
    private float tileSize = 1.0f;
    const string letters = "abcdefghijklmnopqrstuvwxyz";
    public OpeninngSorter sorter;
    public UIManager uiManager;

    private void Start()
    {
        CreateBoard();
        SetupPieces();
        Invoke("LateStart", 3);
    }

    private void Update()
    {
        if (!pSelected)
        {
            CheckPeiceGrab();
        }
        else
        {
            UpdatePiecePosition();
            PlacePiece();
        }
    }

    private void LateStart()
    {
        pSelected = false;
        piece = null;
        moveNum = 0;
        turnNum = 1;
        moveList = string.Empty;
        sorter = gameObject.GetComponent<OpeninngSorter>();
        mathingOpenings = sorter.GetSortedOpenings();
        foreach (var opening in mathingOpenings)
        {
            Debug.Log(opening.Moves);
        }
        UpdateNextMoves();
        moveNum++;
    }

    private void CreateBoard()
    {
        for(int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x * tileOffset, y * tileOffset, 0), Quaternion.identity);
                tile.transform.parent = transform;

                // Alternate tile colors
                if ((x + y) % 2 == 0)
                {
                    tile.GetComponent<SpriteRenderer>().color = new Color(0.30f, 0.08f, 0.08f);
                }
                else
                {
                    tile.GetComponent<SpriteRenderer>().color = new Color(0.92f, 0.72f, 0.51f);  
                }
            }
        }
    }

    void SetupPieces()
    {
        // Instantiate pawns
        for (int i = 0; i < 8; i++)
        {
            Instantiate(whitePawnPrefab, GetTileCenter(i, 1), Quaternion.identity);
            Instantiate(blackPawnPrefab, GetTileCenter(i, 6), Quaternion.identity);
        }

        // Instantiate rooks
        Instantiate(whiteRookPrefab, GetTileCenter(0, 0), Quaternion.identity);
        Instantiate(whiteRookPrefab, GetTileCenter(7, 0), Quaternion.identity);
        Instantiate(blackRookPrefab, GetTileCenter(0, 7), Quaternion.identity);
        Instantiate(blackRookPrefab, GetTileCenter(7, 7), Quaternion.identity);

        // Instantiate knights
        Instantiate(whiteKnightPrefab, GetTileCenter(1, 0), Quaternion.identity);
        Instantiate(whiteKnightPrefab, GetTileCenter(6, 0), Quaternion.identity);
        Instantiate(blackKnightPrefab, GetTileCenter(1, 7), Quaternion.identity);
        Instantiate(blackKnightPrefab, GetTileCenter(6, 7), Quaternion.identity);

        // Instantiate bishops
        Instantiate(whiteBishopPrefab, GetTileCenter(2, 0), Quaternion.identity);
        Instantiate(whiteBishopPrefab, GetTileCenter(5, 0), Quaternion.identity);
        Instantiate(blackBishopPrefab, GetTileCenter(2, 7), Quaternion.identity);
        Instantiate(blackBishopPrefab, GetTileCenter(5, 7), Quaternion.identity);

        // Instantiate queens
        Instantiate(whiteQueenPrefab, GetTileCenter(3, 0), Quaternion.identity);
        Instantiate(blackQueenPrefab, GetTileCenter(3, 7), Quaternion.identity);

        // Instantiate kings
        Instantiate(whiteKingPrefab, GetTileCenter(4, 0), Quaternion.identity);
        Instantiate(blackKingPrefab, GetTileCenter(4, 7), Quaternion.identity);
    }

    Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, y * tileSize, -1);
    }

    void CheckPeiceGrab()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

            if (hit.collider != null)
            {
                // set active piece Piece
                Debug.Log("Collision not null: " + hit.collider.gameObject.name);
                piece = hit.collider.gameObject;

                if ((piece.tag.StartsWith("W") && moveNum % 2 != 0) || (piece.tag.StartsWith("B") && moveNum % 2 == 0))
                {
                    piece.GetComponent<Collider2D>().enabled = false;
                    offset = piece.transform.position - (Vector3)(mousePosition);
                    offset.x = offset.x + .7f;
                    pSelected = true;
                }
            }
        }
    }

    void UpdatePiecePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        piece.transform.position = (Vector3)mousePosition + offset;
    }

    void PlacePiece()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if ((mousePosition.x < 8 && mousePosition.x >= 0) && (mousePosition.y < 8 && mousePosition.y >= 0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

                if (hit.collider != null)
                {
                    GameObject pieceBelow = hit.collider.gameObject;
                    if (pieceBelow != null)
                    {
                        if (pieceBelow.tag[0] != piece.tag[0])
                        {
                            int origXPosition = (int)(Math.Round(piece.transform.position.x, 0, MidpointRounding.AwayFromZero));
                            piece.transform.position = pieceBelow.transform.position;
                            pSelected = false;
                            pieceBelow.SetActive(false);
                            piece.GetComponent<Collider2D>().enabled = true;

                            //Updating move list upon taking another piece
                            UpdateMoveList((int)Math.Round(piece.transform.position.x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(piece.transform.position.y, 0, MidpointRounding.AwayFromZero), true, origXPosition);
                            //Updating Matching Openings upon taking another peice
                            UpdateMatchingOpenings();
                            moveNum++;
                            if (moveNum % 2 == 0)
                            {
                                turnNum++;
                            }
                        }
                    }
                }
                else
                {
                    pSelected = false;
                    Vector3 newPosition = new Vector3((float)Math.Round(mousePosition.x, 0, MidpointRounding.AwayFromZero), (float)Math.Round(mousePosition.y, 0, MidpointRounding.AwayFromZero), -1);
                    piece.transform.position = newPosition;
                    piece.GetComponent<Collider2D>().enabled = true;

                    //Updating move list upon moving without taking
                    UpdateMoveList((int)newPosition.x, (int)newPosition.y, false, 0);
                    //Updating Matching openings upon moving without taking
                    UpdateMatchingOpenings();
                    moveNum++;
                    if (moveNum % 2 == 0)
                    {
                        turnNum++;
                    }
                }
            }
        }
    }

    void UpdateMoveList(int col, int row, bool capture, int origCol)
    {
        if (moveNum > 1)
        {
            moveList += ", ";
        }
        //append starting info
        if(moveNum % 2 != 0)
        {
            moveList += "'" + turnNum.ToString() + ".";
        }
        else
        {
            moveList += "'";
        }

        //append piece notation (except for pawn)
        string pieceName = piece.tag.Substring(5);
        switch(pieceName)
        {
            case "King":
                moveList += "K";
                break;
            case "Bishop":
                moveList += "B";
                break;
            case "Queen":
                moveList += "Q";
                break;
            case "Knight":
                moveList += "N";
                break;
            case "Pawn":
                if (capture)
                {
                    moveList += letters[origCol];
                }
                break;
            case "Rook":
                moveList += "R";
                break;
            default:
                break;
        }

        if (capture)
        {
            moveList += "x";
        }

        moveList += letters[col] + (row + 1).ToString() + "'";
        Debug.Log(moveList);
    }

    void UpdateMatchingOpenings()
    {
        mathingOpenings = mathingOpenings.FindAll(opening => opening.Moves.StartsWith(moveList));
        Debug.Log(mathingOpenings.Count);
        //Updating the correct next moves for all openings that still apply
        UpdateNextMoves();
        uiManager.UpdateOpeningList(mathingOpenings);
    }

    void UpdateNextMoves()
    {
        if (mathingOpenings.Count <= 0)
        {
            return;
        }

        string[] movesMade = moveList.Split(' ');
        foreach (Opening op in mathingOpenings)
        {
            string[] openingMoves = op.Moves.Split(" ");
            if (moveNum > 0)
            {
                if (openingMoves.Length > movesMade.Length)
                {
                    op.NextMove = openingMoves[movesMade.Length];
                }
                else
                {
                    op.NextMove = "Line Finished";
                }
            }
            else
            {
                op.NextMove = openingMoves[0];
            }
        }
    }

    public List<Opening> GetMatchingOpenings()
    {
        return mathingOpenings;
    }
}
