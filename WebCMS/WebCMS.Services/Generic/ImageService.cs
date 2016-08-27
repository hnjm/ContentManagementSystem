﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMS.Entities;
using WebCMS.Entities.Models.Generic;

namespace WebCMS.Services.Generic
{
    public interface IImageService
    {
        Image Get(int imageId);

        List<Image> Get();

        int Create(string imageFilePath, ImageCategory imageCategory);

        void Delete(int imageId);
    }

    public class ImageService : IImageService
    {
        #region Dependencies

        private readonly WebEntityModel _context;

        public ImageService(WebEntityModel context)
        {
            _context = context;
        }

        #endregion Dependencies

        public Image Get(int imageId)
        {
            var results = _context.Images.SingleOrDefault(x => x.ImageId == imageId);

            return results;
        }

        public List<Image> Get()
        {
            var results = _context.Images.OrderByDescending(x => x.ImageId).ToList();

            return results;
        }

        public int Create(string imageFilePath, ImageCategory imageCategory)
        {
            var image = new Image
            {
                ImagePath = imageFilePath,
                ImageCategory = imageCategory
            };

            _context.Images.Add(image);

            _context.SaveChanges();

            return image.ImageId;
        }

        public void Delete(int imageId)
        {
            var image = _context.Images.SingleOrDefault(x => x.ImageId == imageId);

            if (image == null)
                return;

            _context.Images.Remove(image);

            _context.SaveChanges();
        }
    }
}
