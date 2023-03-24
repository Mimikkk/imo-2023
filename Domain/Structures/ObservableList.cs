using System.Collections;

namespace Domain.Structures;

public sealed class ObservableList<T> : IList<T> {
  public ObservableList(Action<ObservableList<T>>? action = null) {
    if (action is not null) Changed += (_, _) => action.Invoke(this);
  }

  public static ObservableList<T> Create(Action<ObservableList<T>> action) => new(action);
  public static ObservableList<T> Create() => new();

  public void Notify() => Changed?.Invoke(this, EventArgs.Empty);

  public int Count => _list.Count;
  public bool IsReadOnly => false;

  public event EventHandler? Changed;
  public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void Add(T item) => _list.Add(item);
  public void Clear() => _list.Clear();
  public bool Contains(T item) => _list.Contains(item);
  public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
  public bool Remove(T item) => _list.Remove(item);
  public int IndexOf(T item) => _list.IndexOf(item);
  public void Insert(int index, T item) => _list.Insert(index, item);
  public void RemoveAt(int index) => _list.RemoveAt(index);

  public T this[int index] {
    get => _list[index];
    set => _list[index] = value;
  }

  private readonly List<T> _list = new();
}
