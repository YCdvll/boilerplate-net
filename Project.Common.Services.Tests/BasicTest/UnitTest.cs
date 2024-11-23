using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Models;

namespace Project.Common.Services.Tests.BasicTest;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public async Task GetEmailTemplate()
    {
        var blobStorage = new BlobStorageService(
            "storage url");
        var fileContent = await blobStorage.GetFileAsync("mail/template-link-mail.html", ContainerName.Public, CancellationToken.None);
        var stream = new StreamReader(new MemoryStream(fileContent));
        var emailTemplate = await stream.ReadToEndAsync();
        var content = "Bonjour, Merci Au revoir";
        var contentResult = emailTemplate.Replace("{{Content}}", content);
    }
}