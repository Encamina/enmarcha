using System.Diagnostics.CodeAnalysis;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Encamina.Enmarcha.Agents.Controllers;

/// <summary>
/// A base controller to handle a request from web chats or DirectLine clients. Dependency Injection will provide the 'Adapter' and '<see cref="IAgent"/>'
/// implementation at runtime. Multiple different <see cref="IAgent"/> implementations running at different endpoints can be achieved by specifying a
/// more specific type (i.e, different interface) for the agent constructor argument. <b>This class is abstract</b>.
/// </summary>
[ApiController]
[Route(@"api/messages")]
[SuppressMessage(@"Minor Code Smell",
                 @"S1694:An abstract class should have both abstract and concrete methods",
                 Justification = @"This class must be abstract; otherwise, any inheritance might produce a 'AmbiguousMatchException' because requests will match multiple endpoints")]
public abstract class AgentBaseController : ControllerBase
{
    private readonly IAgent agent;
    private readonly IAgentHttpAdapter adapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentBaseController"/> class.
    /// </summary>
    /// <param name="adapter">An agent adapter to use.</param>
    /// <param name="agent">An agent that can operate on incoming activities.</param>
    protected AgentBaseController(IAgentHttpAdapter adapter, IAgent agent)
    {
        this.adapter = adapter;
        this.agent = agent;
    }

    /// <summary>
    /// Handles a request for the agent.
    /// </summary>
    /// <remarks>
    /// This method does not use a `CancellationToken` because Azure Bot Service would cancel the request if it did not receive a response within 15 seconds.
    /// </remarks>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of handling the request for the agent.
    /// </returns>
    [HttpGet]
    [HttpPost]
    public virtual async Task HandleAsync()
    {
        // Delegate the processing of the HTTP request to the adapter. The adapter will invoke the agent.
        await adapter.ProcessAsync(Request, Response, agent);
    }
}