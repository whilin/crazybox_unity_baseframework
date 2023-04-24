using System;

[Serializable]
public class TAvatarEquipSetModel {
    public int equipSetId;

    public int bodyItemCode;
    public int faceItemCode;
    public int wearItemCode;
    public int pantsItemCode;
    public int shoesItemCode;

    public int hairItemCode;
    public int faceAccessoryItemCode;
    public int hatItemCode;
    public int backpackItemCode;

    public int this [int index] {
        get {
            return GetGearItemCode ((GearItemType) index);
        }
    }

    public void Equip (GearItemType gearItemType, int itemCode) {
        switch (gearItemType) {
            case GearItemType.Body:
                bodyItemCode = itemCode;
                break;
            case GearItemType.Face:
                faceItemCode = itemCode;
                break;
            case GearItemType.Wear:
                wearItemCode = itemCode;
                break;
            case GearItemType.Pants:
                pantsItemCode = itemCode;
                break;
            case GearItemType.Shoes:
                shoesItemCode = itemCode;
                break;
            case GearItemType.Hair:
                hairItemCode = itemCode;
                break;
            case GearItemType.FaceAccessory:
                faceAccessoryItemCode = itemCode;
                break;
            case GearItemType.Hat:
                hatItemCode = itemCode;
                break;
            case GearItemType.Backpack:
                backpackItemCode = itemCode;
                break;

            default:
                break;
        }
    }

    public void Equip (TAvatarEquipSetModel set) {
        bodyItemCode = set.bodyItemCode;
        faceItemCode = set.faceItemCode;
        wearItemCode = set.wearItemCode;
        pantsItemCode = set.pantsItemCode;
        shoesItemCode = set.shoesItemCode;

        hairItemCode = set.hairItemCode;
        faceAccessoryItemCode = set.faceAccessoryItemCode;
        hatItemCode = set.hatItemCode;
        backpackItemCode = set.backpackItemCode;
    }

    public int GetGearItemCode (GearItemType gearItemType) {
        switch (gearItemType) {
            case GearItemType.Body:
                return bodyItemCode;
            case GearItemType.Face:
                return faceItemCode;
            case GearItemType.Wear:
                return wearItemCode;
            case GearItemType.Pants:
                return pantsItemCode;
            case GearItemType.Shoes:
                return shoesItemCode;
            case GearItemType.Hair:
                return hairItemCode;
            case GearItemType.FaceAccessory:
                return faceAccessoryItemCode;
            case GearItemType.Hat:
                return hatItemCode;
            case GearItemType.Backpack:
                return backpackItemCode;
            default:
                return 0;
        }
    }

    public TAvatarEquipSetModel Clone () {
        return new TAvatarEquipSetModel () {
            bodyItemCode = bodyItemCode,
                faceItemCode = faceItemCode,
                wearItemCode = wearItemCode,
                pantsItemCode = pantsItemCode,
                shoesItemCode = shoesItemCode,
                hairItemCode = hairItemCode,
                faceAccessoryItemCode = faceAccessoryItemCode,
                hatItemCode = hatItemCode,
                backpackItemCode = backpackItemCode
        };
    }


    public void Set (TAvatarEquipSetModel set) {
        equipSetId = set.equipSetId;
        
        bodyItemCode = set.bodyItemCode;
        faceItemCode = set.faceItemCode;
        wearItemCode = set.wearItemCode;
        pantsItemCode = set.pantsItemCode;
        shoesItemCode = set.shoesItemCode;

        hairItemCode = set.hairItemCode;
        faceAccessoryItemCode = set.faceAccessoryItemCode;
        hatItemCode = set.hatItemCode;
        backpackItemCode = set.backpackItemCode;
    }
}