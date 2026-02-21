
public readonly record struct EventClassDescriptor(
    string Namespace,
    string ClassName,
    bool NeedsInvoker,
    bool NeedsRevertPack,
    bool HasDefaultPack
);