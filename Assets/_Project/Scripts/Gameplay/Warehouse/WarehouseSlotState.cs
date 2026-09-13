using System;

public sealed class WarehouseSlotState
{
    public const int RequiredCharge = 3;
    
    public PlayerController Owner { get; }
    public int Charge { get; private set; }
    public TileView Tile { get; private set; }

    public WarehouseSlotStatus Status
    {
        get
        {
            if (Tile != null)
            {
                return WarehouseSlotStatus.Occupied;
            }

            if (Charge >= RequiredCharge)
            {
                return WarehouseSlotStatus.Ready;
            }

            return WarehouseSlotStatus.Closed;
        }
    }

    public event Action Changed;

    public WarehouseSlotState(PlayerController owner)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));
        }

        Owner = owner;
    }

    public void AddMatchedTiles(int removedTileCount)
    {
        if (removedTileCount < 3)
        {
            return;
        }

        int gainedCharge = removedTileCount / 3;
        int newCharge = Math.Min(RequiredCharge, Charge + gainedCharge);

        if (newCharge == Charge)
        {
            return;
        }

        Charge = newCharge;
        Changed?.Invoke();
    }

    public bool TryStore(TileView tile)
    {
        if (tile == null)
        {
            throw new ArgumentNullException(nameof(tile));
        }

        if (Status != WarehouseSlotStatus.Ready)
        {
            return false;
        }

        Tile = tile;
        Changed?.Invoke();

        return true;
    }

    public bool TryTake(out TileView tile)
    {
        if (Status != WarehouseSlotStatus.Occupied)
        {
            tile = null;
            return false;
        }

        tile = Tile;
        Tile = null;
        Charge = 0;

        Changed?.Invoke();

        return true;
    }
}
