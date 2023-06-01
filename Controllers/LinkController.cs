using Microsoft.AspNetCore.Mvc;

namespace NHME_Apps.Controllers;

public class LinkController : Controller
{

    private readonly IConfiguration configuration;

    public LinkController(IConfiguration iConfig)
    {
        configuration = iConfig;
    }

    [HttpGet]
    public IActionResult GetSistemas()
    {

        //string Sistema = configuration.GetSection("Sistemas").Value;

        var list = new List<string>();

        return Ok(list);
    }

}