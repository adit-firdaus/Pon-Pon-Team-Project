using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static Room activeRoom;

    public List<Door> doors = new();

    public bool isPlaying => activeRoom == this;

    public void Play()
    {
        activeRoom = this;
    }


}

public class Door
{

}