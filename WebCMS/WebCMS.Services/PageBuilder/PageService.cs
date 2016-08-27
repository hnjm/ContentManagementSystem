﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMS.Entities;
using WebCMS.Entities.Models.PageBuilder;
using WebCMS.Services.Authentication;
using System.Data.Entity;

namespace WebCMS.Services.PageBuilder
{
    public interface IPageService
    {
        IEnumerable<Page> Get();

        Page View(int? userId, int pageId);

        Page Get(int pageId);

        int Add(string pageName, string area, string controller, string action);

        void Edit(int pageId, string pageName, string area, string controller, string action);

        void Delete(int pageId);

        void Order(int pageId, string sectionList);

        void Roles(int pageId, List<string> roleList);
    }

    public class PageService : IPageService
    {
        #region Dependencies

        private readonly WebEntityModel _context;
        private readonly IUserService _userService;

        public PageService(WebEntityModel context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        #endregion Dependencies

        public IEnumerable<Page> Get()
        {
            var results = _context.Pages.OrderBy(x => x.PageName);

            return results;
        }

        public Page View(int? userId, int pageId)
        {
            var page = _context.Pages.Include(x => x.PageSections).SingleOrDefault(x => x.PageId == pageId);

            var userRoleList = new List<string>();

            if (userId.HasValue)
            {
                var user = _userService.GetUser(userId.Value);

                if (user.Roles.Any(x => x.Role.RoleName == "Admin"))
                    return page;

                userRoleList.AddRange(user.Roles.Select(x => x.Role.RoleName));

                if (userRoleList.Contains(page.PageRoles.SelectMany(x => x.Role.RoleName)))
                    return page;
            }
            else if (!page.PageRoles.Any())
            {
                return page;
            }

            return null;
        }

        public Page Get(int pageId)
        {
            var page = _context.Pages.Include(x => x.PageSections).SingleOrDefault(x => x.PageId == pageId);

            return page;
        }

        public int Add(string pageName, string area, string controller, string action)
        {
            var newPage = new Page
            {
                PageName = pageName,
                PageArea = area,
                PageController = controller,
                PageAction = action,
                DateAdded = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            _context.Pages.Add(newPage);

            _context.SaveChanges();

            return newPage.PageId;
        }

        public void Edit(int pageId, string pageName, string area, string controller, string action)
        {
            var page = _context.Pages.SingleOrDefault(x => x.PageId == pageId);

            if (page == null)
                return;

            page.PageName = pageName;
            page.PageArea = area;
            page.PageController = controller;
            page.PageAction = action;
            page.DateUpdated = DateTime.Now;

            _context.SaveChanges();
        }

        public void Delete(int pageId)
        {
            var page = _context.Pages.SingleOrDefault(x => x.PageId == pageId);

            if (page == null)
                return;

            _context.Pages.Remove(page);

            _context.SaveChanges();
        }

        public void Order(int pageId, string sectionList)
        {
            var page = _context.Pages.SingleOrDefault(x => x.PageId == pageId);

            if (page == null)
                return;

            var sections = sectionList.Split(',');

            foreach (var sectionProperties in sections)
            {
                var properties = sectionProperties.Split('-');

                var orderId = properties[0];
                var sectionId = properties[1];

                var section = page.PageSections.SingleOrDefault(x => x.PageSectionId.ToString() == sectionId.ToString());

                if (section == null)
                    continue;

                section.PageSectionOrder = Convert.ToInt32(orderId);
            }

            _context.SaveChanges();
        }

        public void Roles(int pageId, List<string> roleList)
        {
            var page = Get(pageId);

            if (page == null)
                return;

            var roles = _context.Roles.ToList();

            if (page.PageRoles != null)
                foreach (var role in page.PageRoles.ToList())
                    _context.PageRoles.Remove(role);

            foreach (var roleName in roleList)
            {
                var currentRole = roles.FirstOrDefault(x => x.RoleName == roleName);

                if (currentRole == null)
                    continue;

                _context.PageRoles.Add(new PageRole { PageId = pageId, RoleId = currentRole.RoleId });
            }

            _context.SaveChanges();
        }
    }
}
