// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fuke.Common.Utilities;

internal class StronglyConnectedComponent<T> : IEnumerable<Vertex<T>>
{
    private readonly LinkedList<Vertex<T>> _list;

    public StronglyConnectedComponent()
    {
        _list = new LinkedList<Vertex<T>>();
    }

    public void Add(Vertex<T> vertex)
    {
        _list.AddLast(vertex);
    }

    public int Count => _list.Count;

    public bool IsCycle => _list.Count > 1;

    public IEnumerator<Vertex<T>> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
