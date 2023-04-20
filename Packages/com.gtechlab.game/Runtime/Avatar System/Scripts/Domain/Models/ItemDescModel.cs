public enum ItemType {
    Undef,
    Gold = 1,
    Gear = 2,

    QuestItem = 10,
}

public enum ItemFlag {
    Undef = 0,
    New = 1,
    Hot = 2,
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