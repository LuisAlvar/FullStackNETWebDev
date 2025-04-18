

if (args.Length != 2) throw new ArgumentException("Missing arguments");
if (!File.Exists(args[0].Trim())) throw new Exception("The excel file is not exist");

File.Open
