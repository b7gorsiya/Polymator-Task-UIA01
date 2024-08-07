using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IMoveLogger
{
    void LogMove(int player, int cellIndex);
    void SaveMoves();
}
public class MoveLogger : IMoveLogger
{
    private Dictionary<string, string> moveLog = new Dictionary<string, string>();
    private int moveCount = 1;

    public void LogMove(int player, int cellIndex)
    {
        string row = (cellIndex / 3).ToString("0");
        string column = (cellIndex % 3).ToString("0");

        string moveKey = $"Move{moveCount}";
        string moveValue = $"P{player}-{row}{column}";

        moveLog[moveKey] = moveValue;
        moveCount++;
    }

    public void SaveMoves()
    {
        string json = JsonUtility.ToJson(new MoveDictionary(moveLog));
        File.WriteAllText(Application.persistentDataPath + "/moves.json", json);
    }
}
[System.Serializable]
public class MoveDictionary
{
    public List<MoveEntry> Entries;

    public MoveDictionary(Dictionary<string, string> dictionary)
    {
        Entries = new List<MoveEntry>();
        foreach (var kvp in dictionary)
        {
            Entries.Add(new MoveEntry(kvp.Key, kvp.Value));
        }
    }
}

[System.Serializable]
public class MoveEntry
{
    public string Key;
    public string Value;

    public MoveEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
