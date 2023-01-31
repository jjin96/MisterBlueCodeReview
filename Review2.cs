public IEnumerable<VehicleJson> SetUms(string I_SSTCSTSGCD)
{
    Middle<VehicleJson> results = new Middle<VehicleJson>();


    results = repository.ReadForJson();

    if (results.IsSuccess)
    {
        if (string.IsNullOrEmpty(I_SSTCSTSGCD))
        {
            results.QueryableList = results.QueryableList.Where(w => !string.IsNullOrEmpty(w.UseYN) && w.UseYN.Equals("Y") && !w.I_PSYN.Equals("Y") && !w.I_SSTCSTSGCD.Equals("03"));
        }
        else
        {
            results.QueryableList = results.QueryableList.Where(w => w.I_SSTCSTSGCD.Equals(I_SSTCSTSGCD) && !string.IsNullOrEmpty(w.UseYN) && w.UseYN.Equals("Y") && !w.I_PSYN.Equals("Y") && !w.I_SSTCSTSGCD.Equals("03"));
        }
        
    }

    //results.List = results.QueryableList.ToList();
    List<VehicleJson> vehicles = results.QueryableList.ToList();
    string[] ids = vehicles.Select(s => s.AttachmentIdentity).ToArray();

    IQueryable<Attachment> tmp = repository.db.Attachments.Where(w => w.ModelName.Equals("Offer") && w.Kind.Equals("CkeditorImage") && w.IsDefault == true && ids.Contains(w.Identity) );

    List<Attachment> attachments = tmp.ToList();

    for (int i = 0; i < vehicles.Count(); i++)
    {
        var e = vehicles.ElementAt(i);

        e.Images =
            attachments.Where(w => w.Identity.Equals(e.AttachmentIdentity) && w.IsDefault).Select(s => $"{s.FileUrl}{s.ThumbnailFile}").ToArray().Length > 0 ?
            attachments.Where(w => w.Identity.Equals(e.AttachmentIdentity) && w.IsDefault).Select(s => $"{s.FileUrl}{s.ThumbnailFile}").FirstOrDefault() : attachments.Where(w => w.Identity.Equals(e.AttachmentIdentity)).OrderBy(o => o.Sort).Select(s => $"{s.FileUrl}{s.ReNameFile}").FirstOrDefault();
    }

    return vehicles;
}