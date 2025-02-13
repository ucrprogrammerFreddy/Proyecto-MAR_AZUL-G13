using AppUsuarios.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class HomeControllerTest
{
    private Mock<ILogger<HomeController>> _mockLogger;
    private HomeController _controller;

    [TestInitialize]
    public void Setup()
    {
        // Crear un Mock de ILogger<HomeController>
        _mockLogger = new Mock<ILogger<HomeController>>();

        // Pasar el mock al controlador
        _controller = new HomeController(_mockLogger.Object);
    }

    [TestMethod]
    public void Index_ReturnsViewResult()
    {
        // Act: Llamar al método Index()
        var result = _controller.Index();

        // Assert: Verificar que el resultado es una vista
        Assert.IsInstanceOfType(result, typeof(ViewResult));

        // Verificar que el logger fue llamado con cierto mensaje
        _mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information), // Nivel de log esperado
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, t) => state.ToString().Contains("Cargando la página de inicio.")), // Mensaje esperado
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once // Se espera que el logger se llame una sola vez
        );
    }
}
