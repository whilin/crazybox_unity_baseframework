using UnityEngine;
using System.Collections;

public class CarExplodeFx : MonoBehaviour
{
    [System.Serializable]
    public class ProceduralEffect
    {
        public float delay;

        public GameObject partObject;
        /*라이버러리 작업 */
       // public FracturedObject fracturedObject;
        public Vector3 explodeDirection;

        public Transform effectPoint;
        public OneShotEffect prefabEffect;
        public float duration;
    }

    public class ExplodeParam
    {
        public Vector3 impactPos;
        public Vector3 impactPower;
    }

    public ProceduralEffect[] pase;

    ExplodeParam m_exParam;

    public static void Explode(Vector3 pos, Quaternion rot, Vector3 impactPos, Vector3 impactPower)
    {
        
    }
    
    // Use this for initialization
    void Start()
    {
        
    }

    IEnumerator DoProceduralDestroyFx(ProceduralEffect e)
    {
        yield return new WaitForSeconds(e.delay);

    /*라이버러리 작업 */
        // if (e.fracturedObject != null)
        // {
        //      e.fracturedObject.Explode(m_exParam.impactPos, m_exParam.impactPower.magnitude);
        // }

        if(e.partObject !=null)
        {
            e.partObject.transform.SetParent(null);
            var rigid= e.partObject.GetComponent<Rigidbody>();
            rigid.AddExplosionForce(m_exParam.impactPower.magnitude, m_exParam.impactPos, 1.0f);
           // rigid.AddForceAtPosition();
        }

        if (e.prefabEffect != null)
        {
            GameObject effect = GameObjectPoolManager.Spawn(e.prefabEffect.gameObject, e.effectPoint.position, Quaternion.LookRotation(Vector3.up));
            effect.transform.SetParent(e.effectPoint);
        }

        yield return new WaitForSeconds(e.duration);
    }

}

