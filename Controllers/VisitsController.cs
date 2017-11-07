using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;

[Produces("application/json")]
public class VisitsController : Controller
{
    private Storage storage = Storage.Instance;

    [HttpPost("/visits/new")]
    public IActionResult CreateVisit([FromBody] VisitModel visit)
    {
        if (visit == null) return BadRequest();
        if (!visit.IsValidModel()) return BadRequest();
        if (visit.id == null) return BadRequest();
        if (this.storage.GetVisit(visit.id.Value) != null) return BadRequest();
        this.storage.AddVisit(visit);
        return this.Json(Const.None);
    }

    [HttpPost("/visits/{id}")]
    public IActionResult UpdateVisit(uint id, [FromBody] VisitModel newv)
    {
        if (newv == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest();
        if (!newv.IsValidModel()) return BadRequest();
        if (newv.id != null) return BadRequest();

        var oldv = this.storage.GetVisit(id);
        if (oldv == null) return NotFound();

        oldv.UpdateFrom(newv, this.storage);
        return this.Json(Const.None);
    }
    
    [HttpGet("/visits/{id}")]
    public IActionResult GetVisit(uint id)
    {
        var model = this.storage.GetVisit(id);
        if (model == null) return NotFound();
        return Ok(model);
    }
}
