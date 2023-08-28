using UnityEngine;

public static class cxLog {

    static bool verbose = true;

    static cxLog () {
        //  UnityEngine.Debug.unityLogger.logHandler ;
    }

    public static void VerboseOn (bool _verbose) {
        verbose = _verbose;
    }

    public static void Verbose (string format, params object[] args) {
        if (verbose) {
            if (args == null || args.Length == 0) {
                format = format.Replace ("{", "{{");
                format = format.Replace ("}", "}}");
            }
            Debug.LogFormat (LogType.Log, LogOption.NoStacktrace, null, format, args);
        }
    }

    public static void Log (string format, params object[] args) {

        if (args == null || args.Length == 0) {
            format = format.Replace ("{", "{{");
            format = format.Replace ("}", "}}");
        }

        Debug.LogFormat (LogType.Log, LogOption.NoStacktrace, null, format, args);
    }

    public static void Warning (string format, params object[] args) {
        if (args == null || args.Length == 0) {
            format = format.Replace ("{", "{{");
            format = format.Replace ("}", "}}");
        }

        Debug.LogFormat (LogType.Warning, LogOption.NoStacktrace, null, format, args);
    }

    public static void Error (string format, params object[] args) {
        if (args == null || args.Length == 0) {
            format = format.Replace ("{", "{{");
            format = format.Replace ("}", "}}");
        }

        Debug.LogFormat (LogType.Error, LogOption.None, null, format, args);
    }

    public static void Exception (System.Exception ex) {
        Debug.LogError (ex);
    }
}