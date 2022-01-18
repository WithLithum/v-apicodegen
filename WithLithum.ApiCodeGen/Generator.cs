// Copyright (C) WithLithum 2022
// Licensed under Apache-2.0 license

namespace WithLithum.ApiCodeGen;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

internal class Generator
{
    private readonly Dictionary<string, Dictionary<string, Native>> _data;
    private readonly string _ns;
    private readonly List<string> _pointered = new List<string>();

    internal Generator(string ns)
    {
        _data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Native>>>(
            Encoding.Default.GetString(UserData.Natives)) ?? throw new InvalidOperationException("Unexcepted error when parsing data");

        if (string.IsNullOrWhiteSpace(ns))
        {
            throw new ArgumentException("Invalid namespace");
        }

        if (!_data.ContainsKey(ns))
        {
            throw new ArgumentException("No such namespace");
        }

        _ns = ns;
    }

    internal void BuildFinalData()
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"// Generated via ApiCodeGen

#pragma warning disable CS1591

namespace WithLithum.Core.Util.Native;
using GTA;
using GTA.Native;
            
public static partial class Api
{");

        foreach (var native in _data[_ns])
        {
            sb.Append("    // ").AppendLine(native.Key);
            sb.AppendLine("    /// <summary>");

            sb.Append("    /// Hash: ").AppendLine(native.Key);

            sb.AppendLine("    /// </summary>");
            sb.Append("    public static ")
                .Append(ProcessType(native.Value.ReturnType, out var rtnPointer))
                .Append(' ')
                .Append(native.Value.Name)
                .Append('(');

            if (native.Value.Params.Count > 0)
            {
                foreach (var para in native.Value.Params)
                {
                    sb.Append(ProcessType(para.Type, out var paramPointer))
                        .Append(' ')
                        .Append(para.Name)
                        .Append(',');

                    if (paramPointer)
                    {
                        rtnPointer = true;
                    }
                }

                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(')')
                .Append(" => Function.Call");

            if (native.Value.ReturnType != "void")
            {
                sb.Append('<')
                    .Append(ProcessType(native.Value.ReturnType, out _))
                    .Append('>');
            }

            sb.Append("((Hash)")
                .Append(native.Key)
                .Append(',');

            if (native.Value.Params.Count > 0)
            {
                foreach (var para in native.Value.Params)
                {
                    sb.Append(para.Name)
                        .Append(',');
                }

                sb.Remove(sb.Length - 1, 1);
            }
            sb.AppendLine(");")
                .AppendLine();

            if (rtnPointer)
            {
                _pointered.Add(native.Key);
            }
        }

        sb.AppendLine("}");

        Console.WriteLine("Writing output");
        File.WriteAllText($"Api.{_ns.ToLowerInvariant()}.cs", sb.ToString());

        Console.ForegroundColor = ConsoleColor.Yellow;
        foreach (var p in _pointered)
        {
            Console.WriteLine("Warning: Native with hash {0} needs your attention.", p);
        }

        Console.ResetColor();
    }

    internal static string ProcessType(string type, out bool pointer)
    {
        var result = type
            .Replace("const char*", "string")
            .Replace("Hash", "uint")
            .Replace("Any*", "ref object /* TODO: FIX THIS */")
            .Replace("int*", "ref int /* TODO: FIX THIS */")
            .Replace("Any", "object")
            .Replace("BOOL", "bool")
            .Replace("Ped", "uint")
            .Replace("Vehicle", "uint")
            .Replace("Object", "uint")
            .Replace("Prop", "uint")
            .Replace("Blip", "uint")
            .Replace("uint*", "ref uint /* TODO: FIX THIS */");

        pointer = result.Contains("/* TODO: FIX THIS */");

        return result;
    }
}
