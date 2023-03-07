using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace OtherWay.Radio.Scheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase {
  [HttpGet("version")]
  public IActionResult GetVersion() {
    var assembly = Assembly.GetExecutingAssembly();
    var versions = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
    return Ok(new {
      FullName = assembly.FullName,
      AssemblyVersion = assembly.GetName().Version,
      AssemblyFileVersion = versions.FileVersion,
      AssemblyInformationalVersion = versions.ProductVersion
    });
  }
}