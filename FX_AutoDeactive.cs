using UnityEngine;
using System.Collections;
using PathologicalGames;

public class FX_AutoDeactive : MonoBehaviour
{
    public bool deactiveByPool;
    public string poolName;

    void OnEnable ()
    {
        StartCoroutine(DeactiveOnComplete());
    }

    IEnumerator DeactiveOnComplete()
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().duration);

        if (deactiveByPool)
        {
            PoolManager.Pools[poolName].Despawn(this.transform);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
