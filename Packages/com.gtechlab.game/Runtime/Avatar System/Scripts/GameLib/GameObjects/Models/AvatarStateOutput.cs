
public class cxAvatarStateOutput {
    public bool grounded;
    public bool jump;
    public bool freeFall;
    public float speed;

    public void Reset () {
        grounded = false;
        jump = false;
        freeFall = false;
        speed = 0.0f;
    }
}