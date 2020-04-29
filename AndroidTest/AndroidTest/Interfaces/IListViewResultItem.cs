using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidTest.Interfaces
{
    public interface IListViewResultItem
    {
        string DisplayValue1 { get;  }
        string DisplayValue2 { get; }

        int ListPosition { get; set; }

        float SearchRsltSimilarity { get; set; }
    }
}
