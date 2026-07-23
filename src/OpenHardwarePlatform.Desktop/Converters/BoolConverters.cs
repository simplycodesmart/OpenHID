using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace OpenHardwarePlatform.Desktop.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var on = parameter?.ToString()?.Split('|')[0] ?? "#6BFF8F";
        var off = parameter?.ToString()?.Split('|')[1] ?? "#FFB4AB";
        return Avalonia.Media.Brush.Parse((value is true ? on : off));
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class IsNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value == null;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => !string.IsNullOrEmpty(value?.ToString());
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b ? !b : false;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b ? !b : false;
}

public class IsNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => !string.IsNullOrEmpty(value?.ToString());
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public static class ObjectConverters
{
    public static readonly IsNotNullConverter IsNotNull = new();
    public static readonly IsNullConverter IsNull = new();
    public static readonly IsNotNullOrEmptyConverter IsNotNullOrEmpty = new();
    public static readonly InvertBoolConverter InvertBool = new();
    public static readonly BoolToBrushConverter ToBrush = new();
}

public static class StringConverters
{
    public static readonly IsNotNullOrEmptyConverter IsNotNullOrEmpty = new();
    public static readonly IsNotEmptyConverter IsNotEmpty = new();
}
