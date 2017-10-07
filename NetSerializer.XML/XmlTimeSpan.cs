using System;
using System.Xml.Serialization;

namespace NetSerializer.XML
{
    public class XmlTimeSpan
    {
        private TimeSpan _value;

        public XmlTimeSpan()
        {
            _value = TimeSpan.Zero;
        }

        public XmlTimeSpan(TimeSpan source)
        {
            _value = source;
        }

        // public static implicit operator TimeSpan?(XmlTimeSpan o) => o?._value;

        // public static implicit operator XmlTimeSpan(TimeSpan? o) => o == null ? null : new XmlTimeSpan(o.Value);

        public static implicit operator TimeSpan(XmlTimeSpan o) => o?._value ?? default(TimeSpan);

        public static implicit operator XmlTimeSpan(TimeSpan o) => o == default(TimeSpan) ? null : new XmlTimeSpan(o);

        [XmlText]
        public string Default
        {
            get { return _value.ToString(); }
            set { _value = TimeSpan.Parse(value); }
        }
    }
}