using MySyntheticNetworkMessenger.Models;
using System.Text;

namespace MySyntheticNetworkMessenger.Services
{
    public interface ITemplateBuilderService
    {
        string BuildInstructionTemplate(ContactData contactData);
    }
    public class TemplateBuilderService : ITemplateBuilderService
    {
        public TemplateBuilderService() { }

        public string BuildInstructionTemplate(ContactData contactData)
        {
            var instructionBuilder = new StringBuilder();
            instructionBuilder.AppendFormat("Alla svar ska vara på språket Svenska (SE-sv). Ditt namn är {0} och du är en {1}. Svara som att du är {2} år. Erbjud inte att vara hjälpsam om inte du blir tillfrågad att hjälpa till. Fråga inte vad du kan hjälpa till med. ",
                                            contactData.Name,
                                            contactData.ManKvinna ? "Kvinna" : "Man",
                                            contactData.Age);

            instructionBuilder.Append(GetFormalityInstruction(contactData));
            instructionBuilder.Append(GetPolitenessInstruction(contactData));
            instructionBuilder.Append(GetConfidenceInstruction(contactData));
            instructionBuilder.Append(GetHumorInstruction(contactData));
            instructionBuilder.Append(GetGoofinessInstruction(contactData));
            instructionBuilder.Append(GetEraInstruction(contactData));
            instructionBuilder.Append(GetShortAnswersInstruction(contactData));

            return instructionBuilder.ToString();
        }

        private static string GetFormalityInstruction(ContactData contactData)
        {
            return contactData.Formality switch
            {
                <= 2 => "Dina svar ska vara väldigt informella. Du är bekväm med att vara lite slapp. ",
                >= 3 and <= 4 => "Dina svar ska ha en tendens av formalitet. ",
                5 => "Dina svar ska vara väldigt formella. ",
                _ => throw new NotImplementedException(),
                //_ => string.Empty
            };
        }

        private static string GetPolitenessInstruction(ContactData contactData)
        {
            return contactData.Politeness switch
            {
                <= 3 => "Dina svar behöver inte alls vara artiga. ",
                >= 4 => "Dina svar måste vara väldigt artiga. "
            };
        }

        private static string GetConfidenceInstruction(ContactData contactData)
        {
            return contactData.Confidence switch
            {
                <= 3 => "Du är inte så självsäker. ",
                >= 4 => "Du är väldigt självsäker. "
            };
        }

        private static string GetHumorInstruction(ContactData contactData)
        {
            return contactData.Humor switch
            {
                <= 3 => "Du är inte så humoristisk. ",
                >= 4 => "Du är väldigt humoristisk. "
            };
        }

        private static string GetGoofinessInstruction(ContactData contactData)
        {
            return contactData.Goofiness switch
            {
                <= 3 => "Du är inte så knasighet. ",
                >= 4 => "Du är väldigt knasighet. "
            };
        }

        private static string GetShortAnswersInstruction(ContactData contactData)
        {
            return contactData.ShortAnswers switch
            {
                <= 2 => "Svara så kort som möjligt. ",
                >= 3 and <= 4 => "Svara med bekvämt långa svar. ",
                >= 5 => "Håll inte igen med svarets längd. "
            };
        }

        private static string GetEraInstruction(ContactData contactData)
        {
            return contactData.Era switch
            {
                Era.TwoThousand => "Svara som om du är född efter 2000. ",
                Era.Nineties => "Svara som om du är född någon gång mellan 1990 - 1999. ",
                Era.Eighties => "Svara som om du är född någon gång mellan 1980 - 1989. ",
                Era.Seventies => "Svara som om du är född någon gång mellan 1970 - 1979. ",
                Era.Sixties => "Svara som om du är född någon gång mellan 1960 - 1969. ",
                Era.Fifties => "Svara som om du är född någon gång mellan 1950 - 1959. ",
                Era.None => string.Empty,
                _ => throw new NotImplementedException()
            };
        }
    }

}
