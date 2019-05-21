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
            //Implementar uma queue de desenhos, se são recebidos 5 mostrar os 5 antes de chamar GuessingOver
            //Os critérios para mostrar o desenho seguinte são um timeout, enviado pelo cliente, ou todos os users acertarem
            await Clients.Client(CoreManager.GetUiClient(context))
                .SendAsync("ShowDrawing", CoreManager.GetSession(context.Session)
                .players.Where(p => p.PlayerId == context.PlayerId)
                .FirstOrDefault()
                .Draws
                .FirstOrDefault());
        }

        //Este método poderá ser opcional para a beta.
        public async Task DrawExpired()
        {
            //TODO: Chamado pelo front-end (FE), notifica o servidor que o tempo de desenho terminou (90 segundos?)
            //Notificar o android, sendo forçado o envio do canvas actual, esteja ou não terminado.
            //No fim chamar SendAsync("ShowDrawing") com o primeiro desenho.
        }

        public async Task SubmitGuess()
        {
            //TODO: Cada guess do Android é recebida e enviada para a respectiva sessão
            //sendo validada pelo servidor se é ou não a correta.
            //Para ja não é preciso ser enviada uma cor, no entanto futuramente as cores irão 
            //servir para indicar ao player quão próximo está, ou não, da resposta certa.
            //Podera ser cor ou um enum que depois é interpretado no FE aplicando a respectiva cor.
            /*
              SendAsync("DisplayGuess", userGuess) userGuess -> {userName: "Briceno", guess: "GoT", isCorrect: false, color: "#000"}
             */
        }

        //Este método poderá ser opcional para a beta.
        public async Task GuessExpired()
        {
            //TODO: Chamado pelo front-end (FE), notifica o servidor para passar ao desenho seguinte,
            //já que o tempo de guess terminou (90 segundos?)
            //No fim chamar novamente SendAsync("ShowDrawing") com o desenho seguinte.
        }

        public async Task GuessingOver()
        {
            //TODO: Quando todos os desenhos, desta ronda, são mostrados notificar o front-end com a 
            //pontuação global. Caso seja a última ronda chamar "GameFinished" em vez de "ShowScores"
            /*
              SendAsync("ShowScores", scores) scores -> {player1: 1, player2: 20, player3: 14...}
             */
        }

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

        public async Task GameFinished()
        {
            //TODO: Chamar sempre este método no fim do jogo. Caso haja rondas, apenas chamar quando todas
            //tiverem terminado. Mostrar no FE o scoreboard final e no android um botão de NewGame
            /*
              SendAsync("GameFinished", scores) -> scores -> {player1: 1, player2: 20, player3: 14...}
             */
        }

        public async Task NewGame()
        {
            //TODO: Metodo chamado pelo android quando 1 jogo termina e o user quer jogar novamente.
            //Notificar o FE do sucedido de forma a ser enviado para a página de join, com um novo
            //room code.
            /*
              SendAsync("NewGame", roomCode)
             */
        }

    }
}
