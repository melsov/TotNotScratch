using System.Collections.Generic;
using System.Linq;

public class RingBuffer<T>
{
    private T[] storage;
    public int size {
        get {
            return storage.Length;
        }
    }

    private int cursor;
    private int first;

    public RingBuffer(int size) {
        storage = new T[size];
        first = 1;
    }

    public void push(T t) {
        storage[cursor] = t;
        cursor = (cursor + 1) % size;
        first = (cursor + 1) % size;
    }

    public T pop() {
        T result = storage[cursor];
        if(cursor == 0) {
            cursor = size - 1;
        } else {
            cursor -= 1;
        }
        first = (cursor + 1) % size;
        return result;
    }

    public IEnumerable<T> getValues() {
        foreach(int i in Enumerable.Range(0, size)) {
            yield return this[i];
        }
    }

    public T this[int i] {
        get {
            return storage[(first + i) % size];
        }
    }
}

