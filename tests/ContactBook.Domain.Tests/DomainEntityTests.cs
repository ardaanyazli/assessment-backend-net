using ContactBook.Contacts.Domain.Entities;
using ContactBook.Reports.Domain.Entities;
using FluentAssertions;

namespace ContactBook.Domain.Tests.Entities;

public class ContactTests
{
    [Fact]
    public void Contact_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        // Assert
        contact.Id.Should().NotBe(Guid.Empty);
        contact.FirstName.Should().Be("John");
        contact.LastName.Should().Be("Doe");
        contact.ContactInfos.Should().BeNull();
    }

    [Fact]
    public void Contact_CanHaveContactInfos()
    {
        // Arrange
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith"
        };

        var contactInfos = new List<ContactInfo>
        {
            new() { Id = Guid.NewGuid(), InfoType = ContactInfoType.Email, Value = "jane@example.com", IsDefault = true },
            new() { Id = Guid.NewGuid(), InfoType = ContactInfoType.Phone, Value = "123-456-7890", IsDefault = false }
        };

        // Act
        contact.ContactInfos = contactInfos;

        // Assert
        contact.ContactInfos.Should().HaveCount(2);
        contact.ContactInfos.Should().Contain(ci => ci.InfoType == ContactInfoType.Email);
        contact.ContactInfos.Should().Contain(ci => ci.InfoType == ContactInfoType.Phone);
    }
}

public class ContactInfoTests
{
    [Fact]
    public void ContactInfo_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var contactInfo = new ContactInfo
        {
            Id = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            InfoType = ContactInfoType.Email,
            Value = "test@example.com",
            IsDefault = true
        };

        // Assert
        contactInfo.Id.Should().NotBe(Guid.Empty);
        contactInfo.ContactId.Should().NotBe(Guid.Empty);
        contactInfo.InfoType.Should().Be(ContactInfoType.Email);
        contactInfo.Value.Should().Be("test@example.com");
        contactInfo.IsDefault.Should().BeTrue();
    }

    [Theory]
    [InlineData(ContactInfoType.Phone)]
    [InlineData(ContactInfoType.Email)]
    [InlineData(ContactInfoType.Location)]
    public void ContactInfo_CanHaveDifferentInfoTypes(ContactInfoType infoType)
    {
        // Arrange & Act
        var contactInfo = new ContactInfo
        {
            Id = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            InfoType = infoType,
            Value = "test value",
            IsDefault = false
        };

        // Assert
        contactInfo.InfoType.Should().Be(infoType);
    }
}

public class ContactInfoTypeTests
{
    [Fact]
    public void ContactInfoType_HasCorrectValues()
    {
        // Assert
        ContactInfoType.Phone.Should().Be((ContactInfoType)1);
        ContactInfoType.Email.Should().Be((ContactInfoType)2);
        ContactInfoType.Location.Should().Be((ContactInfoType)3);
    }

    [Fact]
    public void ContactInfoType_CanBeConvertedToInt()
    {
        // Act & Assert
        ((int)ContactInfoType.Phone).Should().Be(1);
        ((int)ContactInfoType.Email).Should().Be(2);
        ((int)ContactInfoType.Location).Should().Be(3);
    }
}

public class ReportTests
{
    [Fact]
    public void Report_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var report = new Report
        {
            Id = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Requested
        };

        // Assert
        report.Id.Should().NotBe(Guid.Empty);
        report.RequestedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        report.Status.Should().Be(ReportStatus.Requested);
        report.Data.Should().BeNull();
    }

    [Fact]
    public void Report_CanHaveLocationStatistics()
    {
        // Arrange
        var report = new Report
        {
            Id = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Completed
        };

        var locationStats = new List<LocationStatistics>
        {
            new("Istanbul", 10, 8),
            new("Ankara", 5, 3)
        };

        // Act
        report.Data = locationStats;

        // Assert
        report.Data.Should().HaveCount(2);
        report.Data.Should().Contain(ls => ls.Location == "Istanbul" && ls.ContactCount == 10 && ls.PhoneCount == 8);
        report.Data.Should().Contain(ls => ls.Location == "Ankara" && ls.ContactCount == 5 && ls.PhoneCount == 3);
    }
}

public class LocationStatisticsTests
{
    [Fact]
    public void LocationStatistics_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var locationStats = new LocationStatistics("Istanbul", 15, 12);

        // Assert
        locationStats.Location.Should().Be("Istanbul");
        locationStats.ContactCount.Should().Be(15);
        locationStats.PhoneCount.Should().Be(12);
    }

    [Fact]
    public void LocationStatistics_IsRecord_SupportsValueEquality()
    {
        // Arrange
        var stats1 = new LocationStatistics("Istanbul", 10, 8);
        var stats2 = new LocationStatistics("Istanbul", 10, 8);
        var stats3 = new LocationStatistics("Ankara", 10, 8);

        // Assert
        stats1.Should().Be(stats2);
        stats1.Should().NotBe(stats3);
    }
}

public class ReportStatusTests
{
    [Fact]
    public void ReportStatus_HasCorrectValues()
    {
        // Assert
        ReportStatus.Requested.Should().Be((ReportStatus)0);
        ReportStatus.InProgress.Should().Be((ReportStatus)1);
        ReportStatus.Completed.Should().Be((ReportStatus)2);
    }

    [Theory]
    [InlineData(ReportStatus.Requested)]
    [InlineData(ReportStatus.InProgress)]
    [InlineData(ReportStatus.Completed)]
    public void ReportStatus_CanBeUsedInReport(ReportStatus status)
    {
        // Arrange & Act
        var report = new Report
        {
            Id = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = status
        };

        // Assert
        report.Status.Should().Be(status);
    }
}