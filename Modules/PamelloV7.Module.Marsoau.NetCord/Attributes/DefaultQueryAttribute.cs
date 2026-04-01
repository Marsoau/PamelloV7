namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class DefaultQueryAttribute : Attribute
{
    public string Query { get; set; }
    public bool AtLeastOne { get; set; }
    
    public DefaultQueryAttribute(string query, bool atLeastOne = false) { 
        Query = query;
        AtLeastOne = atLeastOne;
    }
}
