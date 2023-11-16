using Microsoft.Bot.Builder.Dialogs;

#pragma warning disable S2360 // Optional parameters should not be used

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Represents a dialog provider to get <see cref="Dialog"/>s by its type.
/// </summary>
public interface IDialogTypeProvider
{
    /// <summary>
    /// Gets a <see cref="Dialog"/> from its type.
    /// </summary>
    /// <param name="dialogType">The type of the <see cref="Dialog"/> to get.</param>
    /// <param name="filterExpression">An optional filter expression to help selecting the correct (and expected) <see cref="Dialog"/> instance.</param>
    /// <returns>
    /// A valid <see cref="Dialog"/>.
    /// </returns>
    Dialog GetByType(Type dialogType, Func<Dialog, bool> filterExpression = null);

    /// <summary>
    /// Gets a <see cref="Dialog"/> by its type from a generic type parameter.
    /// </summary>
    /// <typeparam name="T">The type of the dialog to get.</typeparam>
    /// <param name="filterExpression">An optional filter expression to help selecting the correct (and expected) <see cref="Dialog"/> instance.</param>
    /// <returns>
    /// A valid <see cref="Dialog"/>.
    /// </returns>
    T GetByType<T>(Func<Dialog, bool> filterExpression = null) where T : Dialog;

    /// <summary>
    /// Tries to get a <see cref="Dialog"/> from its type.
    /// </summary>
    /// <param name="dialogType">The type of the <see cref="Dialog"/> to get.</param>
    /// <param name="dialog">
    /// When this method returns, the <see cref="Dialog"/> associated with the specified type, if the
    /// type is found; otherwise, <see langword="null"/> or default's <see cref="Dialog"/> value. This
    /// parameter is passed uninitialized.
    /// </param>
    /// <param name="filterExpression">An optional filter expression to help selecting the correct (and expected) <see cref="Dialog"/> instance.</param>
    /// <returns>
    /// Returns <see langword="true"/> if a valid <see cref="Dialog"/> is found by its type; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetByType(Type dialogType, out Dialog dialog, Func<Dialog, bool> filterExpression = null);

    /// <summary>
    /// Tries to get a <see cref="Dialog"/> by its type from a generic type parameter.
    /// </summary>
    /// <typeparam name="T">The type of the dialog to get.</typeparam>
    /// <param name="dialog">
    /// When this method returns, the <see cref="Dialog"/> associated with the specified generic type
    /// parameter, if the type is found; otherwise, <see langword="null"/> or default's <see cref="Dialog"/>
    /// value. This parameter is passed uninitialized.
    /// </param>
    /// <param name="filterExpression">An optional filter expression to help selecting the correct (and expected) <see cref="Dialog"/> instance.</param>
    /// <returns>
    /// Returns <see langword="true"/> if a valid <see cref="Dialog"/> is found by its type; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetByType<T>(out T dialog, Func<Dialog, bool> filterExpression = null) where T : Dialog;
}

#pragma warning restore S2360 // Optional parameters should not be used
