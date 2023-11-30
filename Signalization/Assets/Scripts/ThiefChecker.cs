using UnityEngine;

[RequireComponent(typeof(Signalization))]
public class ThiefChecker : MonoBehaviour
{
    private Signalization _signalization;

    private void Awake()
    {
        _signalization = GetComponent<Signalization>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ThiefMovement>(out ThiefMovement thiefScript))
        {
            _signalization.ChangeAlarmState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ThiefMovement>(out ThiefMovement thiefScript))
        {
            _signalization.ChangeAlarmState(false);
        }
    }
}
