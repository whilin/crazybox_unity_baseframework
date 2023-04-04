using System;

public enum cxResourceType {
    Asset,
    Scene
}

public enum cxResourceAssetFileType {
    BuiltinAsset = 0,
    BundleAsset = 1,
    BundleScene = 2,
    NoneBundleAsset = 3, //Resource 폴더 내부에 존재하는 파일
    HttpAsset = 4,
}

[Serializable]
public class cxResourceAssetFile {
    public cxResourceAssetFileType fileType;
    public string filename;
}

public class cxResourceDescModel {
    public string resourceId;
    public cxResourceType resourceType;
    public string creatorId;
    public int version;

    public DateTime date;

    public cxResourceAssetFile[] assetFiles;
    public string resourceLocation;
    public string bundleHash;
    public int bundleSize;
    public int releaseFlag;
}