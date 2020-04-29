using AndroidTest.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AndroidTest.Interfaces
{
    public interface IPhotoPickerService
    {
        Task<PickedImage> GetImageStreamAsync();
    }
}
