/* *********************************************************************** *
 * File   : PlaceholderManager.cs                         Part of Sitecore *
 * Version: 1.1.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Used by Poll wizard when it rans from page editor              *
 *                                                                         *
 * Copyright (C) 1999-2009 by Sitecore A/S. All rights reserved.           *
 *                                                                         *
 * This work is the property of:                                           *
 *                                                                         *
 *        Sitecore A/S                                                     *
 *        Meldahlsgade 5, 4.                                               *
 *        1613 Copenhagen V.                                               *
 *        Denmark                                                          *
 *                                                                         *
 * This is a Sitecore published work under Sitecore's                      *
 * shared source license.                                                  *
 *                                                                         *
 * *********************************************************************** */
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Web;

namespace Sitecore.Modules.DMSPoll.Domain
{
    public class PlaceholderManager
    {
        private readonly Item _item;
        private readonly string _deviceID;

        #region public properties

        public LayoutDefinition Layouts
        {
            get
            {
                return LayoutDefinition.Parse(_item.Fields["__renderings"].Value);
            }
        }

        public DeviceDefinition Device
        {
            get
            {
                return Layouts.GetDevice(_deviceID);
            }
        }

        public string PlaceholdersUrl
        {
            get
            {
                var url = new StringBuilder(WebUtil.GetServerUrl());
                url.AppendFormat("/default.aspx?sc_de=1&sc_mode=edit&sc_itemid={0}&sc_placeholder=1", _item.ID);

                return url.ToString();
            }
        }

        #endregion

        #region public methods

        public PlaceholderManager(Item item, string deviceID)
        {
            _item = item;
            _deviceID = deviceID;
        }
        
        /// <summary>
        /// Gets the placeholders URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="deviceID">The device ID.</param>
        /// <returns></returns>
        public static string GetPlaceholdersUrl(Item item, string deviceID)
        {
            var manage = new PlaceholderManager(item, deviceID);
            return manage.PlaceholdersUrl;
        }

        /// <summary>
        /// Gets the device query string.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceQueryString()
        {
            Item device = _item.Database.GetItem(_deviceID);
            if (device != null && device.Fields["query string"] != null)
            {
                return "&" + device.Fields["query string"].Value;
            }
            return "";
        }

        #endregion
    }
}
