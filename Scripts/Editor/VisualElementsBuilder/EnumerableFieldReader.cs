using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder.Editor
{
    // Generated code by AI
    public sealed class EnumerableFieldReader : IBaseFieldReader
    {
        private sealed class ItemCtx
        {
            public IBaseFieldReader Reader;
            public Action<object> OnChanged;
        }

        public event Action OnValueChangedAfterBlur;
        public event Action OnValueChange;

        public bool TwoWay => false;
        public object Value => GetValue();
        public BindableElement VisualElement => _listView;

        private readonly ListView _listView;
        private readonly IList _data;
        private readonly Type _elemType;
        private readonly Type _declaredEnumerableType;
        private readonly Func<Type, IBaseFieldReader> _createBaseField;

        private bool _dirty;
        private IVisualElementScheduledItem _pollHandle;

        public EnumerableFieldReader(
            ListView listView,
            IList data,
            Type elemType,
            Type declaredEnumerableType,
            Func<Type, IBaseFieldReader> createBaseField)
        {
            _listView = listView;
            _data = data;
            _elemType = elemType;
            _declaredEnumerableType = declaredEnumerableType;
            _createBaseField = createBaseField;

            // Bubble child readers' changes
            _listView.itemIndexChanged += (_, __) => MarkDirty();
            _listView.itemsAdded += OnItemsAdded;
            _listView.itemsRemoved += _ => MarkDirty();

            // Catch focus leaving the list => AfterBlur
            _listView.RegisterCallback<FocusOutEvent>(_ =>
            {
                if (_dirty) { _dirty = false; OnValueChangedAfterBlur?.Invoke(); }
            });

            // Hook to child readers as they are created
            _listView.makeItem = () =>
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };

                var child = _createBaseField(_elemType);

                // per-row context so we can reassign handlers on bind without duplicating subscriptions
                var ctx = new ItemCtx { Reader = child };
                row.userData = ctx;

                // bubble child events once (handlers are redirected by ctx)
                child.OnValueChange += () =>
                {
                    ctx.OnChanged?.Invoke(child.Value); // updates backing and marks dirty
                    MarkDirty();
                };
                child.OnValueChangedAfterBlur += () =>
                {
                    if (_dirty) { _dirty = false; OnValueChangedAfterBlur?.Invoke(); }
                };

                row.Add(child.VisualElement);
                return row;
            };


            _listView.bindItem = (row, idx) =>
            {
                var ctx = (ItemCtx)row.userData;
                var reader = ctx.Reader;

                reader.SetLabel($"[{idx}]");

                // ensure slot exists
                while (_data.Count <= idx) _data.Add(Default(_elemType));
                reader.SetValue(_data[idx]);

                // redirect change target for this realized row/index
                ctx.OnChanged = v => { _data[idx] = v; };
            };
        }

        public void SetValue(object value)
        {
            _data.Clear();
            if (value is IEnumerable src && value is not string)
            {
                foreach (var it in src) _data.Add(CastOrDefault(it, _elemType));
            }
            _listView.Rebuild();
            MarkDirty(invokeAfterBlur: false); // reflect programmatic change
        }

        public void SetLabel(string label)
        {
            _listView.headerTitle = label;
        }

        public void Bind(object target, PropertyInfo propertyInfo)
        {
            SetValue(SafeGet(() => propertyInfo.GetValue(target)));
            // Live write-back on any change
            OnValueChange += () => SafeSet(() => propertyInfo.SetValue(target, GetValueShaped()));
            // Also commit once after blur (useful for batch edits)
            OnValueChangedAfterBlur += () => SafeSet(() => propertyInfo.SetValue(target, GetValueShaped()));
        }

        public void Bind(object target, FieldInfo fieldInfo)
        {
            SetValue(SafeGet(() => fieldInfo.GetValue(target)));
            OnValueChange += () => SafeSet(() => fieldInfo.SetValue(target, GetValueShaped()));
            OnValueChangedAfterBlur += () => SafeSet(() => fieldInfo.SetValue(target, GetValueShaped()));
        }

        public void CreateGetSet(Func<object> get, Action<object> set)
        {
            SetValue(SafeGet(get));

            void Push() => SafeSet(() => set(GetValueShaped()));

            OnValueChange += Push;
            OnValueChangedAfterBlur += Push;

            // Lightweight pull-sync (optional, keeps UI in sync if source mutates elsewhere)
            _pollHandle = _listView.schedule.Execute(() =>
            {
                var current = SafeGet(get);
                if (!SequenceEqual(current as IEnumerable, _data)) SetValue(current);
            }).Every(250);
        }

        public void Dispose()
        {
            _listView.itemIndexChanged -= (_, __) => MarkDirty();
            _listView.itemsAdded -= OnItemsAdded;
            _listView.itemsRemoved -= _ => MarkDirty();
            _listView.UnregisterCallback<FocusOutEvent>(_ => { });

            _pollHandle?.Pause();
            _pollHandle = null;

            // Best-effort: dispose any realized child readers
            foreach (var ve in _listView.Children())
            {
                if (ve.userData is IBaseFieldReader r) r.Dispose();
            }
        }

        // ---- internals ----

        private void OnItemsAdded(IEnumerable<int> indices)
        {
            foreach (var i in indices)
            {
                // Ensure a non-null default slot
                if (i >= 0 && i < _data.Count && _data[i] == null)
                    _data[i] = Default(_elemType);
            }
            MarkDirty();
        }

        private void MarkDirty() => MarkDirty(true);
        private void MarkDirty(bool invokeAfterBlur) => MarkDirty(invokeAfterBlur, true);

        private void MarkDirty(bool invokeAfterBlur, bool notifyChange)
        {
            _dirty = true;
            if (notifyChange) OnValueChange?.Invoke();
            if (invokeAfterBlur == false) return;
        }

        private object GetValue() => GetValueShaped();

        private object GetValueShaped()
        {
            // Return the same "shape" as declared type
            if (_declaredEnumerableType.IsArray)
            {
                var arr = Array.CreateInstance(_elemType, _data.Count);
                _data.CopyTo(arr, 0);
                return arr;
            }

            if (typeof(IList).IsAssignableFrom(_declaredEnumerableType) && !_declaredEnumerableType.IsInterface && !_declaredEnumerableType.IsAbstract)
            {
                var inst = (IList)Activator.CreateInstance(_declaredEnumerableType);
                foreach (var it in _data) inst.Add(it);
                return inst;
            }

            // Fallback: List<T>
            var listType = typeof(List<>).MakeGenericType(_elemType);
            var list = (IList)Activator.CreateInstance(listType);
            foreach (var it in _data) list.Add(it);
            return list;
        }

        private static object CastOrDefault(object value, Type targetType)
        {
            if (value == null) return Default(targetType);
            if (targetType.IsInstanceOfType(value)) return value;

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return Default(targetType);
            }
        }

        private static object Default(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        private static T SafeGet<T>(Func<T> f)
        {
            try { return f(); } catch { return default; }
        }
        private static void SafeSet(Action a)
        {
            try { a(); } catch { /* swallow to keep UI responsive */ }
        }

        private static bool SequenceEqual(IEnumerable a, IEnumerable b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;

            var ea = a.Cast<object>();
            var eb = b.Cast<object>();

            // Compare counts quickly if possible
            if (a is ICollection ca && b is ICollection cb && ca.Count != cb.Count) return false;

            using var ia = ea.GetEnumerator();
            using var ib = eb.GetEnumerator();
            while (true)
            {
                var ma = ia.MoveNext();
                var mb = ib.MoveNext();
                if (ma != mb) return false;
                if (!ma) return true;
                if (!Equals(ia.Current, ib.Current)) return false;
            }
        }
    }
    public static class EnumerableFieldFactory
    {
        public static bool IsEnumerableButNotString(Type t) =>
            t != typeof(string) && typeof(IEnumerable).IsAssignableFrom(t);

        public static Type GetEnumerableElementType(Type t)
        {
            if (t.IsArray) return t.GetElementType();

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return t.GetGenericArguments()[0];

            var ienum = t.GetInterfaces().FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return ienum?.GetGenericArguments()[0] ?? typeof(object);
        }

        public static IBaseFieldReader CreateEnumerableField(Type declaredEnumerableType, Func<Type, IBaseFieldReader> createBaseField)
        {
            var elementType = GetEnumerableElementType(declaredEnumerableType);
            var listType = typeof(List<>).MakeGenericType(elementType);
            var backing = (IList)Activator.CreateInstance(listType);
            var listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderable = true,
                showAddRemoveFooter = true,
                selectionType = SelectionType.None,
                headerTitle = $"{elementType.Name.GetNiceString()} List",
                style = { flexGrow = 1 }
            };

            // Per-item editors reuse the existing factory.
            listView.makeItem = () =>
            {
                var reader = createBaseField(elementType);
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };
                row.userData = reader;
                row.Add(reader.VisualElement);
                return row;
            };

            listView.bindItem = (ve, idx) =>
            {
                var reader = (IBaseFieldReader)ve.userData;
                reader.SetLabel($"[{idx}]");
                reader.SetValue(idx >= 0 && idx < backing.Count ? backing[idx] : Default(elementType));
            };

            listView.itemsSource = backing;
            return new EnumerableFieldReader(listView, backing, elementType, declaredEnumerableType, createBaseField);
        }

        private static object Default(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}