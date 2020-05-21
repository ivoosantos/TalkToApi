using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TalkToApi.V1.Models;
using TalkToApi.V1.Models.DTO;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MensagemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMensagemRepository _mensagemRepository;
        public MensagemController(IMapper mapper, IMensagemRepository mensagemRepository)
        {
            _mapper = mapper;
            _mensagemRepository = mensagemRepository;
        }

        [Authorize]
        [HttpGet("{usuarioUmId}/{usuarioDoisId}", Name = "MensagemObter")]
        public ActionResult ObterMensagens(string usuarioUmId, string usuarioDoisId, [FromHeader(Name = "Accept")]string mediaType)
        {
            if(usuarioUmId == usuarioDoisId)
            {
                return UnprocessableEntity();
            }

            var mensagens = _mensagemRepository.ObterMensagens(usuarioUmId, usuarioDoisId);
            if (mediaType == "application/vnd.talkto.hateoas+json")
            {

                var listaMsg = _mapper.Map<List<Mensagem>, List<MensagemDTO>>(mensagens);
                var lista = new ListaDTO<MensagemDTO>() { Lista = listaMsg };
                lista.Links.Add(new LinkDTO("_self", Url.Link("MensagemObter", new { usuarioUmId = usuarioUmId, usuarioDoisId = usuarioDoisId }), "GET"));

                return Ok(lista);
            }
            else
            {
                return Ok(mensagens);
            }

        }

        [Authorize]
        [HttpPost("", Name = "MensagemCadastrar")]
        public ActionResult Cadastrar([FromBody]Mensagem mensagem)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.Cadastrar(mensagem);

                    var msgDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                    msgDTO.Links.Add(new LinkDTO("_self", Url.Link("MensagemCadastrar", null), "POST"));
                    msgDTO.Links.Add(new LinkDTO("_mensagemAtualizacaoParcial", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));

                    return Ok(msgDTO);
                }
                catch(Exception e)
                {
                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }

            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}", Name = "MensagemAtualizacaoParcial")]
        public ActionResult AtualizarParcial(int id, [FromBody]JsonPatchDocument<Mensagem> jsonPatch)
        {
            /*
             *  JSONPatch - [{ "op": "add|remove|replace", "path": "texto", "value": "Mensagem substituida!" }, { "op": "add|remove|replace", "path": "excluido", "value": true }]
             */

            if (jsonPatch == null)
                return BadRequest();

            var mensagem = _mensagemRepository.Obter(id);
            jsonPatch.ApplyTo(mensagem);

            mensagem.Atualizado = DateTime.UtcNow;
            _mensagemRepository.Atualizar(mensagem);

            var msgDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
            msgDTO.Links.Add(new LinkDTO("_self", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));

            return Ok(msgDTO);

        }
    }
}