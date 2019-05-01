using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GameofThronesCharacters
{
    public class Function
    {

        private static HttpClient httpClient;

        public function()
        {
            httpClient = new HttpClient();
        }




        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }

        public async Task<SKillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var requestType = input.GetRequestType();
            string outputText = "";

            if (requestType == typeof(LaunchRequest))

            {

                return BodyResponse("Hello traveler. Good day to you. Say the name of a Game of Thrones character to enhance your Thrones intellect!", false);

            }

            else if (requestType == typeof(IntentRequest))
            {
                var intent = input.Request as IntentRequest;

                if (intent.Intent.Name.Equals("CharacterIntent"))
                {
                    var characterRequestedName = intent.Intent.Slots["name"].Value;
                    

                    if (characterRequestedName == null)
                    {
                        return BodyResponse("I have no idea who you are talking about, you should try to say the first name someone i know", false);
                    }
                    
                    else if (intent.Intent.Name.Equals("AMAZON.StopIntent"))

                    {

                        return BodyResponse("You have now exited the Thrones Universe", true);

                    }


                    else 

                    {

                        return BodyResponse("Am confused. Please try again.", false);

                    }

                    var characterInfo = await GetCharacterInfo(characterRequestedName, context);
                    {
                         outputText = "This is " + characterInfo.name + "of house " + characterInfo.allegiances;
                    }

                    return BodyResponse(outputText, true);


            }
            }
        }

        private SkillResponse BodyResponse(string outputSpeech, bool shouldEndSession, string repromptText = "Just say, tell me about Jon Snow. To exit, say, exit.")
        {
            var response = new ResponseBody
            {
                shouldEndSession = ShouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { TextReader = outputSpeech }
            };

            if (repromtText != null)
            {
                response.Repromt = new Reprompt() { outputSpeech = new PlainTextOutputSpeech() { TextReader = repromptText } };
            }

            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0"
            };
            return skillResponse;
        }

        private async Task<Character> GetCharacterInfo(string name)
        {
            Character character = new Character();

            var uri = new Uri($"https://www.anapioficeandfire.com/api");

            try
            {
                var response = await httpClient.GetStringAsync(uri);
                context.Logger.LogLine($"Response from URL:\n{response}");
                character = JsonConvert.DeserializeObject<List<Character>>(response).FirstorDefault();
            }

            catch (Exception ex)

            {

                Context.Logger.LogLine($"\nException: {ex.Message}");

                Context.Logger.LogLine($"\nStack Trace: {ex.StackTrace}");

            }

            return character;
        }


    }



    
}
