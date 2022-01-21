
public abstract class cxSingleton<T> where T : class,  new() {

    private static T instance;
    public static T Instance {
        get {
            if (instance != null)
                return instance;
            else {
                instance = new T ();
                return instance;
            }
        }
    }
}