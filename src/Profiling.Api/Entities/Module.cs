using System;

namespace Profiling.Api.Entities;

public class Module
{
    public int Id { get; set; }
    public string Resource { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }

    public override string ToString()
    {
        var res = "{" + $"Id: {Id}, Resource: {Resource}, Name: {Name}, NormalizedName: {NormalizedName}" + "}";
        return res;
    }
}
