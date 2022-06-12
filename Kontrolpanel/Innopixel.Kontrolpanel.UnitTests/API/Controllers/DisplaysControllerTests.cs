using Innopixel.Kontrolpanel.API.Controllers;
using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Innopixel.Kontrolpanel.UnitTests.API.Controllers;

public class DisplaysControllerTests
{
	private readonly Mock<IRepositoryAPI<Display>> mockRepository;

	public DisplaysControllerTests()
	{
		mockRepository = new Mock<IRepositoryAPI<Display>>();
	}

	[Theory]
	[InlineData(1, "Navn 1", "Beskrivelse 2")]
	[InlineData(10, "Navn 10", "Beskrivelse 10")]
	[InlineData(20, null, null)]
	[InlineData(1, null, null)]
	[InlineData(int.MaxValue, null, null)]
	public async Task GetDisplayByIdAsync_Should_Return_Specific_Object_Theory(int id, string name, string description)
	{
		// Arrange
		Display? expectedDisplay = new() { Id = id, Name = name, Description = description };
		mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(expectedDisplay);
		DisplaysController? controller = new(mockRepository.Object);

		// Act
		ActionResult<Display>? actionResult = await controller.GetDisplayByIdAsync(id);
		Display? actualDisplay = (Display?)((actionResult.Result as OkObjectResult)?.Value);

		// Assert
		Assert.Equal(expectedDisplay, actualDisplay);
	}
}