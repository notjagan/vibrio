using System.ComponentModel;
using System.Globalization;

namespace vibrio.src.Utilities {
    public class ModWrapperConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
            if (value is string acronym) {
                return new ModWrapper(acronym);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
