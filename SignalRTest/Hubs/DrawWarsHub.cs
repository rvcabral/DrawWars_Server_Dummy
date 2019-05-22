using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.GameManager;
using SignalRTest.Logic;

namespace SignalRTest.Hubs
{
    public class DrawWarsHub : Hub
    {

        public async Task Inlist(string room)
        {
            Console.WriteLine($"Registering unto room {room}");
           
            var res = CoreManager.inlist(room, Context.ConnectionId);
            if (res.Session.Equals(Guid.Empty))
            {
                await Clients.Caller.SendAsync("NonExistingSession", res.PlayerId);
            }
            else
            {
                await Clients.Caller.SendAsync("AckSession", res);
            }
            
        }

        public async Task SetPlayerNickName(Context context, string nickname)
        {
            Console.WriteLine($"Player {context.PlayerId} from Session {context.Session} has set his nickname to {nickname}");
            var success = CoreManager.SetUserNickName(context, nickname);
            await Clients.Caller.SendAsync("AckNickname");
            string uiConId = CoreManager.GetUiClient(context);
            await Clients.Client(uiConId).SendAsync("NewPlayer", nickname);
        }

        public async Task Ready(Context context)
        {
            await Clients.All.SendAsync("DrawThemes", CoreManager.GetSession(context.Session).GetThemes());
        }

        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", session.Room);
        }

        public async Task SetTimesUp(Guid session)
        {
            var s = CoreManager.GetSession(session);
            var uiC = s.UiClientConnection;
            await Clients.Client(uiC).SendAsync("timesAsync");
            foreach(var p in s.players)
                await Clients.Client(p.ConnectionId).SendAsync("timesUp");
        }

        public async Task DrawSubmitted(Context context)
        {
            //TODO: Apenas chamar front-end quando todos os desenhos tiverem sido recebidos.
            //Implementar uma queue de desenhos, se são recebidos 5 mostrar os 5
            //Os critérios para mostrar o resultados são um timeout, enviado pelo cliente, ou todos os users acertarem

            if (CoreManager.AllDrawsSubmitted(context))
            {
                foreach (var p in CoreManager.GetSession(context.Session).players)
                {
                    await Clients.Client(p.ConnectionId).SendAsync("TryAndGuess");
                }

                await Clients.Client(CoreManager.GetUiClient(context))
                .SendAsync("ShowDrawing", CoreManager.GetSession(context.Session)
                .players.Where(p => p.PlayerId == context.PlayerId)
                .FirstOrDefault()
                .Draws
                .FirstOrDefault());
                
            }

            
        }

        //Para ja não é preciso ser enviada uma cor, no entanto futuramente as cores irão 
        //servir para indicar ao player quão próximo está, ou não, da resposta certa.
        //Podera ser cor ou um enum que depois é interpretado no FE aplicando a respectiva cor.
        /*
          SendAsync("DisplayGuess", userGuess) userGuess -> {userName: "Briceno", guess: "GoT", isCorrect: false, color: "#000"}
         */
        public async Task SendGuess(Context context, string guess)
        {

            var currGuess = CoreManager.GetSession(context.Session).currentTheme;
            if (guess.ToLower().Trim() == currGuess.ToLower().Trim())
                await Clients.Caller.SendAsync("RightGuess");
            else
                await Clients.Caller.SendAsync("WrongGuess");
        }

        //Utilizar o ShowScores no contexto de outro método para notificar o front-end
        //Adicionar um boolean na resposta para avisar quando o jogo tiver terminado
        //e colocar no android um botão de NewGame
        /*
            SendAsync("ShowScores", scores) scores -> {player1: 1, player2: 20, player3: 14...}
            */

        //Recomendo não haver nova ronda, para o beta. No entanto se for para suportar,
        //no fim do ScoreBoard ter sido mostrado, por exemplo após 10 segundos, contados pelo servidor,
        //será necessário chamar o NewRound de forma a colocar o FE e o Android nos devidos ecrãs,
        //sendo o ecrã do android o que mostra o tema e a tela e o do FE o que tem o contador à espera que
        //sejam terminados os desenhos.
        public async Task NewRound()
        {
            /*
              SendAsync("NewRound")
             */
        }


        public async Task NewGame()
        {
            //TODO: Metodo chamado pelo android quando 1 jogo termina e o user quer jogar novamente.
            //Notificar o FE do sucedido de forma a ser enviado para a de start drawing
            /*
              SendAsync("NewGame", roomCode)
             */
        }
    }
}
