using UnityEngine;
using System.Collections;
using System;

public class MathUtil
{
    static public Vector3 NormalizeRotation(Vector3 rot)
    {
        rot.x = Mathf.Repeat(rot.x, 360);
        rot.y = Mathf.Repeat(rot.y, 360);
        rot.z = Mathf.Repeat(rot.z, 360);

        if (rot.x > 180) rot.x -= 360;
        if (rot.y > 180) rot.y -= 360;
        if (rot.z > 180) rot.z -= 360;

        return rot;
    }

    static public Vector3 ConstantSlerp(Vector3 from, Vector3 to, float maxAngleVelocity)
    {
        float t = Mathf.Min(1, maxAngleVelocity / Vector3.Angle(from, to));
        return Vector3.Slerp(from, to, t);
    }

    static public Vector3 ConstantLerp(Vector3 from, Vector3 to, float maxLinearVelocity)
    {
        float t = Mathf.Min(1, maxLinearVelocity / Vector3.Distance(from, to));
        return Vector3.Lerp(from, to, t);
    }

    static public float CalcAngle(Vector3 from, Vector3 to, Vector3 up)
    {
        //Note, 각의 +/- 는 왼손 법칙을 따른다.

        float angle = Vector3.Angle(from, to);

        Vector3 cross = Vector3.Cross(from, to);
        float dot = Vector3.Dot(cross, up);
        if (dot < 0.0f)
            angle = -angle;

        return angle;
    }


    static public float CalcAngleVertical(Vector3 from, Vector3 to, Vector3 up)
    {
        //Note, 각의 +/- 는 왼손 법칙을 따른다.

        to.x = from.x;
        to.z = from.z;

        float angle = Vector3.Angle(from, to);

        Vector3 cross = Vector3.Cross(from, to);
        float dot = Vector3.Dot(cross, up);
        if (dot < 0.0f)
            angle = -angle;

        return angle;
    }
    static public float CalcAngleHorizontal(Vector3 pos, Vector3 myView, Vector3 targetView)
    {
        throw new Exception("Not Implement!!");
    }

    static public Vector3 HorizontalDirection(Vector3 myPos, Vector3 targetPos)
    {
        Vector3 view = targetPos - myPos;
        view.y = myPos.y;
        view.Normalize();
        
        return view;
    }

    static public Vector3 RotateView(Vector3 view, Vector3 relativeEulerRot)
    {
        //Note, 알고리즘
        /*
		 * 	- Quaternion 의 기본값은 Vector3.forward ( 0,0,1) 을 기준으로 하고 있다
		 *  - Eular 회전은 Z -> X -> Y 순서로 이루어 진다.
		 *  - 회전 방향은 왼손 법칙을 따른다(??)
		 *  - Eular 회전 계산시, 회전의 기준 축은 World 좌표다.
		 */

        Quaternion q1 = Quaternion.LookRotation(view, Vector3.up);
        Quaternion q2 = Quaternion.Euler(relativeEulerRot);
        Quaternion q3 = q2 * q1;

        return (q3 * Vector3.forward);
    }

    static public Vector3 ClampViewAngle(float minAngle, float maxAngle, Vector3 p, Vector3 from, Vector3 to)
    {
        Vector3 fromView = from - p;
        Vector3 toView = to - p;

        float fromViewL = fromView.magnitude;
        //float toViewL = toView.magnitude;
        fromView.Normalize();
        toView.Normalize();

        Vector3 clampView = toView;


        float viewAngle = MathUtil.CalcAngle(fromView, toView, Vector3.up);

        float t = 1.0f;
        float clampAngle = viewAngle;

        //Note, 이것은 minAngle이 - 값이고 , maxAngle + 값이라는 것을 전제로 한다.
        if (viewAngle < minAngle)
        {
            t = minAngle / viewAngle;
            clampAngle = minAngle;
        }
        else if (viewAngle > maxAngle)
        {
            t = maxAngle / viewAngle;
            clampAngle = maxAngle;
        }

        clampView = Vector3.Slerp(fromView, toView, t);

        float l = fromViewL / Mathf.Cos(Mathf.Deg2Rad * clampAngle);

        Vector3 clampPoint = p + clampView * l;

        return clampPoint;
    }

    
    public Vector3 SpringDamp(Vector3 curPos, Vector3 targetPos, Vector3 prevTargetPos,
        float deltaTime, float springConst, float dampConst, float springLen)
    {
        Vector3 disp;
        Vector3 velocity;
        float forceMag;

        disp = curPos - targetPos;
        velocity = (prevTargetPos - targetPos) * deltaTime;

        float disp_len = disp.magnitude;

        if (Mathf.Abs(disp_len) < 0.0000001f)
            return curPos;

        forceMag = springConst * (springLen - disp_len)
            + dampConst * (Vector3.Dot(disp, velocity)) / disp_len;

        disp.Normalize();
        disp *= forceMag * deltaTime;

        return curPos += disp;
    }

