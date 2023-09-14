using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
   public  class SiteAssets
    {
        public string previewImageSrc { get; set; }
        public string thumbnailImageSrc { get; set; }
        public string alt { get; set; }
        public string tiltle { get; set; }

    }

    public class ImageResponse
    {
        public List<SiteAssets> data { get; set; }
    }
}
