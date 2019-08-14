using System;
using System.Diagnostics;

namespace Gumbo
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public class Attribute
    {
        public Element Parent { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string OriginalName { get; private set; }
        public string OriginalValue { get; private set; }
        public GumboSourcePosition NameStart { get; private set; }
        public GumboSourcePosition NameEnd { get; private set; }
        public GumboSourcePosition ValueStart { get; private set; }
        public GumboSourcePosition ValueEnd { get; private set; }
        public GumboAttributeNamespaceEnum Namespace { get; private set; }

        internal Attribute(GumboAttribute attribute, Element parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Name = NativeUtf8.StringFromNativeUtf8(attribute.name);
            Value = NativeUtf8.StringFromNativeUtf8(attribute.value);
            OriginalName = NativeUtf8.StringFromNativeUtf8(attribute.original_name.data, (int)attribute.original_name.length);
            OriginalValue = NativeUtf8.StringFromNativeUtf8(attribute.original_value.data, (int)attribute.original_value.length);
            NameStart = attribute.name_start;
            NameEnd = attribute.name_end;
            ValueStart = attribute.value_start;
            ValueEnd = attribute.value_end;
            Namespace = attribute.attr_namespace;
        }
    }
}
