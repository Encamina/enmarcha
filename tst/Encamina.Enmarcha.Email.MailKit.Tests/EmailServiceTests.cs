using System.Net.Mime;
using System.Text;

using Bogus;
using Bogus.DataSets;

using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Testing;
using Encamina.Enmarcha.Testing.Smtp;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Moq;

namespace Encamina.Enmarcha.Email.MailKit.Tests;

[Collection(MagicStrings.FixturesCollection)]
public class EmailServiceTests : FakerProviderFixturedBase, IDisposable
{
    private SmtpServer smtpServer;

    public EmailServiceTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
        smtpServer = Configuration.Configure()
                                  .WithRandomPort()
                                  .Build();
    }

    [Fact]
    public void EmailService_OptionsForSmtpClientOptions_AsNull_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new EmailService(null));

        // Assert...
        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"smtpClientOptions", argumentNullException.ParamName);
    }

    [Fact]
    public void EmailService_SmtpClientOptions_AsNull_ThrowsException()
    {
        // Arrange...
        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns<SmtpClientOptions>(null);

        // Act...
        var exception = Record.Exception(() => new EmailService(mockOptions.Object));

        // Assert...
        Assert.NotNull(exception);
        var argumetnNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"smtpClientOptions", argumetnNullException.ParamName);
    }

    [Fact]
    public void EmailService_SmtpClientOptions_ValidOptions_WithDefaultName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var smtpClientOptions = BuildValidSmtpClientOptions(faker);

        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(smtpClientOptions);

        // Act...
        var emailService = new EmailService(mockOptions.Object);

        // Assert...
        var smtpClientOptionsProvider = Assert.IsAssignableFrom<ISmtpClientOptionsProvider>(emailService);
        Assert.NotNull(smtpClientOptionsProvider?.SmtpClientOptions);
        Assert.Equal(smtpClientOptions.Name, smtpClientOptionsProvider.SmtpClientOptions.Name);
        Assert.Equal(smtpClientOptions.Password, smtpClientOptionsProvider.SmtpClientOptions.Password);
        Assert.Equal(smtpClientOptions.Host, smtpClientOptionsProvider.SmtpClientOptions.Host);
        Assert.Equal(smtpClientOptions.Port, smtpClientOptionsProvider.SmtpClientOptions.Port);
        Assert.Equal(smtpClientOptions.User, smtpClientOptionsProvider.SmtpClientOptions.User);
        Assert.Equal(smtpClientOptions.UseSSL, smtpClientOptionsProvider.SmtpClientOptions.UseSSL);

        Assert.Equal(smtpClientOptions.Name, emailService.Name);
        Assert.Equal($@"{smtpClientOptions.Host}:{smtpClientOptions.Port}", emailService.Name);
    }

    [Fact]
    public void EmailService_SmtpClientOptions_ValidOptions_WithCustomName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var smtpClientOptions = new SmtpClientOptions()
        {
            Name = faker.Random.Word(),
            Host = smtpServer.Configuration.Domain,
            Port = smtpServer.Configuration.Port,
            Password = faker.Internet.Password(),
            User = faker.Internet.Email(),

            // For test porposes, SSL should not be used. Otherwise, a `MailKit.Security.SslHandshakeException` might be thrown...
            UseSSL = false,
        };

        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(smtpClientOptions);

        // Act...
        var emailService = new EmailService(mockOptions.Object);

        // Assert...
        var smtpClientOptionsProvider = Assert.IsAssignableFrom<ISmtpClientOptionsProvider>(emailService);
        Assert.NotNull(smtpClientOptionsProvider?.SmtpClientOptions);
        Assert.Equal(smtpClientOptions.Name, smtpClientOptionsProvider.SmtpClientOptions.Name);
        Assert.Equal(smtpClientOptions.Password, smtpClientOptionsProvider.SmtpClientOptions.Password);
        Assert.Equal(smtpClientOptions.Host, smtpClientOptionsProvider.SmtpClientOptions.Host);
        Assert.Equal(smtpClientOptions.Port, smtpClientOptionsProvider.SmtpClientOptions.Port);
        Assert.Equal(smtpClientOptions.User, smtpClientOptionsProvider.SmtpClientOptions.User);
        Assert.Equal(smtpClientOptions.UseSSL, smtpClientOptionsProvider.SmtpClientOptions.UseSSL);

        Assert.Equal(smtpClientOptions.Name, emailService.Name);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public void EmailService_SmtpClientOptions_WithHost_AsEmptyOrWhiteSpace_ThrowsException(string invalidHost)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testSmtpClientOptions = new SmtpClientOptions()
        {
            Host = invalidHost,
            Password = faker.Internet.Password(),
            Port = faker.Internet.Port(),
            User = faker.Internet.UserName(),
            UseSSL = faker.Internet.Random.Bool(),
        };

        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(testSmtpClientOptions);

        // Act...
        var exception = Record.Exception(() => new EmailService(mockOptions.Object));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"Host", argumentException.ParamName);
    }

    [Fact]
    public void EmailService_SmtpClientOptions_WithHost_AsNull_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testSmtpClientOptions = new SmtpClientOptions()
        {
            Host = null,
            Password = faker.Internet.Password(),
            Port = faker.Internet.Port(),
            User = faker.Internet.UserName(),
            UseSSL = faker.Internet.Random.Bool(),
        };

        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(testSmtpClientOptions);

        // Act...
        var exception = Record.Exception(() => new EmailService(mockOptions.Object));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EmailService_SmtpClientOptions_WithPort_AsZeroOrNegative_ThrowsException(int invalidPort)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testSmtpClientOptions = new SmtpClientOptions()
        {
            Host = faker.Internet.DomainName(),
            Password = faker.Internet.Password(),
            Port = invalidPort,
            User = faker.Internet.UserName(),
            UseSSL = faker.Internet.Random.Bool(),
        };

        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(testSmtpClientOptions);

        // Act...
        var exception = Record.Exception(() => new EmailService(mockOptions.Object));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"Port", argumentException.ParamName);
    }

    [Fact]
    public async Task EmailService_SendMail_WithAttachment_WithFileName_AsNull_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.AddAttachment(null, BuildValidAttachment(faker));

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"fileName", argumentNullException.ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public async Task EmailService_SendMail_WithAttachment_WithFileName_AsEmptyOrWhiteSpace_ThrowsException(string invalidAttachmentFileName)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);
        emailBuilder.AddAttachment(invalidAttachmentFileName, BuildValidAttachment(faker));

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentExcepton = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"fileName", argumentExcepton.ParamName);
    }

    [Fact]
    public async Task EmailService_SendMail_WithAttachment_WithData_AsNull_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);
        emailBuilder.AddAttachment(FakerProvider.GetFaker().System.FileName(), null);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"data", argumentNullException.ParamName);
    }

    [Fact]
    public async Task EmailService_AddAttachment_NoContentType_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testAttachmentName = faker.System.FileName();
        var testAttachmentData = BuildValidAttachment(faker);

        // Act...
        await emailBuilder.AddAttachment(testAttachmentName, testAttachmentData).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.Attachments);
        Assert.NotEmpty(emailBuilder.Specification.Attachments);
        Assert.Collection(emailBuilder.Specification.Attachments, a =>
        {
            Assert.Equal(testAttachmentName, a.FileName);
            Assert.Equal(testAttachmentData, a.Data);
            Assert.Null(a.ContentType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Collection(smtpServer.ReceivedMessages[0].Attachments, attachment =>
        {
            var buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(testAttachmentName, attachment.Name);
            Assert.Equal(testAttachmentData, buffer);
            Assert.NotNull(attachment.ContentType);
        });
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public async Task EmailService_AddAttachment_WithContentTypeValue_AsNullOrEmptyOrWhiteSpace_Succeeds(string contentTypeValue)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testAttachmentName = faker.System.FileName();
        var testAttachmentData = BuildValidAttachment(faker);

        // Act...
        await emailBuilder.AddAttachment(testAttachmentName, testAttachmentData, contentTypeValue: contentTypeValue).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.Attachments);
        Assert.NotEmpty(emailBuilder.Specification.Attachments);
        Assert.Collection(emailBuilder.Specification.Attachments, a =>
        {
            Assert.Equal(testAttachmentName, a.FileName);
            Assert.Equal(testAttachmentData, a.Data);
            Assert.Null(a.ContentType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Collection(smtpServer.ReceivedMessages[0].Attachments, attachment =>
        {
            var buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(testAttachmentName, attachment.Name);
            Assert.Equal(testAttachmentData, buffer);
            Assert.NotNull(attachment.ContentType);
        });
    }

    [Fact]
    public async Task EmailService_AddAttachment_WithContentType_AsNull_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testAttachmentName = faker.System.FileName();
        var testAttachmentData = BuildValidAttachment(faker);

        // Act...
        await emailBuilder.AddAttachment(testAttachmentName, testAttachmentData, contentType: null).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.Attachments);
        Assert.NotEmpty(emailBuilder.Specification.Attachments);
        Assert.Collection(emailBuilder.Specification.Attachments, a =>
        {
            Assert.Equal(testAttachmentName, a.FileName);
            Assert.Equal(testAttachmentData, a.Data);
            Assert.Null(a.ContentType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Collection(smtpServer.ReceivedMessages[0].Attachments, attachment =>
        {
            var buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(testAttachmentName, attachment.Name);
            Assert.Equal(testAttachmentData, buffer);
            Assert.NotNull(attachment.ContentType);
        });
    }

    [Fact]
    public async Task EmailService_AddAttachment_WithContentTypeValue_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testMimeType = faker.System.MimeType();
        var testAttachmentName = faker.System.FileName(faker.System.FileExt(testMimeType));
        var testAttachmentData = BuildValidAttachment(faker);

        // Act...
        await emailBuilder.AddAttachment(testAttachmentName, testAttachmentData, contentTypeValue: testMimeType).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.Attachments);
        Assert.NotEmpty(emailBuilder.Specification.Attachments);
        Assert.Collection(emailBuilder.Specification.Attachments, a =>
        {
            Assert.Equal(testAttachmentName, a.FileName);
            Assert.Equal(testAttachmentData, a.Data);
            Assert.Equal(testMimeType, a.ContentType?.MediaType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Collection(smtpServer.ReceivedMessages[0].Attachments, attachment =>
        {
            var buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(testAttachmentName, attachment.Name);
            Assert.Equal(testAttachmentData, buffer);
            Assert.NotNull(attachment.ContentType);
            Assert.Equal(testMimeType, attachment.ContentType.MediaType);
        });
    }

    [Fact]
    public async Task EmailService_AddAttachment_WithContentType_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testMimeType = faker.System.MimeType();
        var testAttachmentName = faker.System.FileName(faker.System.FileExt(testMimeType));
        var testAttachmentData = new byte[faker.Random.Int(1, 1000)];
        var testContentType = new ContentType(testMimeType);

        // Act...
        await emailBuilder.AddAttachment(testAttachmentName, testAttachmentData, testContentType).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.Attachments);
        Assert.NotEmpty(emailBuilder.Specification.Attachments);
        Assert.Single(emailBuilder.Specification.Attachments);
        Assert.Collection(emailBuilder.Specification.Attachments, a =>
        {
            Assert.Equal(testAttachmentName, a.FileName);
            Assert.Equal(testAttachmentData, a.Data);
            Assert.Equal(testContentType, a.ContentType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Collection(smtpServer.ReceivedMessages[0].Attachments, attachment =>
        {
            var buffer = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(testAttachmentName, attachment.Name);
            Assert.Equal(testAttachmentData, buffer);
            Assert.NotNull(attachment.ContentType);
            Assert.Equal(testContentType.MediaType, attachment.ContentType.MediaType);
        });
    }

    [Theory]
    [InlineData(@"", EmailRecipientType.CC)]
    [InlineData(@"", EmailRecipientType.TO)]
    [InlineData(@"", EmailRecipientType.BCC)]
    [InlineData(@" ", EmailRecipientType.CC)]
    [InlineData(@" ", EmailRecipientType.TO)]
    [InlineData(@" ", EmailRecipientType.BCC)]
    public async Task EmailService_AddRecipient_WithEmailAddress_AsEmptyOrWhiteSpace_WithRecipientName_AsNull_WithRecipientType_ThrowsException(string emailAddress, EmailRecipientType recipientType)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.AddRecipient(emailAddress, null, recipientType).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Theory]
    [InlineData(null, EmailRecipientType.TO)]
    [InlineData(null, EmailRecipientType.CC)]
    [InlineData(null, EmailRecipientType.BCC)]
    public async Task EmailService_AddRecipient_WithEmailAddress_AsNull_WithRecipientName_AsNull_WithRecipientType_ThrowsException(string emailAddress, EmailRecipientType recipientType)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.AddRecipient(emailAddress, null, recipientType).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"emailAddress", argumentNullException.ParamName);
    }

    [Fact]
    public async Task EmailService_AddRecipientBcc_WithValidEmailAddress_WithRecipientName_AsNull_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, null, EmailRecipientType.BCC).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.BCC, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].CC);
        Assert.Empty(smtpServer.ReceivedMessages[0].To);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);   // In SMTP, the `Bcc` field is not printed out to DATA like message headers (i.e, `TO`, `CC`). Any Bcc’ed recipient won’t be added to the recipients list.
    }

    [Fact]
    public async Task EmailService_AddRecipientCc_WithValidEmailAddress_WithRecipientName_AsNull_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, null, EmailRecipientType.CC).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.CC, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);
        Assert.Empty(smtpServer.ReceivedMessages[0].To);
        Assert.Collection(smtpServer.ReceivedMessages[0].CC, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(testEmailAddress, recipient.DisplayName);
        });
    }

    [Fact]
    public async Task EmailService_AddRecipientTo_WithValidEmailAddress_WithRecipientName_AsNull_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, null, EmailRecipientType.TO).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.TO, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].CC);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);
        Assert.Collection(smtpServer.ReceivedMessages[0].To, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(testEmailAddress, recipient.DisplayName);
        });
    }

    [Fact]
    public async Task EmailService_AddRecipientTo_WithValidEmailAddress_WithRecipientName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();
        var testRecipientName = faker.Person.FullName;

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, testRecipientName, EmailRecipientType.TO).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testRecipientName, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.TO, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].CC);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);
        Assert.Collection(smtpServer.ReceivedMessages[0].To, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(testRecipientName, recipient.DisplayName);
        });
    }

    [Fact]
    public async Task EmailService_AddRecipientCc_WithValidEmailAddress_WithRecipientName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();
        var testRecipientName = faker.Person.FullName;

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, testRecipientName, EmailRecipientType.CC).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testRecipientName, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.CC, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].To);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);
        Assert.Collection(smtpServer.ReceivedMessages[0].CC, recipient =>
        {
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(testRecipientName, recipient.DisplayName);
        });
    }

    [Fact]
    public async Task EmailService_AddRecipientBcc_WithValidEmailAddress_WithRecipientName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        var testEmailAddress = faker.Internet.Email();
        var testRecipientName = faker.Person.FullName;

        // Act...
        await emailBuilder.AddRecipient(testEmailAddress, testRecipientName, EmailRecipientType.BCC).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotEmpty(emailBuilder.Specification.To);
        Assert.Single(emailBuilder.Specification.To);
        Assert.Collection(emailBuilder.Specification.To, recipient =>
        {
            Assert.Equal(testRecipientName, recipient.Name);
            Assert.Equal(testEmailAddress, recipient.Address);
            Assert.Equal(EmailRecipientType.BCC, recipient.RecipientType);
        });

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Empty(smtpServer.ReceivedMessages[0].To);
        Assert.Empty(smtpServer.ReceivedMessages[0].CC);
        Assert.Empty(smtpServer.ReceivedMessages[0].Bcc);   // In SMTP, the `Bcc` field is not printed out to DATA like message headers (i.e, `TO`, `CC`). Any Bcc’ed recipient won’t be added to the recipients list.
    }

    [Fact]
    public async Task EmailService_NoRecipients_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.To.Clear();  // Use a specific test data for this test...

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(@"No recipients have been specified.", exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EmailService_SetBody_AsString(bool isHtml)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testBody = faker.Rant.Review();

        // Act...
        await emailBuilder.SetBody(testBody, isHtml).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.False(string.IsNullOrWhiteSpace(emailBuilder.Specification.Body));
        Assert.Equal(isHtml, emailBuilder.Specification.IsHtmlBody);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Equal(testBody, smtpServer.ReceivedMessages[0].Body);
        Assert.Equal(isHtml, smtpServer.ReceivedMessages[0].IsBodyHtml);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EmailService_SetBody_AsStringBuilder_Succeeds(bool isHtml)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testBody = new StringBuilder(faker.Rant.Review());

        // Act...
        await emailBuilder.SetBody(testBody, isHtml).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.False(string.IsNullOrWhiteSpace(emailBuilder.Specification.Body));
        Assert.Equal(isHtml, emailBuilder.Specification.IsHtmlBody);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Equal(testBody.ToString(), smtpServer.ReceivedMessages[0].Body);
        Assert.Equal(isHtml, smtpServer.ReceivedMessages[0].IsBodyHtml);
    }

    [Theory]
    [InlineData(@"", true)]
    [InlineData(@" ", true)]
    [InlineData(null, true)]
    [InlineData(@"", false)]
    [InlineData(@" ", false)]
    [InlineData(null, false)]
    public async Task EmailService_SetBody_AsString_WithNullOrEmptyOrWhiteSpace_Succeeds(string body, bool isHtml)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        await emailBuilder.SetBody(body, isHtml).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.True(string.IsNullOrWhiteSpace(emailBuilder.Specification.Body));
        Assert.Equal(isHtml, emailBuilder.Specification.IsHtmlBody);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.True(string.IsNullOrWhiteSpace(smtpServer.ReceivedMessages[0].Body));

        if (body == null)
        {
            Assert.False(smtpServer.ReceivedMessages[0].IsBodyHtml);    // If the `BODY` part in an SMTP message is `NULL`, then the HTML flag is always `FALSE`.
        }
        else
        {
            Assert.Equal(isHtml, smtpServer.ReceivedMessages[0].IsBodyHtml);
        }
    }

    [Theory]
    [InlineData(@"", true)]
    [InlineData(@" ", true)]
    [InlineData(null, true)]
    [InlineData(@"", false)]
    [InlineData(@" ", false)]
    [InlineData(null, false)]
    public async Task EmailService_SetBody_AsStringBuilder_WithNullOrEmptyOrWhiteSpace_Succeeds(string body, bool isHtml)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        await emailBuilder.SetBody(new StringBuilder(body), isHtml).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.True(string.IsNullOrWhiteSpace(emailBuilder.Specification.Body));
        Assert.Equal(isHtml, emailBuilder.Specification.IsHtmlBody);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.True(string.IsNullOrWhiteSpace(smtpServer.ReceivedMessages[0].Body));
        Assert.Equal(isHtml, smtpServer.ReceivedMessages[0].IsBodyHtml);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EmailService_SetBody_AsNullStringBuilder_Succeeds(bool isHtml)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        StringBuilder nullTestStringBuilderv = null;

        // Act...
        await emailBuilder.SetBody(nullTestStringBuilderv, isHtml).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.True(string.IsNullOrWhiteSpace(emailBuilder.Specification.Body));
        Assert.Equal(isHtml, emailBuilder.Specification.IsHtmlBody);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.True(string.IsNullOrWhiteSpace(smtpServer.ReceivedMessages[0].Body));
        Assert.False(smtpServer.ReceivedMessages[0].IsBodyHtml);    // If the `BODY` part in an SMTP message is `NULL`, then the HTML flag is always `FALSE`.
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public async Task EmailService_SetSender_WithEmailAddress_AsEmptyOrWhiteSpace_WithSenderName_AsNull_ThrowsException(string emailAddress)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(emailAddress, null).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithEmailAddress_AsNull_WithSenderName_AsNull_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(null, null).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"emailAddress", argumentNullException.ParamName);
    }

    [Fact]
    public async Task EmailService_NoSender_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        emailBuilder.Specification.From = null;

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(@"No sender has been specified.", exception.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public async Task EmailService_SetSender_WithOnlyEmptyOrWhiteSpaceEmailAddress_ThrowsException(string emailAddress)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(emailAddress).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithOnlyNullSpaceEmailAddress_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(null).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithValidEmailAddress_WithNullSenderName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testSenderEmailAddress = faker.Internet.Email();

        // Act...
        await emailBuilder.SetSender(testSenderEmailAddress, null).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.From);
        Assert.Equal(testSenderEmailAddress, emailBuilder.Specification.From.Name);
        Assert.Equal(testSenderEmailAddress, emailBuilder.Specification.From.Address);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.NotNull(smtpServer.ReceivedMessages[0].From);
        Assert.Equal(testSenderEmailAddress, smtpServer.ReceivedMessages[0].From.Address);
        Assert.Equal(testSenderEmailAddress, smtpServer.ReceivedMessages[0].From.DisplayName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithOnlyValidEmailAddress_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testSenderEmailAddress = faker.Internet.Email();

        // Act...
        await emailBuilder.SetSender(testSenderEmailAddress).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.From);
        Assert.Equal(testSenderEmailAddress, emailBuilder.Specification.From.Name);
        Assert.Equal(testSenderEmailAddress, emailBuilder.Specification.From.Address);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.NotNull(smtpServer.ReceivedMessages[0].From);
        Assert.Equal(testSenderEmailAddress, smtpServer.ReceivedMessages[0].From.Address);
        Assert.Equal(testSenderEmailAddress, smtpServer.ReceivedMessages[0].From.DisplayName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithValidEmailAddress_WithSenderName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testSenderEmailAddress = faker.Internet.Email();
        var testSenderName = faker.Person.FullName;

        // Act...
        await emailBuilder.SetSender(testSenderEmailAddress, testSenderName).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.From);
        Assert.Equal(testSenderName, emailBuilder.Specification.From.Name);
        Assert.Equal(testSenderEmailAddress, emailBuilder.Specification.From.Address);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.NotNull(smtpServer.ReceivedMessages[0].From);
        Assert.Equal(testSenderName, smtpServer.ReceivedMessages[0].From.DisplayName);
        Assert.Equal(testSenderEmailAddress, smtpServer.ReceivedMessages[0].From.Address);
    }

    [Fact]
    public async Task EmailService_SetSender_WithInvalidValue_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(faker.Internet.UserName()).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public async Task EmailService_SetSender_WithInvalidValue_WithSenderName_AsNullOrEmptyOrWhiteSpace_ThrowsException(string senderName)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(faker.Internet.UserName(), senderName).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Fact]
    public async Task EmailService_SetSender_WithInvalidValue_WithValidSenderName_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = await Record.ExceptionAsync(() => emailBuilder.SetSender(faker.Internet.UserName(), faker.Person.FullName).SendAsync(CancellationToken.None));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal(@"emailAddress", argumentException.ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public async Task EmailService_SetDefaultSender_WithSenderName_AsNullOrEmptyOrWhiteSpace_Succeeds(string senderName)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        await emailBuilder.SetDefaultSender(senderName).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.From);

        var smtpClientOptionsProvider = emailBuilder as ISmtpClientOptionsProvider;

        Assert.NotNull(smtpClientOptionsProvider);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, emailBuilder.Specification.From.Name);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, emailBuilder.Specification.From.Address);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.NotNull(smtpServer.ReceivedMessages[0].From);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, smtpServer.ReceivedMessages[0].From.DisplayName);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, smtpServer.ReceivedMessages[0].From.Address);
    }

    [Fact]
    public async Task EmailService_SetDefaultSender_WithValidSenderName_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testDefaultSenderName = faker.Person.FullName;

        // Act...
        await emailBuilder.SetDefaultSender(testDefaultSenderName).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.NotNull(emailBuilder.Specification.From);

        Assert.Equal(testDefaultSenderName, emailBuilder.Specification.From.Name);

        var smtpClientOptionsProvider = emailBuilder as ISmtpClientOptionsProvider;

        Assert.NotNull(smtpClientOptionsProvider);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, emailBuilder.Specification.From.Address);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.NotNull(smtpServer.ReceivedMessages[0].From);
        Assert.Equal(testDefaultSenderName, smtpServer.ReceivedMessages[0].From.DisplayName);
        Assert.Equal(smtpClientOptionsProvider.SmtpClientOptions.User, smtpServer.ReceivedMessages[0].From.Address);
    }

    [Fact]
    public void EmailService_SetSubject_AsNull_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        var exception = Record.Exception(() => emailBuilder.SetSubject(null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"subject", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    public async Task EmailService_SetSubject_AsEmptyOrWhiteSpace_Succeeds(string subject)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        // Act...
        await emailBuilder.SetSubject(subject).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.Equal(subject, emailBuilder.Specification.Subject);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Equal(subject.Trim(), smtpServer.ReceivedMessages[0].Subject);   // The SMTP protocol trims any whitespace string in the subject.
    }

    [Fact]
    public async Task EmailService_SetSubject_WithValidValue_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var emailProvider = BuildValidEmailProviderArrangement(faker);
        var emailBuilder = emailProvider.BeginSendEmail();
        BuildVaildEmailSpecificationArrangement(emailBuilder, faker);

        var testSubject = faker.Random.Words();

        // Act...
        await emailBuilder.SetSubject(testSubject).SendAsync(CancellationToken.None);

        // Assert...
        Assert.NotNull(emailBuilder.Specification);
        Assert.Equal(testSubject, emailBuilder.Specification.Subject);

        // Assert mail message at server...
        Assert.Equal(1, smtpServer.ReceivedMessagesCount);
        Assert.Equal(testSubject, smtpServer.ReceivedMessages[0].Subject);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && smtpServer != null)
        {
            smtpServer.Dispose();
            smtpServer = null;
        }
    }

    private static byte[] BuildValidAttachment(Faker faker)
    {
        return faker.Random.Bytes(faker.Random.Int(1, 100));
    }

    private static void BuildVaildEmailSpecificationArrangement(IEmailBuilder emailBuilder, Faker faker)
    {
        emailBuilder.Specification.Body = faker.Random.Words();
        emailBuilder.Specification.IsHtmlBody = faker.Random.Bool();
        emailBuilder.Specification.Subject = faker.Random.Words();
        emailBuilder.Specification.From = new EmailSenderSpecification()
        {
            Address = faker.Internet.Email(),
            Name = faker.Person.FullName,
        };
        emailBuilder.Specification.To.Add(new EmailRecipientSpecification()
        {
            Address = faker.Internet.Email(),
            Name = faker.Person.FullName,
            RecipientType = faker.Random.Enum<EmailRecipientType>(),
        });
    }

    private IEmailProvider BuildValidEmailProviderArrangement(Faker faker)
    {
        var mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        mockOptions.Setup(o => o.Value).Returns(BuildValidSmtpClientOptions(faker));

        return new EmailService(mockOptions.Object);
    }

    private SmtpClientOptions BuildValidSmtpClientOptions(Faker faker)
    {
        return new SmtpClientOptions()
        {
            Host = smtpServer.Configuration.Domain,
            Port = smtpServer.Configuration.Port,
            Password = faker.Internet.Password(),
            User = faker.Internet.Email(),

            // For test porposes, SSL should not be used. Otherwise, a `MailKit.Security.SslHandshakeException` might be thrown...
            UseSSL = false,
        };
    }
}