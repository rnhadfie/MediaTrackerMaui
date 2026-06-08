using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace MauiApp1.BackEnd.Shared
{
    // Small observable wrapper used across view models to provide a Value property
    // that raises PropertyChanged. Provides implicit conversions to/from T so existing
    // code can assign plain values when convenient.
    public class Observable<T> : INotifyPropertyChanged
    {
        private T _value = default!;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Observable()
        {
        }

        public Observable(T value)
        {
            _value = value!;
        }

        public T Value
        {
            get => _value!;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value!, value))
                {
                    _value = value!;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public override string? ToString()
        {
            return Value?.ToString();
        }

        public static implicit operator T?(Observable<T>? o)
        {
            if (o is null) return default;
            return o.Value;
        }

        public static implicit operator Observable<T>(T value)
        {
            return new Observable<T>(value);
        }
    }
}
