using Microsoft.AspNetCore.Http;
using SportsStore.Models.Photos;

namespace SportsStore.Interfaces
{
    public interface IPhotoAccessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);
        string DeletePhoto(string publicId);
    }
}