// Copyright (C) WithLithum 2022
// Licensed under Apache-2.0 license

using WithLithum.ApiCodeGen;

Console.WriteLine("Native API code generator for C#");

if (args.Length != 1)
{
    Console.WriteLine("Invalid param");
}

try
{
    var gen = new Generator(args[0]);
    gen.BuildFinalData();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}