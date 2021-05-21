using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location Name", menuName = "ScriptableObject/Text/LocationName", order = 1)]
public class LocationName : ScriptableObject
{
    [SerializeField]
    private string[] locationZero = new string[]
    {
        "Zone 1:"
    };

    [SerializeField]
    private string[] locationFirst = new string[] {
        ""
    };

    [SerializeField]
    private string[] locationSecond = new string[]
    {
        ""
    };

    [SerializeField]
    private string[] locationThird = new string[]
    {
        ""
    };

    public virtual string GetRandomName()
    {
        string randomZero = locationZero[Random.Range(0, locationZero.Length)];
        string randomFirst = locationFirst[Random.Range(0, locationFirst.Length)];
        string randomSecond = locationSecond[Random.Range(0, locationSecond.Length)];
        string randomThird = locationThird[Random.Range(0, locationThird.Length)];
        return $"{randomZero} {randomFirst} {randomSecond} {randomThird}";
    }
}
