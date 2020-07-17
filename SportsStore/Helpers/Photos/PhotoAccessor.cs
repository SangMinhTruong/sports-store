using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SportsStore.Interfaces;
using SportsStore.Models.Photos;

namespace SportsStore.Helpers.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        private readonly string cloudName = "dmf9ceepl";
        private readonly string apiKey = "256731423822432";
        private readonly string apiSecret = "XX5ppAvMa99PjMJtmULy_O0Gn1s";

        public PhotoAccessor()
        {
            var acc = new Account(cloudName, apiKey, apiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        public PhotoUploadResult AddPhoto(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                //read file into memory and dispose after finished
                using (var stream = file.OpenReadStream())
                {
                    //upload to Cloudinary with file params
                    var uploadParams = new ImageUploadParams
                    {
                        //file name and file content
                        File = new FileDescription(file.FileName, stream),
                        //Crop the top of image
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);
            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.AbsoluteUri
            };
        }

        public string DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = _cloudinary.Destroy(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}