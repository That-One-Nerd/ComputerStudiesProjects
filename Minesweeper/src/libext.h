#include <string.h>

template<typename T>
class vector
{
private:
    int _size;
    T* ptr;

public:
    vector()
    {
        _size = 0;
        ptr = new T[0];
    }
    ~vector()
    {
        delete[] ptr;
    }

    const T& at(int index) const
    {
        return ptr[index];
    }
    T& at(int index)
    {
        return ptr[index];
    }

    void add(T item)
    {
        T* new_ptr = new T[_size + 1];
        memcpy(new_ptr, ptr, _size * sizeof(T));
        new_ptr[_size] = item;

        delete[] ptr;
        ptr = new_ptr;
        _size++;
    }

    void remove_at(int index)
    {
        T* new_ptr = new T[_size - 1];
        memcpy(new_ptr, ptr, index * sizeof(T)); // Before
        memcpy(new_ptr + index, ptr + index + 1, (_size - index) * sizeof(T)); // After

        delete[] ptr;
        ptr = new_ptr;
        _size--;
    }


    int size() const
    {
        return _size;
    }

    const T& operator[](int index) const
    {
        return ptr[index];
    }
    T& operator[](int index)
    {
        return ptr[index];
    }
};
