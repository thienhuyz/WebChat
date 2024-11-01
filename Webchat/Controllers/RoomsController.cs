using AutoMapper;
using Chat.Data;
using Chat.Data.Entities;
using Chat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly IMapper _mapper;

        public RoomsController(ChatDbContext context,
          IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {

            var rooms = await _context.Rooms.ToListAsync();

            var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            return Ok(roomViewModel);
        }

    }
}
