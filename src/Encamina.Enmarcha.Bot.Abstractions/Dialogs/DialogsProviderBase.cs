using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Bot.Builder.Dialogs;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Base class for <see cref="Dialog"/>s providers.
/// </summary>
/// <remarks>
/// This base class is intended to provide all possible dialog provisioning alternatives, such as providing a dialog from its ID, name, or intent.
/// </remarks>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class DialogsProviderBase : IDialogProvider, IIntendedDialogProvider, INamedDialogProvider, IDialogTypeProvider
{
    private static readonly Func<Dialog, string, bool> FilterByIntent = (d, i) => d is IIntendable intendable && intendable.Intent.Equals(i, StringComparison.OrdinalIgnoreCase);
    private static readonly Func<Dialog, string, bool> FilterByName = (d, n) => d is INameable nameable && nameable.Name.Equals(n, StringComparison.OrdinalIgnoreCase);
    private static readonly Func<Dialog, string, bool> FilterById = (d, i) => d.Id.Equals(i, StringComparison.OrdinalIgnoreCase);

    private readonly IDictionary<string, Type> dialogsById;
    private readonly IDictionary<string, Type> dialogsByName;
    private readonly IDictionary<string, Type> dialogsByIntent;

    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogsProviderBase"/> class.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: This class implements a Service Locator pattern.
    /// </remarks>
    /// <param name="serviceProvider">A service provider used to create a scope from which to retrieve <see cref="Dialog"/>s.</param>
    protected DialogsProviderBase(IServiceProvider serviceProvider)
    {
        Guard.IsNotNull(serviceProvider);

        this.serviceProvider = serviceProvider;

        using var serviceScope = this.serviceProvider.CreateScope();
        var dialogs = serviceScope.ServiceProvider.GetServices<Dialog>();

        dialogsById = dialogs.ToDictionary(d => d.Id, d => d.GetType());
        dialogsByName = BuildFromName(dialogs);
        dialogsByIntent = BuildFromIntent(dialogs);
    }

    /// <inheritdoc/>
    public virtual Dialog GetById(string dialogId) => GetDialogByType(dialogsById?[dialogId], d => FilterById(d, dialogId));

    /// <inheritdoc/>
    public virtual Dialog GetByIntent(string dialogIntent) =>
        GetDialogByType(dialogsByIntent?[dialogIntent.ToUpperInvariant()], d => FilterByIntent(d, dialogIntent));

    /// <inheritdoc/>
    public virtual Dialog GetByName(string dialogName) =>
        GetDialogByType(dialogsByName?[dialogName.ToUpperInvariant()], d => FilterByName(d, dialogName));

    /// <inheritdoc/>
    public virtual bool TryGetById(string dialogId, out Dialog dialog)
    {
        if (dialogsById == null)
        {
            dialog = null;
            return false;
        }

        dialog = dialogsById.TryGetValue(dialogId.ToUpperInvariant(), out var dialogType)
            ? GetDialogByType(dialogType, d => FilterById(d, dialogId))
            : null;

        return dialog != null;
    }

    /// <inheritdoc/>
    public virtual bool TryGetByIntent(string dialogIntent, out Dialog dialog)
    {
        if (dialogsByIntent == null)
        {
            dialog = null;
            return false;
        }

        dialog = dialogsByIntent.TryGetValue(dialogIntent.ToUpperInvariant(), out var dialogType)
            ? GetDialogByType(dialogType, d => FilterByIntent(d, dialogIntent))
            : null;

        return dialog != null;
    }

    /// <inheritdoc/>
    public virtual bool TryGetByName(string dialogName, out Dialog dialog)
    {
        if (dialogsByName == null)
        {
            dialog = null;
            return false;
        }

        dialog = dialogsByName.TryGetValue(dialogName.ToUpperInvariant(), out var dialogType)
            ? GetDialogByType(dialogType, d => FilterByName(d, dialogName))
            : null;

        return dialog != null;
    }

    /// <inheritdoc/>
    public virtual Dialog GetByType(Type dialogType, Func<Dialog, bool> filterExpression = null) => GetDialogByType(dialogType, filterExpression);

    /// <inheritdoc/>
    public virtual T GetByType<T>(Func<Dialog, bool> filterExpression = null) where T : Dialog => GetDialogByType<T>(filterExpression);

    /// <inheritdoc/>
    public virtual bool TryGetByType(Type dialogType, out Dialog dialog, Func<Dialog, bool> filterExpression = null)
    {
        dialog = GetDialogByType(dialogType, filterExpression);
        return dialog != null;
    }

    /// <inheritdoc/>
    public virtual bool TryGetByType<T>(out T dialog, Func<Dialog, bool> filterExpression = null) where T : Dialog
    {
        dialog = GetDialogByType<T>(filterExpression);
        return dialog != null;
    }

    private static IDictionary<string, Type> BuildFromName(IEnumerable<Dialog> dialogs)
    {
        var result = new Dictionary<string, Type>();

        if (dialogs?.Any() != true)
        {
            return result;
        }

        foreach (var dialog in dialogs)
        {
            if (dialog is INameable nameable)
            {
                result.Add(nameable.Name.ToUpperInvariant(), dialog.GetType());
            }
        }

        return result;
    }

    private static IDictionary<string, Type> BuildFromIntent(IEnumerable<Dialog> dialogs)
    {
        var result = new Dictionary<string, Type>();

        if (dialogs?.Any() != true)
        {
            return result;
        }

        foreach (var dialog in dialogs)
        {
            if (dialog is IIntendable intendable)
            {
                result.Add(intendable.Intent.ToUpperInvariant(), dialog.GetType());
            }
        }

        return result;
    }

    private Dialog GetDialogByType(Type dialogType, Func<Dialog, bool> filterPredicate)
    {
        if (dialogType == null)
        {
            return null;
        }

        using var serviceScope = serviceProvider.CreateScope();
        return (serviceScope.ServiceProvider.GetServices(dialogType) as IEnumerable<Dialog>)?.FirstOrDefault(d => filterPredicate == null || filterPredicate(d));
    }

    private T GetDialogByType<T>(Func<Dialog, bool> filterPredicate) where T : Dialog
    {
        using var serviceScope = serviceProvider.CreateScope();
        return serviceScope.ServiceProvider.GetServices<T>()?.FirstOrDefault(d => filterPredicate == null || filterPredicate(d));
    }
}
