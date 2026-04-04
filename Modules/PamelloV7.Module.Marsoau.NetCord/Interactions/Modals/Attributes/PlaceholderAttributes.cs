namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddCheckBoxAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddCheckBoxGroupAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddCheckBoxOptionAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddSelectAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddSelectOptionAttribute(string name) : Attribute;
