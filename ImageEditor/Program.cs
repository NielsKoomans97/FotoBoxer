using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("FotoEditor ~>");

            var command = Console.ReadLine();
            var parts = command?.Split("\\s").ParseArgs();

            if (parts == null)
            {
                Console.WriteLine("Het opgegeven commando was op een incorrecte wijze ingevuld. Controleer a.u.b. of er geen extra spaties in staan.");
                return;
            }

            switch (parts.Command)
            {
                case "box":
                    var input = parts.Parameters?["input"]
                        .CheckNullOrEmpty();
                    var output = parts.Parameters?["output"]
                        .CheckNullOrEmpty();
                    var barColor = parts.Parameters?["barColor"]
                        .CheckNullOrEmpty();

                    // gecheckt voor parameters
                    // toevoegen code voor het 'inboxen' van fotos die ongelijke groottes/breedtes hebben

                    break;

                case "resize": break;
                case "changebg": break;
                case "changefg": break;
                case "copy": break;
                case "rename": break;
            }
        }
    }
}

public static class ConsoleExtensions
{
    public static CommandArgs ParseArgs(this string[] args)
    {
        var commandArgs = new CommandArgs(args[0]);

        Array.ForEach(args[1..args.Length], a =>
        {
            if (a.Contains('='))
            {
                var split = a.Split('=');
                commandArgs.Parameters?.Add(split[0], split[1]);
            }
            if (!a.Contains('='))
            {
                commandArgs.Switches?.Add(a);
            }
        });

        return commandArgs;
    }

    public static KeyValuePair<string, object> CheckNullOrEmpty(this KeyValuePair<string, object> parameter)
    {
        if (parameter.Key == null || parameter.Value == null)
            throw new KeyNotFoundException(nameof(parameter));

        if (parameter.Key != null && parameter.Key != string.Empty)
            if (parameter.Value != null)
                return parameter;

        throw new KeyNotFoundException(nameof(parameter));
    }
}

public class CommandArgs
{
    public string? Command { get; set; }
    public ParameterDict<string, object>? Parameters { get; set; }
    public List<string>? Switches { get; set; }

    public CommandArgs(string command)
    {
        Command = command;
        Parameters = new ParameterDict<string, object>();
        Switches = new List<string>();
    }
}

public class ParameterDict<TKey, TValue> : Dictionary<TKey, TValue> where TKey : IEquatable<TKey>
{
    public KeyValuePair<TKey, TValue> this[int index]
    {
        get
        {
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException(nameof(index));

            var key = Keys.ToArray()[index];
            var value = Values.ToArray()[index];

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public KeyValuePair<TKey, TValue> this[string key]
    {
        get
        {
            return this[key];
        }
    }
}