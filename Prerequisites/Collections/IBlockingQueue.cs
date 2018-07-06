namespace Collections
{
    public interface IBlockingQueue<T>
    {

        void enqueue(T item);
        T dequeue();

        int size { get; }
        bool isEmpty();

        void clear();

        void pulse();
        void pulseAll();

    }
}