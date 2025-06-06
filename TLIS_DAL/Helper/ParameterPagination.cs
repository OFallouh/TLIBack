﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLIS_DAL.Helper
{
    public class ParameterPagination
    {
        const int maxPageSize = 100000000;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 100;
        public int PageSize
        {   
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
