using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;

public class Symbol : MonoBehaviour
{
    private Reel _reel;

    public Reel reel {
        get {
            return _reel;
        }
        set {
            _reel = value;
            this.transform.SetParent (_reel.transform);
        }
    }

    public string symbolType;

    [SerializeField]
    private UnityEvent<GameObject> _onRolled = new ConcreteUnityEvent<GameObject> ();

    public UnityEvent<GameObject> OnRolled {
        get { return _onRolled; }
        set { _onRolled = value; }
    }

    void OnDisabled ()
    {
        OnRolled.RemoveAllListeners ();
    }

    void FixedUpdate ()
    {
        if (reel.inSpin) {
            var positionY = this.transform.localPosition.y;
            var targetPositionY = positionY - reel.currentSpeed * Time.fixedDeltaTime;

            if (targetPositionY <= reel.rowsCount * -reel.rowHeight) {
                this.transform.localPosition = new Vector3 (this.transform.localPosition.x, targetPositionY + (reel.rowsCount + 1) * reel.rowHeight, this.transform.localPosition.z);
                OnRolled.Invoke (this.gameObject);
            } else {
                this.transform.localPosition = new Vector3 (this.transform.localPosition.x, targetPositionY, this.transform.localPosition.z);
            }
        }
    }

    // Move To Order Position
    public void MoveTo (int seq)
    {
        this.transform.localPosition = new Vector3 (this.transform.localPosition.x, -seq * reel.rowHeight, this.transform.localPosition.z);
    }
}
