using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.GameManager;
using SignalRTest.Logic;

namespace SignalRTest.Hubs
{
    public class DrawWarsHub : Hub
    {
        
        public override Task OnConnectedAsync()
        {
            var teste = base.Context;
            return base.OnConnectedAsync();

        }

        #region Pre game phase

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

        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", new { SessionRoom = session.Room, SessionId = session.SessionId });
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
            var playerConnections = CoreManager.GetContextPlayerConnectionId(context);

            await Clients.Clients(playerConnections).SendAsync("DrawThemes", CoreManager.GetSession(context.Session).GetThemes());
            await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("DrawThemes", 60);
        }

        #endregion

        #region Game Logic

        public async Task SetTimesUp(Guid session)
        {
            var s = CoreManager.GetSession(session);
            var uiC = s.UiClientConnection;
            
            await Clients.Clients(CoreManager.GetSession(session).players.Select(p=>p.ConnectionId).ToList()).SendAsync("TimesUp");
        }

        public async Task DrawSubmitted(Context context)
        {

            if (CoreManager.AllDrawsSubmitted(context))
            {
                await DrawPhaseLogic(context.Session);
            }
        }

        public async Task DrawPhaseLogic(Guid session)
        {
            var nextDraw = CoreManager.GetSession(session).nextDraw();
            if (!string.IsNullOrWhiteSpace(nextDraw))
            {
                CoreManager.GetSession(session).ResetPlayerGuesses();
                await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("TryAndGuess");
                await Clients.Client(CoreManager.GetUiClient(session))
                .SendAsync("ShowDrawing", new
                {
                    drawUrl = nextDraw,
                    timeout = 60
                });
            }
            else
            {
                await Clients.All.SendAsync("EndOfGame");
            }
        }

        public async Task NextGamePhase(Guid session)
        {

            await Clients.Clients(CoreManager.GetSession(session).players.Select(p=>p.ConnectionId).ToList()).SendAsync("SeeResults");
            await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("SeeResults", 15);
        }

        public async Task ResultsShown(Guid session)
        {
            if (!CoreManager.GetSession(session).AllDrawsShown())
                await DrawPhaseLogic(session);
            
            else
                await Clients.All.SendAsync("EndOfGame");
        }

        public async Task SendGuess(Context context, string guess)
        {

            var currGuess = CoreManager.GetSession(context.Session).currentTheme;
            if (guess.ToLower().Trim() == currGuess.ToLower().Trim())
            {
                CoreManager.GetSession(context.Session).PlayerGuessedCorrectly(context.PlayerId);
                await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("PlayerGuess", new
                {
                    guess,
                    isCorrect = true,
                    player = CoreManager.GetSession(context.Session).players.FirstOrDefault(p => p.PlayerId == context.PlayerId).nickname
                });

                await Clients.Caller.SendAsync("RightGuess");
                if (CoreManager.AllGuessedCorrectly(context))
                    await NextGamePhase(context.Session);
            }
            else
            {
                await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("PlayerGuess", new
                {
                    guess,
                    isCorrect = false,
                    player = CoreManager.GetSession(context.Session).players.FirstOrDefault(p => p.PlayerId == context.PlayerId).nickname
                });
                await Clients.Caller.SendAsync("WrongGuess");
            }

        }
        
        #endregion


        #region not yet implemented
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
        #endregion

    }
}
