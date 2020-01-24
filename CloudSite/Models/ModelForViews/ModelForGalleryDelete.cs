using CloudSite.Models.Photos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSite.Models.ModelForViews
{
    public class ModelForGalleryDelete
    {
        public List<Photo> _Photos { get; set; }
        public Dictionary<string, bool> _PhotosToDelete { get; set; }

        public ModelForGalleryDelete(List<Photo> Photos, Dictionary<string, bool> PhotosToDelete)
        {
            _Photos = Photos;
            _PhotosToDelete = PhotosToDelete;
        }
    }
}