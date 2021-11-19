// Decompiled with JetBrains decompiler
// Type: _3DTools.Matrix3DStack
// Assembly: 3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null
// MVID: A40A7738-C81D-401C-894B-697A2262AE5F
// Assembly location: D:\C#\WPF\test\test\3DTools.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace _3DTools
{
  public class Matrix3DStack : IEnumerable<Matrix3D>, ICollection, IEnumerable
  {
    private readonly List<Matrix3D> _storage = new List<Matrix3D>();

    public Matrix3D Peek() => this._storage[this._storage.Count - 1];

    public void Push(Matrix3D item) => this._storage.Add(item);

    public void Append(Matrix3D item)
    {
      if (this.Count > 0)
      {
        Matrix3D matrix3D = this.Peek();
        matrix3D.Append(item);
        this.Push(matrix3D);
      }
      else
        this.Push(item);
    }

    public void Prepend(Matrix3D item)
    {
      if (this.Count > 0)
      {
        Matrix3D matrix3D = this.Peek();
        matrix3D.Prepend(item);
        this.Push(matrix3D);
      }
      else
        this.Push(item);
    }

    public Matrix3D Pop()
    {
      Matrix3D matrix3D = this.Peek();
      this._storage.RemoveAt(this._storage.Count - 1);
      return matrix3D;
    }

    public int Count => this._storage.Count;

    private void Clear() => this._storage.Clear();

    private bool Contains(Matrix3D item) => this._storage.Contains(item);

    void ICollection.CopyTo(Array array, int index) => ((ICollection) this._storage).CopyTo(array, index);

    bool ICollection.IsSynchronized => ((ICollection) this._storage).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection) this._storage).SyncRoot;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) ((IEnumerable<Matrix3D>) this).GetEnumerator();

    IEnumerator<Matrix3D> IEnumerable<Matrix3D>.GetEnumerator()
    {
      for (int i = this._storage.Count - 1; i >= 0; --i)
        yield return this._storage[i];
    }
  }
}
