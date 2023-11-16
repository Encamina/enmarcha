using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Encamina.Enmarcha.AspNet.Mvc.Filters;

/// <summary>
/// A filter that asynchronously confirms multiple request authorizations.
/// </summary>
public sealed class AuthorizeMultiplePolicyFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthorizationService authorization;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeMultiplePolicyFilter"/> class.
    /// </summary>
    /// <param name="policiesNames">A comma separated list of policies' names to evaluate.</param>
    /// <param name="requireAllPolicies">A value indicating whether all policies defined by ?<paramref name="policiesNames"/> are required.</param>
    /// <param name="authorization">A valid reference to an authorization service to use when evaluating the policies.</param>
    public AuthorizeMultiplePolicyFilter(string policiesNames, bool requireAllPolicies, IAuthorizationService authorization)
        : this(policiesNames.Split(',', StringSplitOptions.RemoveEmptyEntries), requireAllPolicies, authorization)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeMultiplePolicyFilter"/> class.
    /// </summary>
    /// <param name="policiesNames">A list of policies' names to evaluate.</param>
    /// <param name="requireAllPolicies">A value indicating whether all policies defined by ?<paramref name="policiesNames"/> are required.</param>
    /// <param name="authorization">A valid reference to an authorization service to use when evaluating the policies.</param>
    public AuthorizeMultiplePolicyFilter(IEnumerable<string> policiesNames, bool requireAllPolicies, IAuthorizationService authorization)
    {
        this.authorization = authorization;

        PoliciesNames = policiesNames;
        RequireAllPolicies = requireAllPolicies;
    }

    /// <summary>
    /// Gets a collection of names of policies to evaluate.
    /// </summary>
    public IEnumerable<string> PoliciesNames { get; init; }

    /// <summary>
    /// Gets a value indicating whether all policies defined by <see cref="PoliciesNames"/> are required.
    /// </summary>
    public bool RequireAllPolicies { get; init; }

    /// <inheritdoc/>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        foreach (var policy in PoliciesNames)
        {
            var authorized = await authorization.AuthorizeAsync(context.HttpContext.User, policy);

            if (RequireAllPolicies)
            {
                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else
            {
                if (authorized.Succeeded)
                {
                    return;
                }
            }
        }

        context.Result = new ForbidResult();
    }
}
