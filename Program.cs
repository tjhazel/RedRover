using System.Text;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("RedRover.Puzzle Version 1.0");
Console.ResetColor();

PrintDefaultResults();
Console.WriteLine("");
Console.WriteLine("Done!  Hit Enter to close");

var newString = Console.ReadLine();

void PrintDefaultResults()
{
   //note:  this is a hardcoded string for testing purposes, but the code should work for any string in this format
   const string sourceString = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
   Console.WriteLine($"Source String: {sourceString}");
   Console.WriteLine("");

   Console.ForegroundColor = ConsoleColor.Green;
   Console.WriteLine("############   Default Sort Order   ############");
   Console.ResetColor();

   var defaultSort = Parse(sourceString);
   PrintResults(defaultSort);
    
   Console.WriteLine("");
   Console.ForegroundColor = ConsoleColor.Green;
   Console.WriteLine("############   Alphabetical Sort   ############");
   Console.ResetColor();

   var alphaSort = Parse(sourceString);
   PrintResults(alphaSort, sort: true);
}

void PrintResults(Node node, int level = 0, bool sort = false)
{
   if (level > 0)
      Console.WriteLine($"{new string(' ', level * 2)}- {node.Field}");

   level++;

   if (sort)
      node.Children.Sort((a, b) => string.Compare(a.Field, b.Field));

   foreach (var child in node.Children)
      PrintResults(child, level, sort);
}

Node Parse(string source)
{
   //adding a fake root node to make this work cleanly
   var root = new Node { Field = "root" };
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

         case ' ':
            break;

         default:
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

class Node
{
   public required string Field { get; set; }
   public int Order { get; set; }
   public List<Node> Children { get; set; } = [];
}