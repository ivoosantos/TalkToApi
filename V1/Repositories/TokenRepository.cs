using TalkToApi.Database;
using TalkToApi.V1.Models;
using TalkToApi.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToApi.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TalkToContext _banco;
        public TokenRepository(TalkToContext banco)
        {
            _banco = banco;
        }
        public void Atualizar(Token token)
        {
            _banco.Tokens.Update(token);
            _banco.SaveChanges();
        }

        public void Cadastrar(Token token)
        {
            _banco.Tokens.Add(token);
            _banco.SaveChanges();
        }

        public Token Obter(string refreshToken)
        {
            return _banco.Tokens.FirstOrDefault(a => a.RefreshToken == refreshToken && a.Utilizado == false);
        }
    }
}
