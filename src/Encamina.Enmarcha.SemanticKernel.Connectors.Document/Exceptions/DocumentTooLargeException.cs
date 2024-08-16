using System.Runtime.Serialization;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Exceptions;

/// <summary>
/// The exception that is thrown when there has been an error with the document size.
/// </summary>
[Serializable]
public class DocumentTooLargeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTooLargeException"/> class.
    /// </summary>
    public DocumentTooLargeException() : base(Resources.ExceptionMessages.DocumentTooLargeExceptionMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTooLargeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes this exception.</param>
    public DocumentTooLargeException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTooLargeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes this exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception.
    /// If the <paramref name="innerException"/> parameter is not <see langword="null"/>, then the
    /// current exception is raised in a catch block that handles the inner exception.
    /// </param>
    public DocumentTooLargeException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTooLargeException"/> class.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected DocumentTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
