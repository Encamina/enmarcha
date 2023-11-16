using Microsoft.AspNetCore.Mvc;

namespace Encamina.Enmarcha.AspNet.Mvc.Filters;

/// <summary>
/// A filter that allows evaluating multiple authorization policies per request.
/// </summary>
public sealed class AuthorizeMultiplePolicyAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeMultiplePolicyAttribute"/> class.
    /// </summary>
    /// <param name="policiesNames">A comma separated list of policies' names to evaluate.</param>
    /// <param name="requireAllPolicies">A value indicating whether all policies defined by ?<paramref name="policiesNames"/> are required.</param>
    public AuthorizeMultiplePolicyAttribute(string policiesNames, bool requireAllPolicies) : base(typeof(AuthorizeMultiplePolicyFilter))
    {
        Arguments = new object[] { policiesNames, requireAllPolicies };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeMultiplePolicyAttribute"/> class.
    /// </summary>
    /// <param name="policiesNames">A list of policies' names to evaluate.</param>
    /// <param name="requireAllPolicies">A value indicating whether all policies defined by ?<paramref name="policiesNames"/> are required.</param>
    public AuthorizeMultiplePolicyAttribute(IEnumerable<string> policiesNames, bool requireAllPolicies) : base(typeof(AuthorizeMultiplePolicyFilter))
    {
        Arguments = new object[] { policiesNames, requireAllPolicies };
    }
}
