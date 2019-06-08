﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NETCoreSyncMobileSample.Models;

namespace NETCoreSyncMobileSample.ViewModels
{
    public class DepartmentListViewModel : CustomBaseViewModel
    {
        public DepartmentListViewModel()
        {
            Title = HomeMenuItem.GetMenus().Where(w => w.Id == MenuItemType.DepartmentList).First().Title;
        }
    }
}