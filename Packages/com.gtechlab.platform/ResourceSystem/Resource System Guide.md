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

# Setup
cxResourceBundleLoader.Instance.Create(<resourceLocation>, <repository>);

# Load
var bundleData = cxResourceBundleLoader.Instance.FindBundleData("com.gtechlab.sample01")
cxResourceBundleLoader.Instance.LoadBundle(bundleData);
await bundleData.WaitForLoad()

bundleData.LoadAsset<>("");
bundleData.LoadScene("");


# Uniform Loader 
CXURL 표기법을 사용
    "" builtin
    resource://<resouceId>/<only object name without ext>
    bundle://<bundlename>/<only object name without ext>
    streaming://<streaming path 이하의 full path with ext>
    http://<full path>
    https://<full path>

var scene = await cxUniversalResourceLoader.Instance.LoadScene("bundle://com.gtechlab.sample01:0/My Scene");
var texture = await cxUniversalResourceLoader.Instance.LoadAsset<Texture>("bundle://com.gtechlab.sample01:1/MyImage");


