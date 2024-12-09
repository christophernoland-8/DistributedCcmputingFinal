using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    public List<string> firstNames = new List<string>() { "Alex", "John", "Mary", "Bob" };

    public List<string> lastNames = new List<string>() { "Smith", "Jones", "Brown", "Miller" };



    public static string playerName; // Variable to store the generated name

    public void Start()
    {
        int firstNameIndex = Random.Range(0, firstNames.Count);

        int lastNameIndex = Random.Range(0, lastNames.Count);

        playerName = firstNames[firstNameIndex] + " " + lastNames[lastNameIndex];
    }

}
