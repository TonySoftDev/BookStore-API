using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Controller API di prova
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Metodo GET di prova per elenco di valori di ritorno
        /// </summary>
        /// <returns></returns>
        // GET: api/<HomrController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Messaggio livello Info");
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Metodo GET di prova per un singolo valore di ritorno
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<HomrController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogDebug("Messaggio livello Debug");
            return "value";
        }

        // POST api/<HomrController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("Messaggio livello Errore");
        }

        // PUT api/<HomrController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomrController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("Messaggio livello Warning");
        }
    }
}
