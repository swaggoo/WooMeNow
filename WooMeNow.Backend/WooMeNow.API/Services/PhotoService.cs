using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Security.Principal;
using WooMeNow.API.Helpers;
using WooMeNow.API.Interfaces;

namespace WooMeNow.API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> settings)
    {
        var acc = new Account
        (
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );
        _cloudinary =  new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhoto(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if(file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "wmn-net7"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;

    }

    public async Task<DeletionResult> DeletePhoto(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);

        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        return deletionResult;
    }
}
