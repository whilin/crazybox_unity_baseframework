using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class cxAvatarSandboxApplication : MonoBehaviour
{
   private void Awake() {
       SetupServiceLocator();
   }

   void SetupServiceLocator(){
       cxGetIt.RegisterSingleton<cxIGameItemBloc>(new cxDefaultGameItemBloc());
       cxGetIt.RegisterSingleton<cxIGamePlayerBloc>(new cxDefaultGamePlayerBloc());
   }
}
