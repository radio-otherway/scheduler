using Microsoft.AspNetCore.Mvc;

namespace OtherWay.Radio.Scheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class InterfaceController : ControllerBase {
  private readonly ILogger<InterfaceController> _logger;

  public InterfaceController(ILogger<InterfaceController> logger) {
    _logger = logger;
  }
}