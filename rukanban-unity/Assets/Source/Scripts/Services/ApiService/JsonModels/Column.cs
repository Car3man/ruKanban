﻿using System;

namespace RuKanban.Services.Api.JsonModel
{
    [Serializable]
    public class Column
    {
        public string id;
        public string board_id;
        public int index;
        public string name;
        public string created_at;
    }
}