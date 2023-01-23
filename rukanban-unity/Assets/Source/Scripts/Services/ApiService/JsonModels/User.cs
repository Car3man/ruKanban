using System;

namespace RuKanban.Services.Api.JsonModel
{
    [Serializable]
    public class User
    {
        public string id;
        public string login;
        public string first_name;
        public string sur_name;
        public string patronymic;
    }
}