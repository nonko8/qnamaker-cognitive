using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Web.Configuration;

using System.Net.Http;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System.Text;
using BotApp.Model;
using BotApp.Service;

namespace LuisBot.Dialogs
{
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Weather.GetForecast")]
        [LuisIntent("Weather.GetCondition")]
        public async Task getWeatherIntent(IDialogContext context, LuisResult result)
        {
            var weatherQuery = new WeatherQuery();

            EntityRecommendation LocationRecommendation;
            if (result.TryFindEntity(ENTITY_LOCATION, out LocationRecommendation))
            {
                weatherQuery.Location = LocationRecommendation.Entity;
            }

            var weatherFormDialog = new FormDialog<WeatherQuery>(weatherQuery, this.BuildWeatherForm, FormOptions.PromptInStart, result.Entities);
            context.Call(weatherFormDialog, this.ResumeAfterFormDialog);
        }

        private IForm<WeatherQuery> BuildWeatherForm()
        {
            OnCompletionAsyncDelegate<WeatherQuery> processWeatherSearch = async (context, state) =>
            {
                var message = "Searching...";
                if (!string.IsNullOrEmpty(state.Location))
                {
                    message += $" in {state.Location}...";
                }
                await context.PostAsync(message);
            };

            return new FormBuilder<WeatherQuery>()
                .Field(nameof(WeatherQuery.Location))
                .OnCompletion(processWeatherSearch)
                .Build();
        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<WeatherQuery> result)
        {
            // 天気APIの呼び出し処理
        }
    }
}