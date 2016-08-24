﻿using System;
using Easy.Web.CMS.Event;
using Easy.Web.CMS.Page;
using System.Web;
using Easy.CMS.Common.Models;
using Easy.Extend;
using Easy.Web.CMS.PageView;
using Easy.Web.ValueProvider;

namespace Easy.CMS.Common.Service
{
    public class OnPageExcuted : IOnPageExecuted
    {
        private readonly IPageViewService _pageViewService;
        private readonly ICookie _cookie;
        private const string UserKey = "DKBNTJGFKDSLW";
        public OnPageExcuted(IPageViewService pageViewService, ICookie cookie)
        {
            _pageViewService = pageViewService;
            _cookie = cookie;
        }

        public void OnExecuted(PageEntity currentPage, HttpContext context)
        {
            var value = _cookie.GetValue<string>(UserKey);
            if (value.IsNullOrWhiteSpace())
            {
                value = Guid.NewGuid().ToString("N");
                _cookie.SetValue(UserKey, value, true, true);
            }
            var pageView = new PageView
            {
                PageUrl = context.Request.RawUrl,
                PageTitle = currentPage.Title ?? currentPage.PageName,
                SessionID = value,
                IPAddress = context.Request.UserHostAddress,
                UserAgent = context.Request.UserAgent
            };
            if (context.Request.UrlReferrer != null)
            {
                pageView = _pageViewService.GenerateReferer(pageView, context.Request.UrlReferrer.ToString());
            }
            _pageViewService.Add(pageView);
        }
    }
}