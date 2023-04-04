#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class cxEditorWindowUtils {

   // [MenuItem ("Vaiv/Message Show")]
    static void Test () {
        // MessageWindow.Show("테스트입니다.");

        EditorUtility.DisplayDialog ("메시지", "테스트입니다", "확인");
    }

    public static void ShowMessage (string message) {
        EditorUtility.DisplayDialog (string.Empty, message, "확인");
    }

    public static void ShowErrorMessage (string title, string message) {
        EditorUtility.DisplayDialog (title, message, "확인");
    }

    public static bool AskOpenViewer () {
        return EditorUtility.DisplayDialog ("리소스 업로드 완료", "웹에서 등록된 리소스를 확인할 수 있습니다.", "뷰어 오픈", "닫기");
    }
}

#endif