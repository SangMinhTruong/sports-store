using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SportsStore.Data;
using SportsStore.Interfaces;
using SportsStore.Models.Photos;

namespace SportsStore.Controllers
{
    public class PhotoController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IPhotoAccessor _photoAccessor;

        public PhotoController(StoreDbContext context, IPhotoAccessor photoAccessor)
        {
            this._photoAccessor = photoAccessor;
            _context = context;
        }

        [HttpPost("[controller]/[action]/{id?}")]
        public async Task<Object> AddPhoto(IFormFile file, int id)
        {
            var inputProduct = await _context.Products.FirstOrDefaultAsync(m => m.ID == id);
            if (inputProduct == null)
            {
                return JsonConvert.SerializeObject(new { Error = "Product not found" });
            }
            var photoUploadResult = _photoAccessor.AddPhoto(file);
            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublicId,
                product = inputProduct,
                ProductId = id
            };
            inputProduct.Photos = _context.Photos.Where(x => x.ProductId == inputProduct.ID).ToList();
            if (!inputProduct.Photos.Any(x => x.IsMain))
                photo.IsMain = true;
            //inputProduct.Photos.Add(photo);
            _context.Photos.Add(photo);
            //return result
            var isSuccess = await _context.SaveChangesAsync() > 0;
            if (isSuccess) return JsonConvert.SerializeObject(new { Id = photo.Id, Url = photo.Url, ProductId = id });

            return JsonConvert.SerializeObject(new { Error = "Problem saving changes" });
        }

        [HttpDelete("[controller]/[action]/{id?}")]
        public async Task<Object> DeletePhoto(string id)
        {
            var photo = _context.Photos.FirstOrDefault(x => x.Id == id);

            if (photo == null)
                return JsonConvert.SerializeObject(new { Error = "Photo not found" });
            if (photo.IsMain)
                return JsonConvert.SerializeObject(new { Error = "Can not delete main photo" });

            var result = _photoAccessor.DeletePhoto(photo.Id);
            _context.Photos.Remove(photo);
            if (result == null)
                return JsonConvert.SerializeObject(new { Error = "Problem deleting photo" });
            //return result
            var isSuccess = await _context.SaveChangesAsync() > 0;
            if (isSuccess) return JsonConvert.SerializeObject(new { Result = "Delete photo success" });

            throw new Exception("Problem saving changes");
        }

        [HttpPut("[controller]/[action]/{id?}")]
        public async Task<Object> SetMain(string id)
        {
            var photo = _context.Photos.FirstOrDefault(x => x.Id == id);

            if (photo == null)
                return JsonConvert.SerializeObject(new { Error = "Photo not found" });
            List<Photo> listPhoto = _context.Photos.Where(x => x.ProductId == photo.ProductId).ToList();
            var currentMain = listPhoto.Find(x => x.IsMain == true);
            currentMain.IsMain = false;
            photo.IsMain = true;
            var isSuccess = await _context.SaveChangesAsync() > 0;
            if (isSuccess) return JsonConvert.SerializeObject(new { Result = "Set main photo success" });

            return JsonConvert.SerializeObject(new { Error = "Problem saving changes" });
        }
    }
}