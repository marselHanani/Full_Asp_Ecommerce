using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Xunit;

namespace Ecommerce.Tests.Services;

public class CategoriesIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllCategories_ReturnsOk()
    {
        var mockFactory = new Mock<WebApplicationFactory<Program>>();
        mockFactory.Setup(f => f.CreateClient()).Returns(_client);

        var response = await mockFactory.Object.CreateClient().GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsOkOrNotFound()
    {
        var categoryId = "some-guid";
        var mockFactory = new Mock<WebApplicationFactory<Program>>();
        mockFactory.Setup(f => f.CreateClient()).Returns(_client);

        var response = await mockFactory.Object.CreateClient().GetAsync($"/api/categories/{categoryId}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubCategoriesByCategoryId_ReturnsOk()
    {
        var categoryId = "some-guid";
        var mockFactory = new Mock<WebApplicationFactory<Program>>();
        mockFactory.Setup(f => f.CreateClient()).Returns(_client);

        var response = await mockFactory.Object.CreateClient().GetAsync($"/api/categories/{categoryId}/subcategories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetParentCategories_ReturnsOk()
    {
        var mockFactory = new Mock<WebApplicationFactory<Program>>();
        mockFactory.Setup(f => f.CreateClient()).Returns(_client);

        var response = await mockFactory.Object.CreateClient().GetAsync("/api/categories/parents");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSubCategories_ReturnsOk()
    {
        var mockFactory = new Mock<WebApplicationFactory<Program>>();
        mockFactory.Setup(f => f.CreateClient()).Returns(_client);

        var response = await mockFactory.Object.CreateClient().GetAsync("/api/categories/subs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}