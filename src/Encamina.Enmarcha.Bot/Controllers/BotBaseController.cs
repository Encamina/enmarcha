using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Encamina.Enmarcha.Bot.Controllers;

/// <summary>
/// A base controller to handle a request from web chats or DirectLine clients. Dependency Injection will provide the 'Adapter' and '<see cref="IBot"/>'
/// implementation at runtime. Multiple different <see cref="IBot"/> implementations running at different endpoints can be achieved by specifying a
/// more specific type (i.e, different interface) for the bot constructor argument. <b>This class is abstract</b>.
/// </summary>
[ApiController]
[Route(@"api/messages")]
[SuppressMessage(@"Minor Code Smell",
                 @"S1694:An abstract class should have both abstract and concrete methods",
                 Justification = @"This class must be abstract; otherwise, any inheritance might produce a 'AmbiguousMatchException' because requests will match multiple endpoints")]
public abstract class BotBaseController : ControllerBase
{
    private readonly IBot bot;
    private readonly IBotFrameworkHttpAdapter adapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotBaseController"/> class.
    /// </summary>
    /// <param name="adapter">A bot adapter to use.</param>
    /// <param name="bot">A bot that can operate on incoming activities.</param>
    protected BotBaseController(IBotFrameworkHttpAdapter adapter, IBot bot)
    {
        this.adapter = adapter;
        this.bot = bot;
    }

    /// <summary>
    /// Handles a request for the bot.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of handling the request for the bot.
    /// </returns>
    [HttpGet]
    [HttpPost]
    public virtual async Task HandleAsync()
    {
        // Delegate the processing of the HTTP request to the adapter. The adapter will invoke the bot.
        await adapter.ProcessAsync(Request, Response, bot);
    }
}