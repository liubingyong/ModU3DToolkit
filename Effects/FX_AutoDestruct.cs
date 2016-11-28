using UnityEngine;
using System.Collections;

public class FX_AutoDestruct : MonoBehaviour
{
    void Start ()
    {
        Destroy (this.gameObject, GetComponent<ParticleSystem> ().duration); 
    }
}