    static public void UnitTest()
    {
        UnitTest_ClampViewAngle();
    }

    static public void UnitTest_ClampViewAngle()
    {
        Vector3 p = Vector3.zero;
        Vector3 from = new Vector3(0, 0, 10);
        Vector3 to = new Vector3(-10, 0, 10);

        Vector3 clampP = MathUtil.ClampViewAngle(-30, 30, p, from, to);

        Debug.Log("ClampViewAngleTest: " + clampP);
        Debug.Log(" 30 Deg, 10 Len :" + (10 * Mathf.Tan(Mathf.Deg2Rad * 30)));
    }

    static public void UnitTest_RotateView()
    {
        /*
		//for unit test
		//baseDir = m_characterState.platformForward;
		baseDir = new Vector3(0,0,-1);
		m_fireSpin.x = 45.0f;
		m_fireSpin.y = 45.0f;
		
		Quaternion q =  Quaternion.Euler(new Vector3(m_fireSpin.x , m_fireSpin.y, 0.0f));
		
		fireDir = q * baseDir;
		
		Debug.Log ("GetFireDirection: " + fireDir.ToString());
		
		Debug.Log ("Rotate Test1: "+ MathUtil.RotateView(new Vector3(0,0,1), new Vector3(45.0f, 45.0f, 45.0f)));
		Debug.Log ("Rotate Test2: "+ MathUtil.RotateView(new Vector3(0,0,-1), new Vector3(45.0f, 45.0f, 45.0f)));	
		*/
    }

    public static byte key = (byte)System.DateTime.Now.Second;

    /*
    public static double EncValue(double original)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte [] byteValue=System.BitConverter.GetBytes(original);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte) (byteValue[i] ^ mykey);

        return System.BitConverter.ToDouble(byteValue,0);
    }

    public static double DecValue(double encValue)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte[] byteValue = System.BitConverter.GetBytes(encValue);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte)(byteValue[i] ^ mykey);

        return System.BitConverter.ToDouble(byteValue, 0);
    }

    public static int EncValue(int original)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte[] byteValue = System.BitConverter.GetBytes(original);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte)(byteValue[i] ^ mykey);

        return System.BitConverter.ToInt32(byteValue, 0);
    }

    public static int DecValue(int encValue)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte[] byteValue = System.BitConverter.GetBytes(encValue);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte)(byteValue[i] ^ mykey);

        return System.BitConverter.ToInt32(byteValue, 0);
    }

    public static float EncValue(float original)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte[] byteValue = System.BitConverter.GetBytes(original);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte)(byteValue[i] ^ mykey);

        return System.BitConverter.ToSingle(byteValue, 0);
    }

    public static float DecValue(float encValue)
    {
        byte mykey = key;

        //return (double)(original ^ mykey);
        byte[] byteValue = System.BitConverter.GetBytes(encValue);

        for (int i = 0; i < byteValue.Length; i++)
            byteValue[i] = (byte)(byteValue[i] ^ mykey);

        return System.BitConverter.ToSingle(byteValue, 0);
    }
    */


    public static double EncValue(double original)
    {
        return original;
    }

    public static double DecValue(double encValue)
    {
        return encValue;
    }

    public static int EncValue(int original)
    {
        return original;
    }

    public static int DecValue(int encValue)
    {
        return encValue;
    }

    public static float EncValue(float original)
    {
        return original;
    }

    public static float DecValue(float encValue)
    {
        return encValue;
    }

