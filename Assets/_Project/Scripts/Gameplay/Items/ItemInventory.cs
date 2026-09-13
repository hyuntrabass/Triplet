using System;
using System.Collections.Generic;

public class ItemInventory
{
    private readonly List<ItemState> _states = new();

    public IReadOnlyList<ItemState> States => _states;
    
    public ItemInventory(IReadOnlyList<ItemDefinition> loadout)
    {
        if (loadout == null)
        {
            throw new ArgumentNullException(nameof(loadout));
        }

        if (loadout.Count == 0)
        {
            throw new ArgumentException("아이템 구성은 하나 이상이어야 합니다.", nameof(loadout));
        }

        foreach (var definition in loadout)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("아이템 구성에 비어 있는 항목이 있습니다.", nameof(definition));
            }

            _states.Add(new ItemState(definition));
        }
    }

    public ItemState GrantRandomItem()
    {
        int index = UnityEngine.Random.Range(0, _states.Count);
        ItemState grantedState = _states[index];

        grantedState.Add();

        return grantedState;
    }
}
