using UnityEngine;

public class PlayerStatistics
{
    private bool _isFrozen;

    public int OrderContribution { get; private set; }
    public int WarehouseStoredCount { get; private set; }
    public int WarehouseTakenCount { get; private set; }
    public int OwnWarehouseTakenCount { get; private set; }

    public int OthersWarehouseTakenCount => WarehouseTakenCount - OwnWarehouseTakenCount;

    public void AddOrderContribution(int count)
    {
        if (_isFrozen || count <= 0)
        {
            return;
        }

        OrderContribution += count;
    }

    public void RecordWarehouseStored()
    {
        if (_isFrozen)
        {
            return;
        }

        WarehouseStoredCount++;
    }

    public void RecordWarehouseTaken(bool isOwnSlot)
    {
        if (_isFrozen)
        {
            return;
        }

        WarehouseTakenCount++;

        if (isOwnSlot)
        {
            OwnWarehouseTakenCount++;
        }
    }

    public void Freeze()
    {
        _isFrozen = true;
    }
}
