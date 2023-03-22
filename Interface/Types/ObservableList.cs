using System;
using System.Collections;
using System.Collections.Generic;

namespace Interface.Types;

public sealed class ObservableList<T> : IList<T> {
  public ObservableList(Action<ObservableList<T>> action) => Changed += (_, _) => action.Invoke(this);

  private readonly List<T> _list = new();
  public event EventHandler? Changed;

  public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void Add(T item) {
    _list.Add(item);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public void Clear() {
    _list.Clear();
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public bool Contains(T item) => _list.Contains(item);
  public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
  public bool Remove(T item) {
    var result = _list.Remove(item);
    Changed?.Invoke(this, EventArgs.Empty);
    return result;
  }
  public int Count => _list.Count;
  public bool IsReadOnly => false;
  public int IndexOf(T item) {
    return _list.IndexOf(item);
  }
  public void Insert(int index, T item) {
    _list.Insert(index, item);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public void RemoveAt(int index) {
    _list.RemoveAt(index);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public T this[int index] {
    get => _list[index];
    set {
      _list[index] = value;
      Changed?.Invoke(this, EventArgs.Empty);
    }
  }
}
