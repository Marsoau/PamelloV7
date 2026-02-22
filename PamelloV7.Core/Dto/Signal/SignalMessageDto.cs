namespace PamelloV7.Core.Dto.Signal;

public record SignalMessageDto(
    int AuthorId,
    bool IsPrivate,
    string Message
);
