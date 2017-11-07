using System;
using System.Collections.Generic;
using System.Linq;

public class LocationModel
{
  public uint? id;

  public string place = Const.Empty;

  public string country = Const.Empty;

  public string city = Const.Empty;

  public uint? distance = uint.MaxValue;

  private List<VisitModel> visits;

  public bool IsValidModel()
  {
    return 
    this.place != null && 
    this.country != null && 
    this.city != null && 
    this.distance != null;
  }

  public void UpdateFrom(LocationModel other)
  {
    this.place = other.place == Const.Empty ? this.place : other.place;
    this.country = other.country == Const.Empty ? this.country : other.country;
    this.city = other.city == Const.Empty ? this.city : other.city;
    this.distance = other.distance == uint.MaxValue ? this.distance : other.distance;
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

  public decimal GetScore(
    Storage storage,
    uint? fromDate, 
    uint? toDate, 
    uint? fromAge, 
    uint? toAge, 
    char? gender)
  {
    return this
     .Filter(storage, fromDate, toDate, fromAge, toAge, gender)
     .Select(x => (decimal)x.mark.Value)
     .DefaultIfEmpty(0)
     .Average();
  }

  private IEnumerable<VisitModel> Filter(
    Storage storage,
    uint? fromDate, 
    uint? toDate, 
    uint? fromAge, 
    uint? toAge, 
    char? gender)
    {
      var now = storage.Timestamp;
      if (this.visits == null)
      {
        yield break;
      }
  
      var birthFrom = now.AddYears(-(int)(fromAge.GetValueOrDefault()));
      var birthTo = now.AddYears(-(int)(toAge.GetValueOrDefault()));

      foreach (var model in this.visits)
      {
        if (fromDate != null && model.visited_at <= fromDate.Value) continue;
        if (toDate != null && model.visited_at >= toDate.Value) continue;

        if (fromAge != null || toAge != null || gender != null)
        {
          var user = storage.GetUser(model.user.Value);
          if (gender != null && user.gender != gender) continue;

          var bd = user.BirthDate();
          if (fromAge != null && birthFrom <= bd) continue;
          if (toAge != null && birthTo >= bd) continue;
        }

        yield return model;
      }
    }
}