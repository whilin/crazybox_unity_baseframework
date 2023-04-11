public enum GearItemType {
    Undef,
    Hair = 1,
    Face = 2,
    FaceAccessory = 3,
    Wear = 4,
    Shoes = 5,
    Body = 10
}

public class TAvatarItemDescModel : TBaseItemDescModel {
    public GearItemType gearItemType;
}
