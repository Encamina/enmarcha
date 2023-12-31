﻿using Microsoft.SemanticKernel.Orchestration;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Utility methods for Semantic Kernel context.
/// </summary>
public static class SKContextExtensions
{
    /// <summary>
    /// Search the value of the given variable in the context.
    /// </summary>
    /// <param name="context">The SKContext object.</param>
    /// <param name="variableName">The name of the variable to retrieve.</param>
    /// <param name="throwExceptionIfNotFound">Whether or not to throw an exception if the variable is not found.</param>
    /// <returns>The value of the variable if found, null or exception otherwise.</returns>
    public static string GetContextVariable(this SKContext context, string variableName, bool throwExceptionIfNotFound = false)
    {
        if (context.Variables.ContainsKey(variableName))
        {
            return context.Variables[variableName];
        }

        return throwExceptionIfNotFound
            ? throw new ArgumentException($"Variable {variableName} not found in SK Context.")
            : null;
    }
}