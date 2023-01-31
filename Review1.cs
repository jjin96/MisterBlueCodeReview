public class AttachmentService
{
    [Unity.Dependency]
    public AttachmentReository repository { get; set; }

    public Tuple<bool, string, IEnumerable<Attachment>> Create(IEnumerable<HttpPostedFileBase> files, AttachmentParams attachmentParams)
    {
        int fileCount = files.Count();
        int defaultId = 0;

        if (fileCount > 0)
        {
            string basePath = string.Empty, baseUrl = string.Empty;
            string[] Identitys = attachmentParams.Identity.Split('_');  //형식 : Session.SessionID_DateTime.Now.ToString("yyMMddHHmmss")
            if (attachmentParams.IsProtect)
            {
                basePath = $"{HttpContext.Current.Server.MapPath("/")}/../Data/{DateTime.Now.ToString("yyyMMdd")}/";
            }
            else
            {
                baseUrl = $"/Data/{DateTime.Now.ToString("yyyMMdd")}/";
                if (Extentions.GetAppSetting("Site").Equals("Hansung"))
                {                        
                    basePath = HttpContext.Current.Server.MapPath(baseUrl).ToString().Replace("supervise.hansungusedcar.co.kr", "hansungusedcar.co.kr");
                    if (basePath.Contains("Pl"))
                    {
                        basePath = basePath.Replace("\\Pl", "");
                    }
                    L4.Info(basePath);
                }
                else
                {
                    basePath = HttpContext.Current.Server.MapPath(baseUrl).ToString();
                }                    
            }

            IQueryable<Attachment> results = repository.ReadAll();

            IQueryable<Attachment> getDefault = null;
            
            int nextId = (results.Count() == 0 ? 0 : results.Max(m => m.Id)) + 1;

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }                

            List<Attachment> attachments = new List<Attachment>();

            List<Attachment> updateList = new List<Attachment>();

            getDefault = results.Where(w => w.Identity.Equals(attachmentParams.Identity) && w.Kind.Equals("CkeditorImage"));

            updateList = getDefault.ToList();

            if (getDefault.Count() > 0 && attachmentParams.IsImage)
            {                    
                defaultId = getDefault.Where(w => w.IsDefault).Count() > 0 ? getDefault.Where(w => w.IsDefault).FirstOrDefault().Id : 0;
            }

            try
            {

                for (int i = 0; i < fileCount; i++)
                {
                    var file = files.ElementAt(i);
                    if (file != null && file.ContentLength > 0)
                    {
                        Attachment attachment = new Attachment();
                        attachment.Id = nextId + i;
                        attachment.CreatedDT = DateTime.Now;
                        attachment.FileType = file.ContentType;
                        attachment.FileSize = file.ContentLength;

                        //사용자에게 보여줄땐 원본파일명을 사용한다.
                        attachment.OriginalFile = file.FileName;
                        attachment.FilePath = basePath;
                        attachment.FileExtention = Path.GetExtension(attachment.OriginalFile).Substring(1);//.제거


                        attachment.IsDefault = attachment.OriginalFile.Equals(attachmentParams.DefaultFile);
                        attachment.Identity = attachmentParams.Identity;
                        attachment.ModelName = attachmentParams.ModelName;
                        attachment.Kind = attachmentParams.Kind;
                        attachment.IsProtect = attachmentParams.IsProtect;

                        //실제 저장은 ReNameFile으로 한다. 파일명 중복에 대비
                        attachment.ReNameFile = $"{Path.GetFileNameWithoutExtension(attachment.OriginalFile)}_{Extentions.GetClaim("Idx")}_{DateTime.Now.ToString("yyyMMddHHmmss")}.{attachment.FileExtention}";
                        attachment.ThumbnailFile = $"{Path.GetFileNameWithoutExtension(attachment.OriginalFile)}_{DateTime.Now.ToString("yyyMMddHHmmss")}_thumb.{attachment.FileExtention}";
                        attachment.FileFullPath = Path.Combine(attachment.FilePath, attachment.ReNameFile);
                        attachment.Sort = i + 1;

                        if (!attachment.IsProtect)
                        {
                            //url을 통한 직접접근을 설정
                            attachment.FileUrl = baseUrl;
                            attachment.FileFullUrl = Path.Combine(baseUrl, (string.IsNullOrEmpty(attachment.ReNameFile) ? attachment.OriginalFile : attachment.ReNameFile));
                        }

                        file.SaveAs(attachment.FileFullPath);

                        if (attachmentParams.IsImage)
                        {
                            if(defaultId > 0)
                            {
                                if (attachment.IsDefault)
                                {
                                    if (defaultId != attachment.Id)
                                    {
                                        Update(updateList);
                                    }
                                }                                    
                            }

                            Image img = Image.FromFile(attachment.FileFullPath);
                            Size thumbsize = new Size();
                            thumbsize.Width = 290;
                            thumbsize.Height = 170;
                            
                            float scalingRatio = CalculateScalingRatio(img.Size, thumbsize);

                            int scaledWidth = (int)Math.Round((float)img.Size.Width * scalingRatio);
                            int scaledHeight = (int)Math.Round((float)img.Size.Height * scalingRatio);
                            int scaledLeft = (thumbsize.Width - scaledWidth) / 2;
                            int scaledTop = (thumbsize.Height - scaledHeight) / 2;

                            Rectangle cropArea = new Rectangle(scaledLeft, scaledTop, scaledWidth, scaledHeight);

                            if (scaledWidth < scaledHeight && scaledHeight > thumbsize.Height)
                            {
                                scaledTop = (thumbsize.Height - scaledHeight) / 4;
                            }

                            Image thumb = img.GetThumbnailImage(thumbsize.Width, thumbsize.Height, () => false, IntPtr.Zero); //썸네일 리사이즈 주석처리
                            using (Graphics thumbGraphic = Graphics.FromImage(thumb))
                            {
                                thumbGraphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                thumbGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                thumbGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                thumbGraphic.DrawImage(img, cropArea);
                            }
                            double aspectRatio_X = 4;
                            double aspectRatio_Y = 3;
                            double targetHeight = Convert.ToDouble(300) / (aspectRatio_X / aspectRatio_Y);

                            thumb = cropImg(thumb);
                            Bitmap bmp = new Bitmap(300, (int)targetHeight);
                            Graphics grp = Graphics.FromImage(bmp);
                            grp.DrawImage(thumb, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

                            thumb.Save(Path.Combine(basePath, attachment.ThumbnailFile));
                            img.Dispose();
                            thumb.Dispose();

                        }

                        repository.Create(attachment);
                        attachments.Add(attachment);
                    }
                }
                return new Tuple<bool, string, IEnumerable<Attachment>>(true, MessageType.Success.GetDescription(), attachments.ToList());
            }
            catch (Exception ex)
            {

            }
        }

        return new Tuple<bool, string, IEnumerable<Attachment>>(false, MessageType.Failed.GetDescription(), null);
    }

    private float CalculateScalingRatio(Size originalSize, Size targetSize)
    {
        float originalAspectRatio = (float)originalSize.Width / (float)originalSize.Height;
        float targetAspectRatio = (float)targetSize.Width / (float)targetSize.Height;

        float scalingRatio = 0;

        if (targetAspectRatio >= originalAspectRatio)
        {
            scalingRatio = (float)targetSize.Width / (float)originalSize.Width;
        }
        else
        {
            scalingRatio = (float)targetSize.Height / (float)originalSize.Height;
        }

        return scalingRatio;
    }

    public Image cropImg(Image img)
    {
        double aspectRatio_X = 4;
        double aspectRatio_Y = 3;

        double imgWidth = Convert.ToDouble(img.Width);
        double imgHeight = Convert.ToDouble(img.Height);

        if (imgWidth / imgHeight > (aspectRatio_X / aspectRatio_Y))
        {
            double extraWidth = imgWidth - (imgHeight * (aspectRatio_X / aspectRatio_Y));
            double cropStartFrom = extraWidth / 2;
            Bitmap bmp = new Bitmap((int)(img.Width - extraWidth), img.Height);
            Graphics grp = Graphics.FromImage(bmp);
            grp.DrawImage(img, new Rectangle(0, 0, (int)(img.Width - extraWidth), img.Height), new Rectangle((int)cropStartFrom, 0, (int)(imgWidth - extraWidth), img.Height), GraphicsUnit.Pixel);
            return (Image)bmp;
        }
        else
            return null;
    }
}