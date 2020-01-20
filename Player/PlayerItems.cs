using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

public class PlayerItems : MonoBehaviour {


    ItemInfo[] _allItems;
    List<ItemInfo> _items = new List<ItemInfo>();

    public List<ItemInfo> items { get { return _items; } }


    //Carga de ItemInfo desde Resources para comparar con el array de items en la carga del juego
    private void Awake()
    {
        _allItems = Resources.LoadAll<ItemInfo>("Items");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemBox itemBox = other.GetComponent<ItemBox>();

        if(itemBox != null)
        {
            ItemInfo itemInfo = itemBox.itemInfo;
            AddItem(itemInfo);

            Destroy(itemBox.gameObject);
        }

    }

    private void AddItem(ItemInfo itemInfo)
    {
        bool alreadyHasItem = CheckHasItem(itemInfo);

        //Si después de la comprobación, no está el item, lo añado
        if (!alreadyHasItem)
        {
            _items.Add(itemInfo);
        }


        switch (itemInfo.type)
        {
            case ItemType.DoubleJump:
                GetComponent<PlayerJump>().SecondJumpEnabled = true;
                break;

            case ItemType.Dashing:
                GetComponent<PlayerWalk>().DashEnabled = true;
                GetComponent<PlayerJump>().DashEnabled = true;
                break;

            case ItemType.UltraFire:
                GetComponent<PlayerWalk>().ChargedShootEnabled = true;
                break;
        }
    }

    private bool CheckHasItem(ItemInfo itemInfo)
    {
        //Comprobación de si el item está en la lista
        bool alreadyHasItem = false;
        for (int i = 0; i < _items.Count && !alreadyHasItem; i++)
        {
            if (_items[i].type == itemInfo.type)
            {
                alreadyHasItem = true;
            }
        }

        return alreadyHasItem;
    }

    public ItemType[] GetItemsData()
    {
        ItemType[] itemTypes = new ItemType[_items.Count];
        for (int i = 0; i < _items.Count; i++)
        {
            itemTypes[i] = _items[i].type;
        }

        return itemTypes;
    }

    public void SetItemsData(ItemType[] itemTypes)
    {
        _items.Clear(); // Borra el contenido del array de items

        //Por cada tipo de item que me indica el JSON
        for(int i=0; i<itemTypes.Length; i++)
        {
            ItemType type = itemTypes[i];

            //Busco en el listado de todos los items el del tipo correcto
            bool itemAdded = false;
            for (int j=0; j<_allItems.Length && !itemAdded; j++)
            {
                ItemInfo possibleItem = _allItems[j];

                //Cuando encuentro el item del tipo correcto
                if(possibleItem.type == type)
                {
                    //Lo añado a mi lista de items
                    AddItem(possibleItem);
                    itemAdded = true;
                }
            }
        }
    }
}
