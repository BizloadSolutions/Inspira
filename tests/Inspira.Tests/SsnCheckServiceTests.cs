using System.Threading.Tasks;
using Xunit;
using Moq;
using Inspira.Application.Services;
using Inspira.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Inspira.Domain.Entities;

namespace Inspira.Tests;

public class SsnCheckServiceTests
{
    [Fact]
    public async Task SsnCheckAsync_CallsMtcAndUpdatesSubmissionProperty_WhenOwnerAndContactFound()
    {
        // Arrange
        var submissionRepo = new Mock<ISubmissionRepository>();
        var submissionPropertyRepo = new Mock<ISubmissionPropertyRepository>();
        var mtcClient = new Mock<IMtcSoapClient>();
        var logger = new Mock<ILogger<SsnCheckService>>();

        var submission = new Submission { SubmissionId = 1, SubmissionPropertyId = 10, FormId = 1 };
        submissionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(submission);

        var sp = new SubmissionProperty { SubmissionPropertyId = 10, OwnerTaxId = "123456789", OwnerContactId = "0" };
        submissionPropertyRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(sp);

        mtcClient.Setup(c => c.SsnInternalCheckAsync("123456789", 1)).ReturnsAsync(555);

        var svc = new SsnCheckService(submissionRepo.Object, submissionPropertyRepo.Object, mtcClient.Object, logger.Object);

        // Act
        var result = await svc.SsnCheckAsync(1, "123456789", "Owner");

        // Assert
        Assert.Equal("true", result.Result);
        Assert.Equal(555, result.ContactId);
        submissionPropertyRepo.Verify(r => r.UpdateAsync(It.Is<SubmissionProperty>(p => p.SubmissionPropertyId == 10 && p.OwnerContactId == "555")), Times.Once);
    }

    [Fact]
    public async Task SsnCheckAsync_UsesOwnerTaxId_WhenRedacted()
    {
        // Arrange
        var submissionRepo = new Mock<ISubmissionRepository>();
        var submissionPropertyRepo = new Mock<ISubmissionPropertyRepository>();
        var mtcClient = new Mock<IMtcSoapClient>();
        var logger = new Mock<ILogger<SsnCheckService>>();

        var submission = new Submission { SubmissionId = 2, SubmissionPropertyId = 20, FormId = 1 };
        submissionRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(submission);

        var sp = new SubmissionProperty { SubmissionPropertyId = 20, OwnerTaxId = "987654321", OwnerContactId = "0" };
        submissionPropertyRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(sp);

        // redacted ssn length 4 should trigger lookup and role "NotOwner" => roleInt 0
        mtcClient.Setup(c => c.SsnInternalCheckAsync("987654321", 0)).ReturnsAsync(0);

        var svc = new SsnCheckService(submissionRepo.Object, submissionPropertyRepo.Object, mtcClient.Object, logger.Object);

        // Act
        var result = await svc.SsnCheckAsync(2, "••••", "NotOwner");

        // Assert: ensure it called mtc with resolved tax id
        mtcClient.Verify(c => c.SsnInternalCheckAsync("987654321", 0), Times.Once);
        Assert.Equal("false", result.Result);
        Assert.Equal(0, result.ContactId);
    }

    [Fact]
    public async Task SsnCheckAsync_HandlesMtcFault_AndReturnsError()
    {
        // Arrange
        var submissionRepo = new Mock<ISubmissionRepository>();
        var submissionPropertyRepo = new Mock<ISubmissionPropertyRepository>();
        var mtcClient = new Mock<IMtcSoapClient>();
        var logger = new Mock<ILogger<SsnCheckService>>();

        var submission = new Submission { SubmissionId = 3, SubmissionPropertyId = 30, FormId = 1 };
        submissionRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(submission);

        var sp = new SubmissionProperty { SubmissionPropertyId = 30, OwnerTaxId = "000000000", OwnerContactId = "0" };
        submissionPropertyRepo.Setup(r => r.GetByIdAsync(30)).ReturnsAsync(sp);

        mtcClient.Setup(c => c.SsnInternalCheckAsync("000000000", 1)).ThrowsAsync(new System.Exception("Multiple active contact master records found"));

        var svc = new SsnCheckService(submissionRepo.Object, submissionPropertyRepo.Object, mtcClient.Object, logger.Object);

        // Act
        var result = await svc.SsnCheckAsync(3, "0000", "Owner");

        // Assert
        Assert.Equal("error", result.Result);
        Assert.Equal(0, result.ContactId);
    }

    [Fact]
    public async Task SsnCheckAsync_ReturnsError_WhenSubmissionNotFound()
    {
        // Arrange
        var submissionRepo = new Mock<ISubmissionRepository>();
        var submissionPropertyRepo = new Mock<ISubmissionPropertyRepository>();
        var mtcClient = new Mock<IMtcSoapClient>();
        var logger = new Mock<ILogger<SsnCheckService>>();

        submissionRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Submission?)null);

        var svc = new SsnCheckService(submissionRepo.Object, submissionPropertyRepo.Object, mtcClient.Object, logger.Object);

        // Act
        var result = await svc.SsnCheckAsync(42, "1234", "Owner");

        // Assert
        Assert.Equal("error", result.Result);
        Assert.Equal(0, result.ContactId);
    }
}
