using CloudinaryDotNet.Actions;

namespace WooMeNow.API.Interfaces;

public interface IPhotoService
{
    public Task<ImageUploadResult> AddPhoto(IFormFile file);
    public Task<DeletionResult> DeletePhoto(string publicId);
}
