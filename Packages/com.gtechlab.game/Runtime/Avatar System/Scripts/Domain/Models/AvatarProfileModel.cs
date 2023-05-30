using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TAvatarProfileModel
{
    public string userKey;
    public string nickname;
    public string greeting;
    public string photo;
   // public int gender;

    public TAvatarEquipSetModel equipSet;
}

[Serializable]
public class TUserInvenItemModel {
    public ItemType itemType;
    public int itemCode;
    public int invenCode;
    public int quantity;
    public int flag;
}

public enum TInvenItemFlag {
    Undef =0 ,
    New = 1
}