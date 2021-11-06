using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file); // IFormFile Represents a file sent with the HttpRequest.
        Task<DeletionResult> DeletePhhotoAsync(string publicId);// pass the string of public id, each file uploaded is gona be gvin a piblic id
    }
}