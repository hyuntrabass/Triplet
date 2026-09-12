using System;

public sealed class ItemState
{
    public ItemDefinition Definition { get; }
    public int Count { get; private set; }

    public bool CanUse => Count > 0;

    public event Action Changed;

    public ItemState(ItemDefinition definition)
    {
        Definition = definition != null ? definition 
            : throw new ArgumentNullException(nameof(definition));
    }

    public void Add(int amount = 1)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        Count += amount;
        Changed?.Invoke();
    }

    public bool TryConsume()
    {
        if (!CanUse)
        {
            return false;
        }

        Count--;
        Changed?.Invoke();
        
        return true;
    }
}
