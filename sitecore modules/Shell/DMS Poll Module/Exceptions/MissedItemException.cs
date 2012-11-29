/* *********************************************************************** *
 * File   : MissedItemException.cs                        Part of Sitecore *
 * Version: 1.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Used when appeared some problems in reddering settings         *
 * (wrong or missed PollPath parameter, wrong or missed  Data source,      *
 * missed poll rendering)                                                  *
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
using System;

namespace Sitecore.Modules.DMSPoll.Exceptions
{
   public class MissedItemException : Sitecore.Exceptions.SitecoreException
   {
      public MissedItemException(string message, Exception e)
         : base(message, e)
      {
      }
   }
}
