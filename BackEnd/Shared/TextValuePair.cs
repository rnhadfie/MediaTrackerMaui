using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Shared
{
    public class TextValuePair<T1, T2>
    {
        // preserve original field names used across codebase
        private T1 _text { get; set; }
        private T2 _value { get; set; }

        // expose property names the DataGrid control expects
        public T1 Text
        {
            get => _text;
            set => _text = value;
        }

        public T2 Value
        {
            get => _value;
            set => _value = value;
        }

        public TextValuePair(T1 text, T2 value)
        {
            _text = text;
            _value = value;
        }
    }
}
