using System.Text;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("RedRover.Puzzle Version 1.0");
Console.ResetColor();

PrintStartInput();
Console.WriteLine("");

while (true)
{
   Console.WriteLine("Enter a string to parse (or 'q' to quit): ");
   string input = Console.ReadLine() ?? string.Empty;

   if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
      break;

   // do something with input
   PrintInput(input);
   Console.WriteLine("");
}


void PrintStartInput()
{
   //note:  this is a hardcoded string for testing purposes, but the code should work for any string in this format
   const string sourceString = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
   PrintInput(sourceString);   
}

void PrintInput(string sourceString)
{
   Console.WriteLine($"Source String: {sourceString}");
   Console.WriteLine("");

   Console.ForegroundColor = ConsoleColor.Green;
   Console.WriteLine("############   Default Sort Order   ############");
   Console.ResetColor();

   try 
   {
      var defaultSort = Parse(sourceString);
      SortAndPrintResults(defaultSort);

      Console.WriteLine("");
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("############   Alphabetical Sort   ############");
      Console.ResetColor();

      var alphaSort = Parse(sourceString);
      SortAndPrintResults(alphaSort, sort: true);
   }   
   catch (ArgumentException ex)
   {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Invalid input \"{sourceString}\". Make sure input is in a valid format: {ex.Message}");
      Console.ResetColor();
      return;
   }
   catch (Exception ex)
   {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Error parsing input: {ex.Message}");
      Console.ResetColor();
      return;
   }

}

//recursively print the sorted results
void SortAndPrintResults(Node node, int level = 0, bool sort = false)
{
   if (level > 0)
      Console.WriteLine($"{new string(' ', level * 2)}- {node.Field}");

   level++;

   if (sort)
      node.Children.Sort((a, b) => string.Compare(a.Field, b.Field));

   foreach (var child in node.Children)
      SortAndPrintResults(child, level, sort);
}

//single pass through the string to build a stack of nodes
Node Parse(string source)
{
   ValidateInput(source);

   //adding a fake root node to make this work cleanly
   var root = new Node { Field = "root" };

   //this is where we store our breadcrumbs so we can step back up to the parent node when we hit a closing paren
   var nodeList = new List<Node> { root };

   var fieldName = new StringBuilder();

   foreach (char c in source)
   {
      Node current = nodeList[nodeList.Count - 1];

      switch (c)
      {
         case '(':
            var added = AddNode(current, fieldName);
            if (!string.IsNullOrWhiteSpace(added.Field))
               nodeList.Add(added);
            break;

         case ')':
            AddNode(current, fieldName);
            if (nodeList.Count > 1)  //done adding, step back up to the parent node
               nodeList.RemoveAt(nodeList.Count - 1);
            break;

         case ',':
            AddNode(current, fieldName);
            break;

         case ' ': //this will remove all whitespace - would it be better to throw and argumenet exception?
            break;

         default: //do we need to filter any other characters here - leading numbers, special characters, etc? 
            fieldName.Append(c);
            break;
      }
   }

   return root;
}

Node AddNode(Node parent, StringBuilder fieldName)
{
   
   var node = new Node { Field = fieldName.ToString().Trim(), Order = parent.Children.Count };
   fieldName.Clear();
   if (!string.IsNullOrWhiteSpace(node.Field))
      parent.Children.Add(node);
   return node;
}

void ValidateInput(string inputString)
{
   ArgumentException.ThrowIfNullOrWhiteSpace(inputString);
   if (!inputString.StartsWith('('))
      throw new ArgumentException("Input string must start with '('", nameof(inputString));
   if (!inputString.EndsWith(')'))
      throw new ArgumentException("Input string must end with ')'", nameof(inputString));   
   if (inputString.Count(c => c == '(') != inputString.Count(c => c == ')'))
      throw new ArgumentException("Input string has mismatched parentheses", nameof(inputString));
   //TODO: add more validation
}

class Node
{
   public required string Field { get; set; }
   //this prop gives a way to reset the sort
   public int Order { get; set; }
   public List<Node> Children { get; set; } = [];
}