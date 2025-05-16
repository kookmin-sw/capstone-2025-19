using ExitGames.Client.Photon;
using Photon.Pun;
using System.IO;

public static class PhotomCustomTypes
{
    // Start is called before the first frame update
    public static void Register()
    {
        //PhotonPeer.RegisterType(typeof(TestID), (byte)'T', SerializeTestID, DeserializeTestID);
        PhotonPeer.RegisterType(typeof(Item), (byte)'I', SerializeItem, DeserializeItem);
    }

    /*public static short SerializeTestID(StreamBuffer outStream, object customObject)
    {
        TestID testID = (TestID)customObject;
        byte[] bytes;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(testID.ID); // TestID 안의 int 값 저장
            }
            bytes = memoryStream.ToArray();
        }
        outStream.Write(bytes, 0, bytes.Length);
        return (short)bytes.Length;
    }

    private static object DeserializeTestID(StreamBuffer inStream, short length)
    {
        byte[] bytes = new byte[length];
        inStream.Read(bytes, 0, length);

        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                int id = reader.ReadInt32(); // 저장한 int 값을 다시 읽음
                return new TestID(id);
            }
        }
    }*/

    public static short SerializeItem(StreamBuffer outStream, object customObject)
    {
        Item item = (Item)customObject;
        byte[] bytes;

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(item.itemData.name); // ItemData를 string으로 저장
                writer.Write(item.quantity);
                writer.Write(item.durability);
            }
            bytes = memoryStream.ToArray();
        }

        outStream.Write(bytes, 0, bytes.Length);
        return (short)bytes.Length;
    }
    public static object DeserializeItem(StreamBuffer inStream, short length)
    {
        byte[] bytes = new byte[length];
        inStream.Read(bytes, 0, length);

        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                string itemName = reader.ReadString();
                int quantity = reader.ReadInt32();
                float durability = reader.ReadSingle();

                ItemData itemData = ItemDatabase.Instance.GetItemDataByName(itemName);
                return new Item(itemData, quantity, durability);
            }
        }

    }
    
}
