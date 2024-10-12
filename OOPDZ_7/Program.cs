//Разработайте атрибут позволяющий методу ObjectToString сохранять поля
// классов с использованием произвольного имени.
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class CustomNameAttribute : Attribute
{
    public string Name { get; }

    public CustomNameAttribute(string name)
    {
        Name = name;
    }
}

public class Example
{
    [CustomName("CustomFieldName")]
    public int I = 0;
}

public static class ObjectConverter
{
    // Преобразование объекта в строку с учетом атрибута CustomName
    public static string ObjectToString(object obj)
    {
        Type type = obj.GetType();
        var fields = type.GetFields();

        string result = "";
        foreach (var field in fields)
        {
            // Проверяем наличие атрибута CustomName
            var customNameAttr = field.GetCustomAttribute<CustomNameAttribute>();
            string fieldName = customNameAttr != null ? customNameAttr.Name : field.Name;

            object value = field.GetValue(obj);
            result += $"{fieldName}:{value}\n";
        }

        return result.Trim();
    }

    // Преобразование строки обратно в объект с учетом атрибута CustomName
    public static void StringToObject(string data, object obj)
    {
        Type type = obj.GetType();
        var fields = type.GetFields();
        var lines = data.Split('\n');

        foreach (var line in lines)
        {
            var keyValue = line.Split(':');
            string propertyName = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            // Поиск поля по имени или по атрибуту CustomName
            var field = fields.FirstOrDefault(f =>
            {
                var customNameAttr = f.GetCustomAttribute<CustomNameAttribute>();
                return (customNameAttr != null && customNameAttr.Name == propertyName) || f.Name == propertyName;
            });

            if (field != null)
            {
                // Преобразуем строковое значение в тип поля
                object convertedValue = Convert.ChangeType(value, field.FieldType);
                field.SetValue(obj, convertedValue);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var example = new Example();

        // Преобразование объекта в строку
        string data = ObjectConverter.ObjectToString(example);
        Console.WriteLine("Object to string:");
        Console.WriteLine(data);

        // Изменяем объект обратно из строки
        var newExample = new Example();
        ObjectConverter.StringToObject(data, newExample);
        Console.WriteLine("\nAfter StringToObject:");
        Console.WriteLine($"I: {newExample.I}");
    }
}