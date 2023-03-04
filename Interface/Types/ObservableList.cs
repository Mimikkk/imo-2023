using System;
using System.Collections;
using System.Collections.Generic;

namespace Interface.Types;

public class ObservableList<T> : IList<T> {
  private List<T> List { get; } = new();
  public event EventHandler? Changed;

  public IEnumerator<T> GetEnumerator() => List.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void Add(T item) {
    List.Add(item);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public void Clear() {
    List.Clear();
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public bool Contains(T item) => List.Contains(item);
  public void CopyTo(T[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);
  public bool Remove(T item) {
    var result = List.Remove(item);
    Changed?.Invoke(this, EventArgs.Empty);
    return result;
  }
  public int Count => List.Count;
  public bool IsReadOnly => false;
  public int IndexOf(T item) {
    return List.IndexOf(item);
  }
  public void Insert(int index, T item) {
    List.Insert(index, item);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public void RemoveAt(int index) {
    List.RemoveAt(index);
    Changed?.Invoke(this, EventArgs.Empty);
  }
  public T this[int index] {
    get => List[index];
    set => List[index] = value;
  }
}
