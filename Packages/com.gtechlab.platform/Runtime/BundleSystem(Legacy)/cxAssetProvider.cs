using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System;
//using VoxelBusters.NativePlugins;

[Obsolete]
public class cxAssetProvider : cxSingleton<cxAssetProvider> {
    
    public async Task<string> LoadJson(string path) {
        
        TextAsset textAsset = (await Resources.LoadAsync<TextAsset>(path)) as TextAsset;
        if(textAsset !=null)
            return textAsset.text;
        else
            return null;
    }

    public async Task<Sprite> LoadSpriteAync(string path) {
        Sprite sprite =  await Resources.LoadAsync<Sprite>(path) as Sprite;
         if(sprite !=null)
            return sprite;
        else
            return null;
    }

    public Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path) as Sprite;
        if (sprite != null)
            return sprite;
        else
            return null;
    }

    /*
    public void PickAvatarImage(Action<Texture2D> onCallback)
    {
      //  onCallback(null);
        
        // Set popover to last touch position
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
        NPBinding.MediaLibrary.SetAllowsImageEditing(true);

        // Pick image
        NPBinding.MediaLibrary.PickImage(eImageSource.BOTH, 0.2f, (_reason, _image) =>
        {
            string log1 = ("Request to pick image from gallery finished. Reason for finish is " + _reason + ".");
            string log2 = (string.Format("Selected image is {0}.", (_image == null ? "NULL" : _image.ToString())));

            Debug.Log(log1);
            Debug.Log(log2);

            onCallback(_image);
        });
        
    }


    public Texture2D PickAvatarImageDefined(string path)
    {
        var tex2d = Resources.Load<Texture2D>(path);
        return tex2d;
    }
    */

    static public string ImageToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        string imageBase64 = System.Convert.ToBase64String(imageBytes);

        return imageBase64;
    }

    static public Texture2D Base64ToImage(string imageBase64)
    {
        byte[] imageBytes = System.Convert.FromBase64String(imageBase64);

        Texture2D texture2D = new Texture2D(2, 2);
        bool ok = texture2D.LoadImage(imageBytes);

        return texture2D;
    }
}