using System;
using System.Collections.Generic;
using System.Linq;

public class UserModel
{
  public uint? id;
  public string email = Const.Empty;
  public string first_name = Const.Empty;
  public string last_name = Const.Empty;
  public char? gender = Const.Empty[0];
  public int? birth_date = int.MaxValue;

  private List<VisitModel> visits;

  public bool IsValidModel()
  {
    return 
    this.email != null && 
    this.first_name != null && 
    this.last_name != null && 
    this.gender != null && 
    this.birth_date != null;
  }

  public DateTime BirthDate()
  {
    return DateTimeOffset.FromUnixTimeSeconds(birth_date.Value).UtcDateTime;
  }

  public void UpdateFrom(UserModel other)
  {
    this.email = other.email == Const.Empty ? this.email : other.email;
    this.first_name = other.first_name == Const.Empty ? this.first_name : other.first_name;
    this.last_name = other.last_name == Const.Empty ? this.last_name : other.last_name;
    this.gender = other.gender == Const.Empty[0] ? this.gender : other.gender;
    this.birth_date = other.birth_date == int.MaxValue ? this.birth_date : other.birth_date;
  }

  public void AddVisit(VisitModel visit)
  {
    if (this.visits == null) this.visits = new List<VisitModel>();
    this.visits.Add(visit);
  }

  public void RemoveVisit(VisitModel visit)
  {
    if (this.visits == null) return;
    this.visits.Remove(visit);
  }
  
  public List<VisitModelClient> GetVisits(
    Storage storage,
    uint? fromDate,
    uint? toDate,
    uint? distance,
    string country)
  {
    if (this.visits == null) return new List<VisitModelClient>();
    return this
     .Filter(storage, fromDate, toDate, distance, country)
     .OrderBy(x => x.visited_at)
     .ToList();
  }

  private IEnumerable<VisitModelClient> Filter(
    Storage storage,
    uint? fromDate, 
    uint? toDate, 
    uint? distance, 
    string country)
    {
      foreach (var model in this.visits)
      {
        var location = storage.GetLocation(model.location.Value);
        if (fromDate != null && model.visited_at <= fromDate.Value) continue;
        if (toDate != null && model.visited_at >= toDate.Value) continue;
        if (distance != null && location.distance >= distance.Value) continue;
        if (country != null && location.country != country) continue;

        yield return new VisitModelClient
        {
          place = location.place,
          visited_at = model.visited_at.Value,
          mark = model.mark.Value
        };
      }
    }
}