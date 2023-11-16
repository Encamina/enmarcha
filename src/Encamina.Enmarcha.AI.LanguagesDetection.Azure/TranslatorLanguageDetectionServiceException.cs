using System.Runtime.Serialization;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// The exception that is thrown when there has been an error with the <see cref="TranslatorLanguageDetectionServiceException">translation language detection service</see>.
/// </summary>
[Serializable]
public class TranslatorLanguageDetectionServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionServiceException"/> class.
    /// </summary>
    public TranslatorLanguageDetectionServiceException() : base(Resources.ExceptionMessages.DefaultTranslationLanguageDetectionServiceExceptionMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionServiceException"/> class.
    /// </summary>
    /// <param name="message">The message that describes this exception.</param>
    public TranslatorLanguageDetectionServiceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionServiceException"/> class.
    /// </summary>
    /// <param name="message">The message that describes this exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception.
    /// If the <paramref name="innerException"/> parameter is not <see langword="null"/>, then the
    /// current exception is raised in a catch block that handles the inner exception.
    /// </param>
    public TranslatorLanguageDetectionServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionServiceException"/> class.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected TranslatorLanguageDetectionServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
