using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveScene(PlayerMovement player, EnterBattle enemy)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPath();

        FileStream stream = new FileStream(path, FileMode.Create);

        UnitData data = new UnitData(player, enemy);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerPosition(PlayerMovement player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPath();

        FileStream stream = new FileStream(path, FileMode.Create);

        UnitData data = new UnitData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static UnitData LoadScene()
    {
        string path = GetPath();
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        UnitData data = formatter.Deserialize(stream) as UnitData;

        stream.Close();
        return data;
    }

    static string GetPath()
    {
        string path = Application.persistentDataPath + "/player.data";
        return path;
    }
}
