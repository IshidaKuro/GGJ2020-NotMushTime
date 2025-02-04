﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAHole_PickingArea : MonoBehaviour
{
    public delegate void PieceDelegate(FixAHole_Piece piece);
    public event PieceDelegate OnPieceSelected;

    TriggerPuzzleSelect triggerPuzzleSelect;

    public bool CanPickPiece = true;

    private int selectedPiece = 0;

    private List<FixAHole_Piece> pieces = new List<FixAHole_Piece>();

    private List<FixAHole_PieceAsset> piecesPool;

    private FixAHole_Piece piecePrefab;

    [SerializeField]
    private float TimeBetweenAddingPieces = 1.0f;

    [SerializeField]
    private float FirstPieceXOffset = -5.0f;

    [SerializeField]
    private float NextPieceXOffset = 2.5f;

    float pieceAddTimer = 0.0f;

    public PlayerController Picker;

    private void Start()
    {
        triggerPuzzleSelect = GetComponent<TriggerPuzzleSelect>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        piecesPool = new List<FixAHole_PieceAsset>(Resources.LoadAll<FixAHole_PieceAsset>("FixAHole/Pieces"));
        piecePrefab = Resources.Load<FixAHole_Piece>("FixAHole/Prefabs/Piece");
    }

    public void SelectPiece(int pieceId)
    {
        pieceId = Mathf.Clamp(pieceId, 0, 4);
        selectedPiece = pieceId;

        for(int i = 0; i < pieces.Count; ++i)
        {
            pieces[i].SetHighlight(i == pieceId);
        }
    }

    private FixAHole_Piece GenerateNewPiece()
    {
        FixAHole_Piece newPiece = Instantiate(piecePrefab);
        newPiece.Initialize(piecesPool[Random.Range(0, piecesPool.Count)]);

        return newPiece;
    }

    private void AddNewPiece(FixAHole_Piece newPiece)
    {
        // Make sure scale is properly set
        newPiece.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        newPiece.transform.SetParent(transform);
        pieces.Add(newPiece);

        // Update selection
        SelectPiece(selectedPiece);
    }

    private void FixedUpdate()
    {
        // Keep adding pieces one by one until we have 5
        if(pieces.Count < 5)
        {
            if(pieceAddTimer <= 0.0f)
            {
                AddNewPiece(GenerateNewPiece());
                pieceAddTimer = TimeBetweenAddingPieces;
            }
            else
            {
                pieceAddTimer -= Time.fixedDeltaTime;
            }
        }

        // Slide the pieces to the left
        for(int i = 0; i < pieces.Count; ++i)
        {
            FixAHole_Piece piece = pieces[i];
            float desiredX = FirstPieceXOffset + i * NextPieceXOffset;
            if (piece.transform.localPosition.x > desiredX)
            {
                piece.transform.localPosition = new Vector3(Mathf.Lerp(piece.transform.localPosition.x, desiredX, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, 0.3f))), 0.0f, 0.0f);
            }
        }

        if(Picker.HorizontalPress < 0)
        {
            SelectPiece(selectedPiece - 1);

            if (triggerPuzzleSelect)
                triggerPuzzleSelect.Trigger();
        }
        if(Picker.HorizontalPress > 0)
        {
            SelectPiece(selectedPiece + 1);

            if (triggerPuzzleSelect)
                triggerPuzzleSelect.Trigger();
        }

        if(Picker.IsGreenDown())
        {
            if(CanPickPiece && OnPieceSelected != null)
            {
                OnPieceSelected.Invoke(pieces[selectedPiece]);
                pieces[selectedPiece].IsHeld = true;
                pieces.Remove(pieces[selectedPiece]);

                if (triggerPuzzleSelect)
                    triggerPuzzleSelect.Trigger();
            }
        }
    }
}
