using System;

public class VisitModel
{
  public uint? id;

  public uint? location = uint.MaxValue;

  public uint? user = uint.MaxValue;

  public int? visited_at = int.MaxValue;

  public uint? mark = uint.MaxValue;

  public DateTime VisitedAt()
  {
    return DateTimeOffset.FromUnixTimeSeconds(visited_at.Value).UtcDateTime;
  }

  public bool IsValidModel()
  {
    return 
    this.location != null && 
    this.user != null && 
    this.visited_at != null && 
    this.mark != null;
  }  

  public VisitModel Init(Storage storage)
  {
    storage.GetUser(user.Value).AddVisit(this);
    storage.GetLocation(location.Value).AddVisit(this);
    return this;
  }

  public void UpdateFrom(VisitModel that, Storage storage)
  {
    if (that.location != uint.MaxValue && this.location != that.location)
    {
        storage.GetLocation(this.location.Value).RemoveVisit(this);
        storage.GetLocation(that.location.Value).AddVisit(this);
        this.location = that.location;
    }

    if (that.user != uint.MaxValue && this.user != that.user)
    {
        storage.GetUser(this.user.Value).RemoveVisit(this);
        storage.GetUser(that.user.Value).AddVisit(this);
        this.user = that.user;
    }

    if (that.mark != uint.MaxValue && that.mark != this.mark)
    {
        this.mark = that.mark;
    }

    if (that.visited_at != int.MaxValue && that.visited_at != this.visited_at)
    {
        this.visited_at = that.visited_at;
    }
  }  
}