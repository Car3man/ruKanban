﻿using System;

namespace RuKanban.Services.Api.JsonModel
{
    [Serializable]
    public class Board
    {
        public string id;
        public string workspace_id;
        public string name;
        public string description;
        public string created_at;
    }
}