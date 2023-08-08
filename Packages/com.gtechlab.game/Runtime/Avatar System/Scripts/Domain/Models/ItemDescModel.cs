public enum ItemType {
    Undef,
    Gold = 1,
    Gear = 2,
    
    ItemExt1 = 10, 
    ItemExt2 = 11,
    ItemExt3 = 12,
    ItemExt4 = 13,
    ItemExt5 = 14,

}

public enum ItemFlag {
    Unused = -1,
    Normal = 0, //Disable 
    FreeUse = 1,
    
    InitUse = 11,
}

public abstract class TBaseItemDescModel {
    public ItemType itemType;
    public ItemFlag itemFlag;
    public int itemCode;
    public string itemName;
    public string resourceURL;
    
    public string iconName;
    public int price;
} 

public class TGoldItemDescModle  : TBaseItemDescModel{
    //Fixed!!
    //ItemType = 1,
    //itemCode = 1,

    public TGoldItemDescModle(){
        itemType = ItemType.Gold;
        itemCode = 1;
        itemName = "Gold";
        price = 0;
    }
}