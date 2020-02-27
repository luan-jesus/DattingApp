using DattingApp.API.Data;
using DattingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using CloudinaryDotNet;
using System.Threading.Tasks;
using DattingApp.API.DTOs;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DattingApp.API.Models;
using System.Linq;

namespace DattingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDattingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudnarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDattingRepository repo, IMapper mapper, IOptions<CloudnarySettings> cloudinaryConfig)
        { 
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account
            (
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.APIKey,
                _cloudinaryConfig.Value.APISecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto (int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoToReturnDTO>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDTO PhotoForCreationDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(userId);

            var file = PhotoForCreationDTO.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            PhotoForCreationDTO.Url = uploadResult.Uri.ToString();
            PhotoForCreationDTO.PublicId = uploadResult.PublicId;

            //var photo = _mapper.Map<Photo>(PhotoForCreationDTO);
            var photo = new Photo()
            {
                Url = PhotoForCreationDTO.Url,
                Description = PhotoForCreationDTO.Description,
                DateAdded = PhotoForCreationDTO.DateAdded,
                PublicId = PhotoForCreationDTO.PublicId
            };

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);
            
            if (await _repo.SaveAll())
            {
                var photoToReturnDTO = _mapper.Map<PhotoToReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, photoToReturnDTO);
            }
            return BadRequest("Could not add the photo");
        }
    }
}