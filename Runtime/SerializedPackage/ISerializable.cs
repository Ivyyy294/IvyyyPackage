namespace Ivyyy
{
	public interface ISerializable
	{
		public string GetSerializedDataAsString();
		public byte[] GetSerializedData();
		public bool DeserializeData(string data);
		public bool DeserializeData(byte[] bytes);
	}
}
