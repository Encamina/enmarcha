using System.Configuration;
using System.Runtime.Serialization;
using System.Xml;

namespace Encamina.Enmarcha.Core;

/// <summary>
/// The expected exception when a configuration error occurs due to a missing configuration parameter or value.
/// </summary>
/// <remarks>
/// The <see cref="MissingConfigurationException"/> exception should be thrown when validating expected configuration
/// parameters and they're not found, or if any error occurs while required configuration information is being read.
/// </remarks>
[Serializable]
public class MissingConfigurationException : ConfigurationErrorsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    public MissingConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    public MissingConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="inner">The exception that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    public MissingConfigurationException(string message, Exception inner) : base(message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="node">The <see cref="XmlNode"/> object that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    public MissingConfigurationException(string message, XmlNode node) : base(message, node)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="reader">The <see cref="XmlReader"/> object that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    public MissingConfigurationException(string message, XmlReader reader) : base(message, reader)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="filename">The path to the configuration file that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    /// <param name="line">The line number within the configuration file at which this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    public MissingConfigurationException(string message, string filename, int line) : base(message, filename, line)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="inner">The exception that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    /// <param name="node">The <see cref="XmlNode"/> object that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    public MissingConfigurationException(string message, Exception inner, XmlNode node) : base(message, inner, node)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="inner">The exception that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    /// <param name="reader">The <see cref="XmlReader"/> object that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    public MissingConfigurationException(string message, Exception inner, XmlReader reader) : base(message, inner, reader)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes why this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    /// <param name="inner">The exception that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    /// <param name="filename">The path to the configuration file that caused this <see cref="MissingConfigurationException"/> exception to be thrown.</param>
    /// <param name="line">The line number within the configuration file at which this <see cref="MissingConfigurationException"/> exception was thrown.</param>
    public MissingConfigurationException(string message, Exception inner, string filename, int line) : base(message, inner, filename, line)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="info">The object that holds the information to deserialize.</param>
    /// <param name="context">Contextual information about the source or destination.</param>
    protected MissingConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
