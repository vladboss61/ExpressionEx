using System.Linq.Expressions;

namespace ExpressionEx;

public class User
{
    public class InnerEx
    {

    }

    public int Age { get; set; }
    public string Name { get; set; }
}

public static class ExpressionBuilder
{
    public static Func<T, bool> BuildPredicate<T>(string propertyName, string operation, object value)
    {
        var parameter = Expression.Parameter(typeof(T), "value");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(Convert.ChangeType(value, property.Type));

        // Build binary expression based on the operation
        Expression comparison = operation switch
        {
            "==" => Expression.Equal(property, constant),
            "!=" => Expression.NotEqual(property, constant),
            ">" => Expression.GreaterThan(property, constant),
            ">=" => Expression.GreaterThanOrEqual(property, constant),
            "<" => Expression.LessThan(property, constant),
            "<=" => Expression.LessThanOrEqual(property, constant),
            _ => throw new NotSupportedException($"Operation '{operation}' is not supported.")
        };

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return lambda.Compile();
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var ex1 = nameof(System.Type);
        var ex2 = nameof(User.Age);
        var ex3 = nameof(User.InnerEx);

        var compared = new User() { Age = 10, Name = "Vlad" };

        Func<User, bool> predicate1 = ExpressionBuilder.BuildPredicate<User>(nameof(compared.Age), "<", compared.Age);
        Func<User, bool> predicate2 = ExpressionBuilder.BuildPredicate<User>(nameof(compared.Name), "==", compared.Name);

        var users = new User[] { new User() { Age = 5, Name = "Den" }, new User { Age = 6, Name = "Vlad" }, new User { Age = 12, Name = "Jon" } };

        var filteredUsers1 = users.Where(predicate1).ToArray();

        foreach (var item in filteredUsers1)
        {
            Console.WriteLine($"Name: {item.Name} | Age: {item.Age}");
        }

        var filteredUsers2 = users.Where(predicate2).ToArray();
    }
}
