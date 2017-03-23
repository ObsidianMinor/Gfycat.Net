namespace Gfycat
{
    /// <summary>
    /// Exposes the GfycatClient to objects
    /// </summary>
    public abstract class Entity
    {
        protected internal GfycatClient Client { get; }

        public string Id { get; internal set; }

        internal Entity(GfycatClient client, string id)
        {
            Client = client;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Entity entity))
                return false;

            return entity.Id == Id;
        }

        public override int GetHashCode()
        {
            int returnValue = 0;
            string tempId = Id;
            for (int i = 1; i < Id.Length / 2; i++)
                tempId = tempId.Insert(i * 2, " ");
            foreach (string byteString in tempId.Split(' '))
                returnValue += System.Convert.ToSByte(byteString, 16);
            return returnValue;
        }
    }
}
