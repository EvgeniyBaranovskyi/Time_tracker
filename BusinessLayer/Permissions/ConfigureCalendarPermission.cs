﻿using DataLayer.Entities;
using System.Security;

namespace BusinessLayer.Permissions
{
    public class ConfigureCalendarPermission : IPermission
    {
        public PermissionType Type => PermissionType.ConfigureCalendar;

        public bool IsGranted(User? user)
        {
            return user?.IsAdmin ?? false;
        }
    }
}