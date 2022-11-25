using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;

namespace Restaurant.Api.Controllers
{
    public class AdditionsController : BaseController
    {
        private readonly IAdditionService _additionService;

        public AdditionsController(IAdditionService additionService)
        {
            _additionService = additionService;
        }

        [HttpGet]
        public async Task<IEnumerable<AdditionDto>> GetAll()
        {
            return await _additionService.GetAllAsync();
        }

        [HttpGet("{additionId:guid}")]
        public async Task<ActionResult<AdditionDto>> Get(Guid additionId)
        {
            return OkOrNotFound(await _additionService.GetAsync(additionId));
        }

        [HttpPost]
        public async Task<ActionResult> Add(AdditionDto additionDto)
        {
            await _additionService.AddAsync(additionDto);
            return CreatedAtAction(nameof(Get), new { additionId = additionDto.Id }, null);
        }

        [HttpPut("{additionId:guid}")]
        public async Task<ActionResult> Update(Guid additionId, AdditionDto additionDto)
        {
            additionDto.Id = additionId;
            await _additionService.UpdateAsync(additionDto);
            return NoContent();
        }

        [HttpDelete("{additionId:guid}")]
        public async Task<ActionResult> Delete(Guid additionId)
        {
            await _additionService.DeleteAsync(additionId);
            return NoContent();
        }
    }
}
