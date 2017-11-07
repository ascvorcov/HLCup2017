using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

[Produces("application/json")]
public class UsersController : Controller
{
    private Storage storage = Storage.Instance;

    [HttpPost("/users/new")]
    public IActionResult CreateUser([FromBody] UserModel user)
    {
        if (user == null) return BadRequest();
        if (!user.IsValidModel()) return BadRequest();
        if (user.id == null) return BadRequest();
        if (this.storage.GetUser(user.id.Value) != null) return BadRequest();
        this.storage.AddUser(user);
        return this.Json(Const.None);
    }

    [HttpPost("/users/{id}")]
    public IActionResult UpdateUser(uint id, [FromBody] UserModel user)
    {
        if (user == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest();
        if (!user.IsValidModel()) return BadRequest();
        if (user.id != null) return BadRequest();
        var model = this.storage.GetUser(id);
        if (model == null) return NotFound();
        model.UpdateFrom(user);
        return this.Json(Const.None);
    }
    
    [HttpGet("/users/broken")]
    public IActionResult GetBroken()
    {
        return NotFound();
    }

    [HttpGet("/users/{id}")]
    public IActionResult GetUser(uint id)
    {
        var model = this.storage.GetUser(id);
        if (model == null) return NotFound();
        return Ok(model);
    }

    [HttpGet("/users/{id}/visits")]
    public IActionResult GetUserVisits(
        uint id, 
        uint? fromDate = null, 
        uint? toDate = null,
        uint? toDistance = null,
        string country = null)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var user = this.storage.GetUser(id);
        if (user == null)
            return NotFound();

        return Ok(new { visits = user.GetVisits(this.storage, fromDate, toDate, toDistance, country) });
    }
}
