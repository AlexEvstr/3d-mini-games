using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotValue
{
    Bar,
    Cherry,
    Diamond,
    Ring,
    Seven
}
public class Slot : MonoBehaviour
{
    public SlotValue randItem;
    private float speed;
    public SlotValue stoppedSlot;
    private SlotMachine sm;

    private void Start()
    {
        sm = gameObject.GetComponentInParent<SlotMachine>();
    }
    public IEnumerator Spin()
    {
        sm.DisableSPinBtn();
        speed = Random.Range(1, 4);
        while(speed >= 0.1f)
        {
            speed -= 0.1f;
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            if (transform.localPosition.y < -15f)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, -5f);
            }
            yield return new WaitForSeconds(sm.timeInterval);
            CheckPosition();
        }

        CheckResults();
        yield return null;
    }

    private void CheckPosition()
    {
        if (transform.localPosition.y < -13.0f && transform.localPosition.y > -15.0f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, -15.0f);
        }
        else if (transform.localPosition.y < -11.0f && transform.localPosition.y > -13.0f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, -13.0f);
        }
        else if (transform.localPosition.y < -9.0f && transform.localPosition.y > -11.0f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, -11.0f);
        }
        else if (transform.localPosition.y < -7.0f && transform.localPosition.y > -9.0f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, -9.0f);
        }
        else if (transform.localPosition.y < -5.0f && transform.localPosition.y > -7.0f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, -7.0f);
        }
    }

    private void CheckResults()
    {
        if (transform.localPosition.y == -7f)
        {
            stoppedSlot = SlotValue.Bar;
        }
        else if (transform.localPosition.y == -5f || transform.localPosition.y == -15.0f)
        {
            stoppedSlot = SlotValue.Cherry;
        }
        else if (transform.localPosition.y == -13f)
        {
            stoppedSlot = SlotValue.Diamond;
        }
        else if (transform.localPosition.y == -11f)
        {
            stoppedSlot = SlotValue.Ring;
        }
        else if (transform.localPosition.y == -9f)
        {
            stoppedSlot = SlotValue.Seven;
        }

        sm.WaitResults();
    }
}

