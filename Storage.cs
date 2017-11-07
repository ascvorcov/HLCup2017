using System;
using System.Collections.Generic;

public class Storage
{
  private static Lazy<Storage> instance = new Lazy<Storage>();
  private UserModel[] userModel = new UserModel[1100000];
  private LocationModel[] locationModel = new LocationModel[1100000];
  private VisitModel[] visitModel = new VisitModel[11000000];
  public static Storage Instance => instance.Value;
  
  public DateTime Timestamp {get;set;}
  public UserModel GetUser(uint id) => id < this.userModel.Length ? this.userModel[id] : null;
  public void AddUser(UserModel model) => this.userModel[(uint)model.id] = model;
  public LocationModel GetLocation(uint id) => id < this.locationModel.Length ? this.locationModel[id] : null;
  public void AddLocation(LocationModel model) => this.locationModel[(uint)model.id] = model;
  public VisitModel GetVisit(uint id) => id < this.visitModel.Length ? this.visitModel[id] : null;
  public void AddVisit(VisitModel model) => this.visitModel[(uint)model.id] = model.Init(this);
}