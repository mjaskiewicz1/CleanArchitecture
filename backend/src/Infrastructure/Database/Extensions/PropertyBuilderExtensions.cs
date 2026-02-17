using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Extensions;

public static class PropertyBuilderExtensions
{
    extension<TEnum>(PropertyBuilder<TEnum> builder)
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Extension method to set the maximum length of a property based on the lengths of enum names.
        /// </summary>
        public void HasMaxLengthFromEnum()
        {
            var maxLength = Enum.GetNames<TEnum>().Max(n => n.Length);
            builder.HasMaxLength(maxLength);
            builder.HasConversion<string>();
        }
    }
}