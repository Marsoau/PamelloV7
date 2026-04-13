using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloCommand))]
[RequiredMethodName("Execute")]

public class PamelloCommandAttribute : Attribute;