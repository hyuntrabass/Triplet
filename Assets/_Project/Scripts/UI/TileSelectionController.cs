using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectionController : MonoBehaviour
{
    private sealed class TileSnapshot
    {
        private readonly Transform _parent;
        private readonly int _siblingIndex;
        private readonly Vector2 _anchorMin;
        private readonly Vector2 _anchorMax;
        private readonly Vector2 _pivot;
        private readonly Vector3 _anchoredPosition;
        private readonly Vector2 _sizeDelta;
        private readonly Quaternion _localRotation;
        private readonly Vector3 _localScale;
        private readonly bool _wasInteractable;

        public TileView Tile { get; }

        public TileSnapshot(TileView tile)
        {
            Tile = tile;

            var rect = (RectTransform)tile.transform;

            _parent = rect.parent;
            _siblingIndex = rect.GetSiblingIndex();
            _anchorMin = rect.anchorMin;
            _anchorMax = rect.anchorMax;
            _pivot = rect.pivot;
            _anchoredPosition = rect.anchoredPosition3D;
            _sizeDelta = rect.sizeDelta;
            _localRotation = rect.localRotation;
            _localScale = rect.localScale;
            _wasInteractable = tile.IsInteractable;
        }

        public void MoveTo(RectTransform tileLayer)
        {
            Tile.transform.SetParent(tileLayer, true);
            Tile.transform.SetAsLastSibling();
        }

        public void Restore()
        {
            if (Tile == null)
            {
                return;
            }

            Tile.EndSelection();

            var rect = (RectTransform)Tile.transform;

            rect.SetParent(_parent, false);
            rect.SetSiblingIndex(_siblingIndex);
            rect.anchorMin = _anchorMin;
            rect.anchorMax = _anchorMax;
            rect.pivot = _pivot;
            rect.anchoredPosition3D = _anchoredPosition;
            rect.sizeDelta = _sizeDelta;
            rect.localRotation = _localRotation;
            rect.localScale = _localScale;
            Tile.SetInteractable(_wasInteractable);
        }
    }

    [SerializeField]
    private GameObject _selectionView;
    [SerializeField]
    private RectTransform _selectedTileLayer;
    [SerializeField]
    private TMP_Text _promptText;
    [SerializeField]
    private Button _cancelButton;

    private readonly List<TileSnapshot> _snapshot = new();
    private Action<TileView> _selectedHandler;

    public bool IsSelecting => _selectedHandler != null;

    private void Awake()
    {
        if (_selectionView == null ||
            _selectedTileLayer == null ||
            _promptText == null ||
            _cancelButton == null)
        {
            throw new InvalidOperationException("타일 선택 UI 참조가 연결되지 않았습니다.");
        }

        _promptText.raycastTarget = false;
        _selectionView.SetActive(false);
    }

    private void OnEnable()
    {
        _cancelButton.onClick.AddListener(CancelSelection);
    }

    public bool TryBegin(IReadOnlyList<TileView> candidates, string prompt, Action<TileView> selectedHandler)
    {
        if (IsSelecting || candidates == null || candidates.Count == 0)
        {
            return false;
        }

        if (selectedHandler == null)
        {
            throw new ArgumentNullException(nameof(selectedHandler));
        }

        var uniqueTiles = new HashSet<TileView>();

        foreach (var tile in candidates)
        {
            if (tile != null && uniqueTiles.Add(tile))
            {
                _snapshot.Add(new TileSnapshot(tile));
            }
        }

        if (_snapshot.Count == 0)
        {
            return false;
        }

        _selectedHandler = selectedHandler;
        _promptText.text = prompt;
        _selectionView.SetActive(true);

        foreach (var snapshot in _snapshot)
        {
            snapshot.Tile.BeginSelection(HandleTileSelected);
            snapshot.MoveTo(_selectedTileLayer);
        }

        return true;
    }

    public void CancelSelection()
    {
        EndSelection();
    }

    private void HandleTileSelected(TileView tile)
    {
        Action<TileView> selectedHandler = _selectedHandler;

        EndSelection();
        selectedHandler?.Invoke(tile);
    }

    private void EndSelection()
    {
        if (IsSelecting == false)
        {
            return;
        }

        _selectedHandler = null;

        foreach (var snapshot in _snapshot)
        {
            snapshot.Restore();
        }

        _snapshot.Clear();
        _selectionView.SetActive(false);
    }

    private void OnDisable()
    {
        _cancelButton.onClick.RemoveListener(CancelSelection);
        EndSelection();
    }
}
