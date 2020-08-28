using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorRepository authorRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List Of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location}: Successful");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }
        /// <summary>
        /// Get An Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id: {id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location}: Successfully got record with id: {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Creates An Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Create Attempted");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty Request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was Incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Creation failed");
                }
                _logger.LogInfo($"{location}: Creation was successful");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Updates An Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update Attempted on record with id: {id} ");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - id: {id}");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was Incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Update failed for record with id: {id}");
                }
                _logger.LogInfo($"{location}: Record with id: {id} successfully updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Removes an author by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Delete Attempted on record with id: {id} ");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Delete failed for record with id: {id}");
                }
                _logger.LogInfo($"{location}: Record with id: {id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }





    #region SENZA location
    ///// <summary>
    ///// Endpoint usato per interagire con gli Authors prensenti nel database.
    ///// </summary>
    //[Route("api/[controller]")]
    //[ApiController]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public class AuthorsController : ControllerBase
    //{
    //    private readonly IAuthorRepository _authorRepository;
    //    private readonly ILoggerService _logger;
    //    private readonly IMapper _mapper;

    //    public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
    //    {
    //        this._authorRepository = authorRepository;
    //        this._logger = logger;
    //        this._mapper = mapper;
    //    }

    //    /// <summary>
    //    /// Recupera tutti gli Autori
    //    /// </summary>
    //    /// <returns>List of Authors</returns>
    //    [HttpGet]
    //    [ProducesResponseType(StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<IActionResult> GetAuthors()
    //    {
    //        try
    //        {
    //            _logger.LogInfo("Tentativo di recupero di tutti gli Autori.");
    //            var authors = await _authorRepository.FindAll();
    //            var response = _mapper.Map<IList<AuthorDTO>>(authors);
    //            _logger.LogInfo("Elenco degli Autori creato con successo.");
    //            return Ok(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            return InternalError($"{ex.Message} - {ex.InnerException}");
    //        }
    //    }

    //    /// <summary>
    //    /// Recupera un Autore tramite il suo ID
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns>Record dell'Autore</returns>
    //    [HttpGet("{id}")]
    //    [ProducesResponseType(StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<IActionResult> GetAuthor(int id)
    //    {
    //        try
    //        {
    //            _logger.LogInfo($"Tentativo di recupero dell'Autore con id:{id}");
    //            var author = await _authorRepository.FindById(id);
    //            if(author==null)
    //            {
    //                _logger.LogWarn($"L'Autore con id:{id} non è stato trovato.");
    //                return NotFound();
    //            }
    //            var response = _mapper.Map<AuthorDTO>(author);
    //            _logger.LogInfo($"Recupero dell'Autore con id:{id} effettuato con successo.");
    //            return Ok(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            return InternalError($"{ex.Message} - {ex.InnerException}");
    //        }
    //    }

    //    /// <summary>
    //    /// Creazione di un nuovo Autore
    //    /// </summary>
    //    /// <param name="authorDTO"></param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [ProducesResponseType(StatusCodes.Status201Created)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
    //    {
    //        try
    //        {
    //            _logger.LogInfo($"Tentativo di creazione di un nuovo Autore.");
    //            if (authorDTO==null)
    //            {
    //                _logger.LogWarn($"I dati inviati con la richiesta sono vuoti.");
    //                return BadRequest(ModelState);
    //            }
    //            if(!ModelState.IsValid)
    //            {
    //                _logger.LogWarn($"I dati dell'Autore sono incompleti.");
    //                return BadRequest(ModelState);
    //            }
    //            var author = _mapper.Map<Author>(authorDTO);
    //            var isSuccess = await _authorRepository.Create(author);
    //            if(!isSuccess)
    //            {
    //                return InternalError($"La creazione del nuovo Autore è fallita.");
    //            }
    //            _logger.LogInfo($"Nuovo Autore creato.");
    //            return Created("Create", new { author });
    //        }
    //        catch (Exception ex)
    //        {
    //            return InternalError($"{ex.Message} - {ex.InnerException}");
    //        }
    //    }

    //    /// <summary>
    //    /// Aggiornamento di un autore esistente
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <param name="authorDTO"></param>
    //    /// <returns></returns>
    //    [HttpPut("{id}")]
    //    [ProducesResponseType(StatusCodes.Status204NoContent)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
    //    {
    //        try
    //        {
    //            _logger.LogInfo($"Tentativo di aggiornamento dell'Autore con id:{id}");
    //            if (id < 1 || authorDTO == null || id != authorDTO.Id)
    //            {
    //                _logger.LogWarn($"Aggiornamento dell'Autore fallita per dati incoerenti.");
    //                return BadRequest();
    //            }
    //            var isExists = await _authorRepository.isExists(id);
    //            if (!isExists)
    //            {
    //                _logger.LogWarn($"Non è stato trovato l'Autore con id:{id}");
    //                return NotFound();
    //            }
    //            if (!ModelState.IsValid)
    //            {
    //                _logger.LogWarn($"I dati dell'autore passati sono incompleti");
    //                return BadRequest(ModelState);
    //            }
    //            var author = _mapper.Map<Author>(authorDTO);
    //            var isSuccess = await _authorRepository.Update(author);
    //            if (!isSuccess)
    //            {
    //                return InternalError($"Aggiornamento dell'Autore fallito.");
    //            }
    //            _logger.LogWarn($"L'Autore con id: {id} è stato aggiornato.");
    //            return NoContent();
    //        }
    //        catch (Exception e)
    //        {
    //            return InternalError($"{e.Message} - {e.InnerException}");
    //        }
    //    }

    //    /// <summary>
    //    /// Cancella un Autore tramite il suo ID
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    [HttpDelete("{id}")]
    //    [ProducesResponseType(StatusCodes.Status204NoContent)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        try
    //        {
    //            _logger.LogInfo($"Tentativo di eliminazione dell'Autore con id:{id}");
    //            if (id < 1)
    //            {
    //                _logger.LogWarn($"Eliminazione dell'Autore fallita per dati incoerenti.");
    //                return BadRequest();
    //            }
    //            var isExists = await _authorRepository.isExists(id);
    //            if (!isExists)
    //            {
    //                _logger.LogWarn($"L'Autore con id:{id} non è stato trovato.");
    //                return NotFound();
    //            }
    //            var author = await _authorRepository.FindById(id);
    //            var isSuccess = await _authorRepository.Delete(author);
    //            if (!isSuccess)
    //            {
    //                return InternalError($"Eliminazione dell'Autore fallita.");
    //            }
    //            _logger.LogWarn($"L'Autore con id:{id} è stato eliminato.");
    //            return NoContent();
    //        }
    //        catch (Exception e)
    //        {
    //            return InternalError($"{e.Message} - {e.InnerException}");
    //        }
    //    }

    //    private ObjectResult InternalError(string message)
    //    {
    //        _logger.LogError(message);
    //        return StatusCode(StatusCodes.Status500InternalServerError, "Si è verificato un errore imprevisto. Prego, contattare l'Amministratore.");

    //    }
    //}
    #endregion SENZA location
}
