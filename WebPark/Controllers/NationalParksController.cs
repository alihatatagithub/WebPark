using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPark.Models;
using WebPark.Models.DTOS;
using WebPark.Repository.IRepository;

namespace WebPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private INationalParkRepository _npRepository;
        private IMapper _mapper;
        public NationalParksController(INationalParkRepository npRepository, IMapper mapper)
        {
            _npRepository = npRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepository.GetNationalParks();
            List<NationalParkDTO> objDto = new List<NationalParkDTO>();
            foreach (var item in objList)
            {
                //NationalParkDTO newnpdto = new NationalParkDTO();
                //newnpdto.Id = item.Id;
                //newnpdto.Name = item.Name;
                //newnpdto.Created = item.Created;
                //newnpdto.Established = item.Established;
                //newnpdto.State = item.State;
                objDto.Add(_mapper.Map<NationalParkDTO>(item));


            }
            return Ok(objDto);
        }

        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepository.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();

            }

            var objDto = _mapper.Map<NationalParkDTO>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark(NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepository.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError("", "National Park Existe");
                return (StatusCode(404, ModelState));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_npRepository.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"SomeThing went wrong when saving the record{nationalParkObj.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id }, nationalParkObj);
        }
        //Do Not Forget [From Body]
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkId != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }
            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_npRepository.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"SomeThing went wrong when saving the record{nationalParkObj.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();

        }

        [HttpDelete]
        public IActionResult DeleteNationalPark(int nationalParkId,NationalParkDTO nationalParkDTO)
        {
            if (!_npRepository.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }
            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_npRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"SomeThing went wrong when saving the record{nationalParkObj.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
            return NoContent();
        }
    }
}
