﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgoMotionLib
{
    public class OrderedList<T, TKey> : List<(TKey key, T item)>
    {
        Func<T, TKey> _keySelector;
        Comparer<TKey> _comparer = Comparer<TKey>.Default;

        public OrderedList(Func<T, TKey> keySelector, IEnumerable<T> collection = null)
        {
            _keySelector = keySelector;

            if (collection != null)
                AddRange(collection.Select(o => (key: _keySelector(o), o)).OrderBy(_ => _.key));
        }

        public void AddForOrder(T item)
        {
            var key = _keySelector(item);

            for (var i = Count - 1; i >= 0; i--)
            {
                if (_comparer.Compare(this[i].key, key) <= 0)
                {
                    Insert(i + 1, (key, item));
                    return;
                }
            }
            Insert(0, (key, item));
        }
    }
}
