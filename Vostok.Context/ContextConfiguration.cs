namespace Vostok.Context
{
    internal class ContextConfiguration : IContextConfiguration
    {
        public IContextErrorListener ErrorListener { get; set; }

        public void RegisterDistributedProperty<T>(string name, IContextSerializer<T> serializer)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterDistributedGlobal<T>(string name, IContextSerializer<T> serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}
