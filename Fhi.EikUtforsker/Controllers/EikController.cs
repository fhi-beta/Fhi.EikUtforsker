using Fhi.EikUtforsker.Tjenester.Analyse;
using Fhi.EikUtforsker.Tjenester.Dekryptering;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Fhi.EikUtforsker.Tjenester.WebDav;

namespace Fhi.EikUtforsker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EikController : ControllerBase
{
    private readonly WebDavTjeneste _service;
    private readonly Dekrypteringstjeneste _dekrypteringstjeneste;
    private readonly Analysetjeneste _analysetjeneste;
    private readonly NyesteElementerTjeneste _nyesteElementerTjeneste;

    public EikController(WebDavTjeneste service, Dekrypteringstjeneste dekrypteringstjeneste, Analysetjeneste analysetjeneste, NyesteElementerTjeneste nyesteElementerTjeneste)
    {
        _service = service;
        _dekrypteringstjeneste = dekrypteringstjeneste;
        _analysetjeneste = analysetjeneste;
        _nyesteElementerTjeneste = nyesteElementerTjeneste;
    }

    [HttpGet]
    [Route("nyesteFiler")]
    public async Task<ActionResult<List<WebDavResourceNode>>> GetNyesteFiler(string uri = "/remote.php/webdav/", int antall=100)
    {
        var resources = await _nyesteElementerTjeneste.HentDeNyeste(antall, uri);
        return Ok(resources);
    }

    [HttpGet]
    [Route("mappe")]
    public async Task<ActionResult<List<WebDavResourceNode>>> GetResources(string uri = "/remote.php/webdav/")
    {
        var resources = await _service.GetResources(uri);
        return Ok(resources);
    }

    [HttpGet]
    [Route("file")]
    public async Task<ActionResult<string>> GetFile(string uri, CancellationToken cancellationToken)
    {
        var file = await _service.GetFile(uri, cancellationToken);

        if (file == null)
        {
            return NotFound();
        }
        return Ok(file);
    }

    [HttpGet]
    [Route("analyse")]
    public async Task<ActionResult<Dekrypteringsanalyse>> Analyser(string uri, CancellationToken cancellationToken)
    {
        var file = await _service.GetFile(uri, cancellationToken);

        if (file == null)
        {
            return NotFound();
        }

        var analyse = _analysetjeneste.Analyser(file, uri);
        return Ok(analyse);
    }

    [HttpGet]
    [Route("dekrypter")]
    public async Task<ActionResult<string>> Dekrypter(string uri, string skjemanavn, CancellationToken cancellationToken)
    {
        var file = await _service.GetFile(uri, cancellationToken);

        if (file == null)
        {
            return NotFound();
        }
        var (feilmelding, dekryptert) = _dekrypteringstjeneste.Dekrypter(file, skjemanavn);

        if (!string.IsNullOrEmpty(feilmelding))
        {
            return Problem(statusCode: 500, title: "Dekryptering feilet", detail: feilmelding);
        }
        return Ok(dekryptert);
    }
}