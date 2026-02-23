using GamePlay;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int index;
    public BoxCollider2D doorLeft, doorRight, doorUp, doorDown;
    public bool roomLeft, roomRight, roomUp, roomDown;
    public int stepToStart;
    public int doorNumber;
    public int width;
    public int height;
    public bool hasBeenSetup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorLeft.gameObject.SetActive(roomLeft);
        doorRight.gameObject.SetActive(roomRight);
        doorUp.gameObject.SetActive(roomUp);
        doorDown.gameObject.SetActive(roomDown);
    }

    private void Update()
    {
        if (roomUp)
        {
            doorUp.isTrigger = true;
            doorUp.enabled = (GameManager.Ist.curRoom != this);
        }
        if (roomDown)
        {
            doorDown.isTrigger = true;
            doorDown.enabled = (GameManager.Ist.curRoom != this);
        }
        if (roomLeft)
        {
            doorLeft.isTrigger = true;
            doorLeft.enabled = (GameManager.Ist.curRoom != this);
        }
        if (roomRight)
        {
            doorRight.isTrigger = true;
            doorRight.enabled = (GameManager.Ist.curRoom != this);
        }
    }

    // Update is called once per frame
    public void UpdateRoom(float xOffset, float yOffset)
    {
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));
        doorNumber = 0;
        if (roomUp) doorNumber++;
        if (roomDown) doorNumber++;
        if (roomLeft) doorNumber++;
        if (roomRight) doorNumber++;
    }
}
