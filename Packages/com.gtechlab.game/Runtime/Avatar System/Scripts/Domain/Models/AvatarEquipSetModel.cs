using System;

[Serializable]
public class TAvatarEquipSetModel {
    public int equipSetId;

    public int hairItemCode;
    public int faceItemCode;
    public int faceAccessoryItemCode;
    public int wearItemCode;
    public int shoesItemCode;
    public int bodyItemCode;

    public void Equip (GearItemType gearItemType, int itemCode) {
        switch (gearItemType) {
            case GearItemType.Hair:
                hairItemCode = itemCode;
                break;
            case GearItemType.Face:
                faceItemCode = itemCode;
                break;
            case GearItemType.FaceAccessory:
                faceAccessoryItemCode = itemCode;
                break;
            case GearItemType.Wear:
                wearItemCode = itemCode;
                break;
            case GearItemType.Shoes:
                shoesItemCode = itemCode;
                break;
            case GearItemType.Body:
                bodyItemCode = itemCode;
                break;
            default:
                break;
        }
    }
    public void Equip (TAvatarEquipSetModel set) {
        hairItemCode = set.hairItemCode;
        faceItemCode = set.faceItemCode;
        faceAccessoryItemCode = set.faceAccessoryItemCode;
        wearItemCode = set.wearItemCode;
        shoesItemCode = set.shoesItemCode;
        bodyItemCode = set.bodyItemCode;
    }

    public int GetGearItemCode (GearItemType gearItemType) {
        switch (gearItemType) {
            case GearItemType.Hair:
                return hairItemCode;
            case GearItemType.Face:
                return faceItemCode;
            case GearItemType.FaceAccessory:
                return faceAccessoryItemCode;
            case GearItemType.Wear:
                return wearItemCode;
            case GearItemType.Shoes:
                return shoesItemCode;
            case GearItemType.Body:
                return bodyItemCode;
            default:
                return 0;
        }
    }
}