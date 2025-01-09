using Microsoft.AspNetCore.Mvc;

namespace webchat.api.Controller;

public class FallbackController : ControllerBase
{
    public IActionResult Index() =>
        PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
}