    public static void EncValueTest()
    {
        Debug.Log("EncValueTest key:" + key);

        /*
        for (int i = 0; i < 10; i++)
        {
            double orinal = (double) Random.value;
            double enc = EncValue(orinal);
            double dec = EncValue(enc);

            Debug.Log(string.Format("EncValueTest orignal:{0}, enc:{1}, dec:{2}, ok:{3}", orinal, enc, dec, orinal == dec));

        }

        for (int i = 0; i < 10; i++)
        {
            int orinal = (int)Random.Range(-10000,10000);
           int enc = EncValue(orinal);
           int dec = EncValue(enc);

            Debug.Log(string.Format("EncValueTest orignal:{0}, enc:{1}, dec:{2}, ok:{3}", orinal, enc, dec, orinal == dec));

        }
         */


        Debug.LogError("UnitTest Begin3");

        for (int i = 0; i < 10; i++)
        {
            //CryptFloat.key = (byte)Random.Range(0, 255);
            CryptFloat MyValue = new CryptFloat();
            float origin = UnityEngine.Random.Range(-100000.0f, 10000.0f);

            MyValue.value = origin;
            float dec = MyValue.value;

            float a1 = MyValue.value - 2;
            float a2 = MyValue.value - 1;


            origin = MyValue.value;
            MyValue.value = MyValue.value + 1;
            MyValue.value = MyValue.value - 1;
            dec = MyValue.value;

            if (origin != dec)
            {
                Debug.LogError(string.Format("value error {0} , {1}, {2}", origin, dec, CryptFloat.key));
            }
        }

        Debug.LogError("UnitTest Completed");

    }

    //static MathUtil()
    //{
    //    EncValueTest();
    //}

    public static int SelProbaList(double[] probaList)
    {
        double totalProba = 0;
        double proba = 0;
        int sel = -1;

        foreach (double p in probaList)
            totalProba += p;

        double v = UnityEngine.Random.value * totalProba;

        for (sel = 0; sel < probaList.Length; sel++)
        {
            proba += probaList[sel];
            if (v <= proba)
                break;
        }

        return sel;
    }

    public static bool SelProbaYes(double proba)
    {
        double v = UnityEngine.Random.value;
        return v <= proba;
    }

    public static int SelIndex(int length)
    {
        return UnityEngine.Random.Range(0, length);
    }
}

public class CryptDouble
{
    public static byte key = (byte) System.DateTime.Now.Second; 

    private byte[] byteValue=null;

    public double value
    {
        get { return DecValue(byteValue); }
        set { byteValue = EncValue(value); }
    }

    byte[] EncValue(double original)
    {
        byte mykey = key;

        byte[] _byteValue = System.BitConverter.GetBytes(original);

        for (int i = 0; i < _byteValue.Length; i++)
            _byteValue[i] = (byte)(_byteValue[i] ^ mykey);

        return _byteValue;
    }

    double DecValue(byte[] _byteValue)
    {
        byte mykey = key;

        if (_byteValue == null)
            return 0.0;

        byte[] decBytes = (byte[])_byteValue.Clone();

        for (int i = 0; i < decBytes.Length; i++)
            decBytes[i] = (byte)(decBytes[i] ^ mykey);

        return System.BitConverter.ToDouble(decBytes, 0);
    }
}

public class CryptInt
{
    public static byte key = (byte) System.DateTime.Now.Second;

    private byte[] byteValue = null;

    public int value
    {
        get { return DecValue(byteValue); }
        set { byteValue = EncValue(value); }
    }

    static byte[] EncValue(int original)
    {
        byte mykey = key;

        byte[] _byteValue = System.BitConverter.GetBytes(original);

        for (int i = 0; i < _byteValue.Length; i++)
            _byteValue[i] = (byte)(_byteValue[i] ^ mykey);

        return _byteValue;
    }

    static int DecValue(byte[] _byteValue)
    {
        byte mykey = key;

        if (_byteValue == null)
            return 0;

        byte[] decBytes = (byte[])_byteValue.Clone();

        for (int i = 0; i < decBytes.Length; i++)
            decBytes[i] = (byte)(decBytes[i] ^ mykey);

        return System.BitConverter.ToInt32(decBytes, 0);
    }
}

public class CryptFloat
{
    public static byte key = (byte)System.DateTime.Now.Second;
    private byte[] byteValue = null;

    public float value
    {
        get { return DecValue(byteValue); }
        set { byteValue = EncValue(value); }
    }

    static byte[] EncValue(float original)
    {
        byte mykey = key;

        byte[] _byteValue = System.BitConverter.GetBytes(original);

        for (int i = 0; i < _byteValue.Length; i++)
            _byteValue[i] = (byte)(_byteValue[i] ^ mykey);

        return _byteValue;
    }

    static float DecValue(byte[] _byteValue)
    {
        byte mykey = key;

        if (_byteValue == null)
            return 0.0f;

        byte[] decBytes = (byte[]) _byteValue.Clone();

        for (int i = 0; i < decBytes.Length; i++)
            decBytes[i] = (byte)(decBytes[i] ^ mykey);

        return System.BitConverter.ToSingle(decBytes, 0);
    }

}
