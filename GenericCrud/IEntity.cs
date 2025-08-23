namespace GenericCrud
{
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
