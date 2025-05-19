namespace Truestory.Frontend.Services;

public class FlashMessageService(IHttpContextAccessor httpContextAccessor)
{
    public void SetMessage(string message, FlashMessageType type)
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null) return;

        context.Session.SetString("FlashMessage", message);
        context.Session.SetString("FlashMessageType", type.ToString());
    }

    public bool HasMessage()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null) return false;

        var message = context.Session.GetString("FlashMessage");
        return !string.IsNullOrEmpty(message);
    }

    public string? GetMessage()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null) return null;

        var message = context.Session.GetString("FlashMessage");
        context.Session.Remove("FlashMessage");
        return message;
    }

    public FlashMessageType GetMessageType()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null) return FlashMessageType.Info;

        var typeString = context.Session.GetString("FlashMessageType");
        context.Session.Remove("FlashMessageType");

        return Enum.TryParse(typeString, out FlashMessageType type) ? type : FlashMessageType.Info;
    }
}

public enum FlashMessageType
{
    Info,
    Success,
    Warning,
    Error
}
