using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cxUIInputTextDialog : cxUIFrame
{
    public Button closeButton;
    public TMP_InputField inputField;
    public Button okButton;
    
    protected override void OnInit()
    {
        closeButton.onClick.AddListener(Pop);
        okButton.onClick.AddListener(()=>{
            PopResult(inputField.text);
        });
    }

    protected override void OnActivated(object showParam)
    {
        string initText =showParam !=null? showParam as string : string.Empty;
        inputField.text = initText;
    }

    protected override void OnDeactivated()
    {
        
    }


   
}
