namespace CRM.SocialDepartment.Domain.Entities.Patietns.Documents
{
    public class Passport
    {
        public string Data { get; private set; }


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Passport()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        {

        }

        public Passport(string data)
        {
            Data = data;
        }
    }
}
