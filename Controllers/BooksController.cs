using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using dorotec_backend_test.Classes.DTOs;
using dorotec_backend_test.Classes.Exceptions;
using dorotec_backend_test.Classes.Pagination;
using dorotec_backend_test.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dorotec_backend_test.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly IBookService _service;

    public BooksController(
        ILogger<BooksController> logger,
        IBookService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary> Lista paginada com todos os Livros que correspondem os critérios de pesquisa informados. </summary>
    /// <param name="filter"> Critérios de pesquisa. </param>
    [AllowAnonymous]
    [HttpPost("search", Name = "Book[action]")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PageResult<BookDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PageResult<BookDTO>>> SearchPage(
        [FromForm] BookFilterDTO filter
        )
    {
        try
        {
            var result = await _service.GetPage(filter);
            return Ok(result);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary> Lista paginada com todos os Livros. </summary>
    /// <param name="index"> Índice da página. </param>
    /// <param name="size"> Quantidade de registros por página. </param>
    [AllowAnonymous]
    [HttpGet(Name = "Book[action]")]
    [ProducesResponseType(typeof(PageResult<BookDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PageResult<BookDTO>>> GetPage(
        [Required][FromQuery][DefaultValue(1)][Range(1, Int32.MaxValue)] int index,
        [Required][FromQuery][DefaultValue(5)][Range(1, 30)] byte size
        )
    {
        try
        {
            var result = await _service.GetPage(index, size);
            return Ok(result);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary> Informações de um único livro. </summary>
    /// <param name="id"> Id do Livro. </param>
    [AllowAnonymous]
    [HttpGet("{id}", Name = "Book[action]")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDTO>> GetOne(
        [Required][FromRoute] int id
    )
    {
        try
        {
            var result = await _service.GetOne(id);
            return Ok(result);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary> Cadastrar um livro. </summary>
    /// <remarks>
    ///     Os livros são únicos e não podem ser repetidos.
    ///     Esta validação é feita de acordo com Nome do Autor, Nome do Livro, Gênero e Edição.
    ///     <para>Apenas com Autorização.</para>
    /// </remarks>
    /// <param name="dto"> Informações necessárias para realizar o cadastro. </param>
    [HttpPost(Name = "Book[action]")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> PostOne(
        [FromBody] BookDTO dto
    )
    {
        var created = await _service.Create(dto);

        return Created($"books/{created.Id}", created);
    }

    /// <summary> Alterar o registro de um único livro. </summary>
    /// <remarks> 
    ///     Apenas as propriedades informadas serão aletradas, propriedades nulas serão ignoradas.
    ///     <para>Apenas com Autorização.</para>
    /// </remarks>
    /// <param name="id"> Id do Livro. </param>
    /// <param name="dto"> Informações necessárias para realizar o cadastro. </param>
    [HttpPatch("{id}", Name = "Book[action]")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookDTO>> EditOne(
        [Required][FromRoute] int id,
        [Required][FromBody] BookDTO dto
    )
    {
        try
        {
            var result = await _service.UpdateOne(id, dto);
            return Ok(result);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary> Remover o registro de um único livro. </summary>
    /// <remarks> Apenas com Autorização. </remarks>
    /// <param name="id"> Id do Livro. </param>
    [HttpDelete("{id}", Name = "Book[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteOne(
        [Required][FromRoute] int id
    )
    {
        try
        {
            await _service.DeleteOne(id);
            return NoContent();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
}
