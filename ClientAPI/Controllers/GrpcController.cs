using ClientAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ClientAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GrpcController : ControllerBase
    {
        private readonly IFoxMessage _foxMessage;

        public GrpcController(IFoxMessage foxMessage)
        {
            _foxMessage = foxMessage;
        }

        public string QueueName { get; set; } = "queue_1";

        [HttpGet("enviar-mensagem")]
        public async Task<IActionResult> EnviarMensagem(string body)
        {
            try
            {
                var response = await _foxMessage.Publish(QueueName, body);

                string mensagem = "Mensagem Não Adicionada na Fila";

                if (response.Status)
                {
                    mensagem = "Mensagem adicionada na fila - Id: " + response.Id;
                    return Ok(mensagem);
                }

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("receber-mensagem")]
        public async Task<IActionResult> ReceberMensagem()
        {
            try
            {
                var response = await _foxMessage.Get(QueueName);

                string mensagem = "Sem mensagem na fila";

                if (response.Status)
                {
                    mensagem = "Mensagem Processada: " + response.Body;

                    mensagem += ". Mensagem Criada em: " + response.Insertiondate;

                    await _foxMessage.Confirm(response.Id, QueueName);
                    mensagem += ". Mensagem Removida";
                }
                
                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("confirm-mensagem")]
        public async Task<IActionResult> ConfirmMensagem(long id)
        {
            try
            {
                var response = await _foxMessage.Confirm(id, QueueName);
                return Ok(response.Status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
