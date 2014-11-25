namespace CSharpArgs
{
    public class Iterator<T>
    {
        private readonly T[] array;
        private int index = -1;

        public Iterator(T[] array)
        {
            this.array = array;
        }

        public bool HasNext()
        {
            return (index + 1 < array.Length);
        }

        public T Next()
        {
            if (!HasNext())
                throw new NoSuchElementException();

            return array[++index];
        }

        public T Previous()
        {
            return array[--index];
        }

        public int NextIndex()
        {
            return index + 1;
        }
    }
}