using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole;

internal class RecipeTypeConverter : TypeConverter
{
	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object value, Type destinationType)
	{
		var casted = value as Recipe;
		return casted.Title;
	}
}
