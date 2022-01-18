// Copyright (C) WithLithum 2022
// Licensed under Apache-2.0 license

namespace WithLithum.ApiCodeGen;

using Newtonsoft.Json;
using System.Collections.Generic;

public class Native
{
    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = "";

    [JsonProperty("jhash", Required = Required.Always)]
    public string JHash { get; set; } = "";

    [JsonProperty("comment", Required = Required.Always)]
    public string Comment { get; set; } = "";

    [JsonProperty("return_type", Required = Required.Always)]
    public string ReturnType { get; set; } = "";

    [JsonProperty("params", Required = Required.DisallowNull)]
    public List<Param> Params { get; set; } = new List<Param>();
}

public class Param
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("type")]
    public string Type { get; set; } = "";
}