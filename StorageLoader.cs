using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using  Newtonsoft.Json;

public class StorageLoader
{
  private readonly Storage storage;
  public StorageLoader(Storage storage)
  {
    this.storage = storage;
  }

  private void DeserializeUsers(string json)
  {
    var wrapper = JsonConvert.DeserializeObject<Users>(json);
    foreach (var u in wrapper.users)
      this.storage.AddUser(u);
  }
  private void DeserializeLocations(string json)
  {
    var wrapper = JsonConvert.DeserializeObject<Locations>(json);
    foreach (var l in wrapper.locations)
      this.storage.AddLocation(l);
  }

  private void DeserializeVisits(string json)
  {
    var wrapper = JsonConvert.DeserializeObject<Visits>(json);
    foreach (var v in wrapper.visits)
      this.storage.AddVisit(v);
  }

  private Task Deserialize(IEnumerable<string> files, Action<string> worker)
  {
    List<Task> pending = new List<Task>();
    foreach (var file in files)
    {
      using (var reader = new StreamReader(file))
      {
        var json = reader.ReadToEnd();
        pending.Add(Task.Run(() => worker(json)));
      }
    }

    return Task.WhenAll(pending);
  }
  public void Load(string dataPath)
  {
    var optionsPath = Path.Combine(dataPath, "options.txt");
    if (!File.Exists(optionsPath)) return;

    var ts = int.Parse(File.ReadLines(optionsPath).First());
    this.storage.Timestamp = DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;
    
    Console.WriteLine("loading users");
    var userFiles = Directory.EnumerateFiles(dataPath, "user*.json");
    Deserialize(userFiles, DeserializeUsers).Wait();

    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine("loading locations");
    var locationFiles = Directory.EnumerateFiles(dataPath, "location*.json");
    Deserialize(locationFiles, DeserializeLocations).Wait();

    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine("loading visits 1");
    var visitFiles = Directory.EnumerateFiles(dataPath, "visit*.json").ToList();
    Deserialize(visitFiles.Take(50), DeserializeVisits).Wait();

    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine("loading visits 2");
    Deserialize(visitFiles.Skip(50), DeserializeVisits).Wait();

    GC.Collect();
    GC.WaitForPendingFinalizers();
  }

  public class Locations { public LocationModel[] locations; }
  public class Users { public UserModel[] users; }
  public class Visits { public VisitModel[] visits; }
}