### Editor

# Create Environment
Assets/G-Tech Lab/Create Creator Environment
 - resource upload api etc
 - scene template 
 
# Create Resource
Assets/G-Tech Lab/Create Scene Resource Template
Assets/G-Tech Lab/Create Asset Resource Template
Assets/G-Tech Lab/Create Resource Definition

# Build
Assets/G-Tech Lab/Build Resource Bundle (& Upload)
Tool/G-Tech Lab/Build All Resource Bundle (& Upload)


### Runtime


# Load
var bundleData = cxResourceBundleLoader.Instance.FindBundleData("com.gtechlab.sample01")
cxResourceBundleLoader.Instance.LoadBundle(bundleData);
await bundleData.WaitForLoad()

bundleData.LoadAsset<>("");
bundleData.LoadScene("");


# Uniform Loader 
CXURL 표기법을 사용
    "" builtin
    resource://<resouceId>/<path>
    bundle://<bundlename>/<path>
    streaming://<path>
    http://
    https://

cxUniversalResourceLoader.Instance.LoadScene("bundle://com.gtechlab.sample01:0/My Scene");
cxUniversalResourceLoader.Instance.LoadAsset<Texture>("bundle://com.gtechlab.sample01:1/MyImage");


