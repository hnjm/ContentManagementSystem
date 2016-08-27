﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMS.Entities;

namespace WebCMS.Services.Copy
{
    public interface ICopyService
    {
        int Create(string copyName, string copyBody);

        void Edit(int copyId, string copyName, string copyBody);

        IEnumerable<WebCMS.Entities.Models.Copy.Copy> Get();

        WebCMS.Entities.Models.Copy.Copy Get(int copyId);

        WebCMS.Entities.Models.Copy.Copy Get(string copyName);

        void Delete(int copyId);
    }

    public class CopyService : ICopyService
    {
        #region Dependencies

        private readonly WebEntityModel _context;

        public CopyService(WebEntityModel context)
        {
            _context = context;
        }

        #endregion Dependencies

        public int Create(string copyName, string copyBody)
        {
            var newCopy = new WebCMS.Entities.Models.Copy.Copy
            {
                CopyName = copyName,
                CopyBody = copyBody,
                DateAdded = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            _context.CopySections.Add(newCopy);

            _context.SaveChanges();

            return newCopy.CopyId;
        }

        public void Edit(int copyId, string copyName, string copyBody)
        {
            var copy = _context.CopySections.SingleOrDefault(x => x.CopyId == copyId);

            if (copy == null)
                return;

            copy.CopyName = copyName;
            copy.CopyBody = copyBody;
            copy.DateUpdated = DateTime.Now;

            _context.SaveChanges();
        }

        public IEnumerable<WebCMS.Entities.Models.Copy.Copy> Get()
        {
            var copySections = _context.CopySections.OrderBy(x => x.CopyName).ThenBy(x => x.CopyId);

            return copySections;
        }

        public WebCMS.Entities.Models.Copy.Copy Get(int copyId)
        {
            var copyItem = _context.CopySections.SingleOrDefault(x => x.CopyId == copyId);

            return copyItem;
        }

        public WebCMS.Entities.Models.Copy.Copy Get(string copyName)
        {
            var copyItem = _context.CopySections.FirstOrDefault(x => x.CopyName == copyName);

            return copyItem;
        }

        public void Delete(int copyId)
        {
            var copyItem = _context.CopySections.SingleOrDefault(x => x.CopyId == copyId);

            if (copyItem != null)
                _context.CopySections.Remove(copyItem);

            _context.SaveChanges();
        }
    }
}
