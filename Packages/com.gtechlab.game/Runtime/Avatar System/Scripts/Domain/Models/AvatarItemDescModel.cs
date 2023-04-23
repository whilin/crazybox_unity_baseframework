public enum GearItemType {
    Undef,
    Body = 1,
    Face = 2,
    Wear = 3,
    Pants = 4,
    Shoes = 5,

    Hair = 10,
    FaceAccessory = 11,
    Hat = 12,
    Backpack = 13,

    MAX_SLOT_SIZE = 14
}

public class TAvatarItemDescModel : TBaseItemDescModel {
    public GearItemType gearItemType;
}
