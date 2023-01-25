using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{


    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(),"Antidote","Cures poison",7,DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Potion","Restores a small amount of HP",5,DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Iron sword","A standard sword made of iron",33,DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Ether","Restores a small amount of MP",66,DateTimeOffset.UtcNow),
        };

        //GET /items
        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
           return items;
        }


        //GET /items/{id}
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.FirstOrDefault(x=>x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }


        //POST /items
        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        //PUT /items/{id}
        [HttpPut]
        public ActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = items.FirstOrDefault(x => x.Id == id);

            if (existingItem == null)
            {
                return NotFound();
            }

            var updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price,
            };

            var index = items.FindIndex(item => item.Id == id);
            items[index] = updatedItem;

            return NoContent();
        }

        //DELETE /items/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            if (index < 0)
            {
                return NotFound();
            }
            items.RemoveAt(index);

            return NoContent();
        }
    }
}
