using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;

[Produces("application/json")]
public class LocationController : Controller
{
    private Storage storage = Storage.Instance; //todo:inject

    [HttpPost("/locations/new")]
    public IActionResult CreateLocation([FromBody] LocationModel location)
    {
        if (location == null) return BadRequest();
        if (!location.IsValidModel()) return BadRequest();
        if (location.id == null) return BadRequest();
        if (this.storage.GetLocation(location.id.Value) != null) return BadRequest();
        this.storage.AddLocation(location);
        return this.Json(Const.None);
    }

    [HttpPost("/locations/{id}")]
    public IActionResult UpdateLocation(uint id, [FromBody] LocationModel location)
    {
        if (location == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest();
        if (!location.IsValidModel()) return BadRequest();
        if (location.id != null) return BadRequest();
        var model = this.storage.GetLocation(id);
        if (model == null) return NotFound();
        model.UpdateFrom(location);
        return this.Json(Const.None);
    }
    
    [HttpGet("/locations/{id}")]
    public IActionResult GetLocation(uint id)
    {
        var model = this.storage.GetLocation(id);
        if (model == null) return NotFound();
        return Ok(model);
    }

    [HttpGet("/locations/{id}/avg")]
    public IActionResult GetAverageLocationScore(
        uint id,
        uint? fromDate = null,
        uint? toDate = null,
        uint? fromAge = null,
        uint? toAge = null,
        char? gender = null)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var location = this.storage.GetLocation(id);
        if (location == null)
            return NotFound();
        var avg = location.GetScore(this.storage, fromDate, toDate, fromAge, toAge, gender);
        return Ok(new { avg = Math.Round(avg, 5) });
    }
}
